using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SI.Web.Services;

namespace SI.Web.Pages.Productos;

public class CrearModel(ProductoService productoService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        var resultado = await productoService.RegistrarAsync(
            Input.Codigo,
            Input.Nombre,
            Input.UnidadMedida,
            Input.Descripcion,
            cancellationToken);

        if (!resultado.Exitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return Page();
        }

        TempData["MensajeExito"] = $"Producto '{Input.Codigo?.Trim()}' registrado exitosamente.";
        return RedirectToPage("./Index");
    }
}
