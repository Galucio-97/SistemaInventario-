using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SI.Web.Data.Entities;
using SI.Web.Services;

namespace SI.Web.Pages.Productos;

public class IndexModel(ProductoService productoService) : PageModel
{
    public IReadOnlyList<Producto> Productos { get; private set; } = [];

    public async Task OnGetAsync()
    {
        // Stub para fase RED
        Productos = await Task.FromResult<IReadOnlyList<Producto>>([]);
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(int id, bool nuevoEstado)
    {
        // Stub para fase RED
        return await Task.FromResult<IActionResult>(Page());
    }
}
