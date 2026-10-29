using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SI.Web.Data;
using SI.Web.Data.Entities;
using SI.Web.Interfaces;

namespace SI.Web.Pages.Movimientos;

public class SalidaModel(InventarioDbContext context, IInventarioService inventarioService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<Producto> ProductosActivos { get; private set; } = [];

    public ResultadoSalidaDto? Resultado { get; private set; }

    public class InputModel
    {
        public int ProductoId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;
        public decimal Cantidad { get; set; }
    }

    public record ResultadoSalidaDto(
        string CodigoProducto,
        string NombreProducto,
        decimal Cantidad,
        decimal ExistenciaNueva,
        decimal CostoTotalPeps,
        decimal CostoUnitarioPeps,
        decimal CostoTotalPromedio,
        decimal CostoUnitarioPromedio
    );

    public async Task OnGetAsync(int? productoId = null, CancellationToken ct = default)
    {
        await CargarProductosActivosAsync(ct);
        if (productoId.HasValue)
        {
            Input.ProductoId = productoId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct = default)
    {
        var resultado = await inventarioService.RegistrarSalidaAsync(
            Input.ProductoId,
            Input.Fecha,
            Input.Cantidad,
            ct);

        if (!resultado.Exitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            await CargarProductosActivosAsync(ct);
            return Page();
        }

        var producto = await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == Input.ProductoId, ct);

        var movimiento = await context.Movimientos
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == resultado.EntidadId, ct);

        Resultado = new ResultadoSalidaDto(
            producto?.Codigo ?? string.Empty,
            producto?.Nombre ?? string.Empty,
            Input.Cantidad,
            movimiento?.ExistenciaResultante ?? producto?.ExistenciaActual ?? 0m,
            movimiento?.CostoTotalPeps ?? 0m,
            movimiento?.CostoUnitarioPeps ?? 0m,
            movimiento?.CostoTotalPromedio ?? 0m,
            movimiento?.CostoUnitarioPromedio ?? 0m
        );

        await CargarProductosActivosAsync(ct);
        return Page();
    }

    private async Task CargarProductosActivosAsync(CancellationToken ct)
    {
        ProductosActivos = await context.Productos
            .AsNoTracking()
            .Where(p => p.Activo)
            .OrderBy(p => p.Codigo)
            .ToListAsync(ct);
    }
}
