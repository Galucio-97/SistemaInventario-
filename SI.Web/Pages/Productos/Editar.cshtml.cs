using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SI.Web.Services;

namespace SI.Web.Pages.Productos;

public class EditarModel(ProductoService productoService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Stub para fase RED
        return await Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Stub para fase RED
        return await Task.FromResult<IActionResult>(Page());
    }
}
