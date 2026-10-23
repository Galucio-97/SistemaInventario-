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

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        var resultado = await productoService.ObtenerPorIdAsync(id, cancellationToken);
        if (!resultado.Encontrado || resultado.Producto is null)
        {
            TempData["MensajeError"] = resultado.MensajeError ?? ProductoService.MensajeProductoNoExiste;
            return RedirectToPage("./Index");
        }

        Input = new InputModel
        {
            Id = resultado.Producto.Id,
            Codigo = resultado.Producto.Codigo,
            Nombre = resultado.Producto.Nombre,
            UnidadMedida = resultado.Producto.UnidadMedida,
            Descripcion = resultado.Producto.Descripcion,
            Activo = resultado.Producto.Activo
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken = default)
    {
        var resultado = await productoService.ModificarAsync(
            id,
            Input.Nombre,
            Input.UnidadMedida,
            Input.Descripcion,
            Input.Activo,
            cancellationToken);

        if (!resultado.Exitoso)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return Page();
        }

        TempData["MensajeExito"] = "Producto modificado exitosamente.";
        return RedirectToPage("./Index");
    }
}
