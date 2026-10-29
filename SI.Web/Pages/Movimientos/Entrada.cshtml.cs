using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SI.Web.Data;
using SI.Web.Data.Entities;
using SI.Web.Interfaces;

namespace SI.Web.Pages.Movimientos;

public class EntradaModel(InventarioDbContext context, IInventarioService inventarioService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<Producto> ProductosActivos { get; private set; } = [];

    public ResultadoMovimientoDto? Resultado { get; private set; }

    public class InputModel
    {
        public int ProductoId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
    }

    public record ResultadoMovimientoDto(
        string CodigoProducto,
        string NombreProducto,
        decimal Cantidad,
        decimal ExistenciaAnterior,
        decimal ExistenciaNueva,
        decimal CostoTotalPeps,
        decimal CostoTotalPromedio
    );

    public async Task OnGetAsync(CancellationToken ct = default)
    {
        await CargarProductosActivosAsync(ct);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct = default)
    {
        var resultado = await inventarioService.RegistrarEntradaAsync(
            Input.ProductoId,
            Input.Fecha,
            Input.Cantidad,
            Input.CostoUnitario,
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

        decimal existenciaAnterior = (producto?.ExistenciaActual ?? Input.Cantidad) - Input.Cantidad;

        Resultado = new ResultadoMovimientoDto(
            producto?.Codigo ?? string.Empty,
            producto?.Nombre ?? string.Empty,
            Input.Cantidad,
            existenciaAnterior,
            producto?.ExistenciaActual ?? Input.Cantidad,
            movimiento?.CostoTotalPeps ?? (Input.Cantidad * Input.CostoUnitario),
            movimiento?.CostoTotalPromedio ?? (Input.Cantidad * Input.CostoUnitario)
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
