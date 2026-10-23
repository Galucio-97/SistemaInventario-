using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SI.Web.Data.Entities;
using SI.Web.Services;

namespace SI.Web.Pages.Productos;

public class IndexModel(ProductoService productoService) : PageModel
{
    public IReadOnlyList<Producto> Productos { get; private set; } = [];

    [TempData]
    public string? MensajeExito { get; set; }

    [TempData]
    public string? MensajeError { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        Productos = await productoService.ListarTodosAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(int id, bool nuevoEstado, CancellationToken cancellationToken = default)
    {
        var resultado = await productoService.CambiarEstadoAsync(id, nuevoEstado, cancellationToken);
        if (resultado.Exitoso)
        {
            MensajeExito = $"El estado del producto fue actualizado a {(nuevoEstado ? "Activo" : "Inactivo")}.";
        }
        else
        {
            MensajeError = resultado.MensajeError;
        }

        return RedirectToPage("./Index");
    }
}
