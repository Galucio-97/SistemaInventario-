using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SI.Web.Common;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Interfaces;
using SI.Web.Services;

namespace SI.Web.Pages.Inventario;

public class ConsultaModel(
    IInventarioService inventarioService,
    ProductoService productoService,
    IEjercicioReferenciaService? ejercicioReferenciaService = null) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? ProductoId { get; set; }

    [BindProperty(SupportsGet = true)]
    public MetodoCosteo Metodo { get; set; } = MetodoCosteo.Peps;

    public List<SelectListItem> ProductosDisponibles { get; set; } = [];

    public ResultadoConsultaExistencias? Resultado { get; private set; }

    public ResultadoConsultaExistencias? ResultadoComparativo { get; private set; }

    public ResultadoHistorialMovimientos? Historial { get; private set; }

    public IReadOnlyList<MovimientoComparativoDto> HistorialComparativo { get; private set; } = [];

    [TempData]
    public string? MensajeExito { get; set; }

    public string? MensajeError { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        var productos = await productoService.ListarTodosAsync(cancellationToken);
        ProductosDisponibles = productos
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Codigo} — {p.Nombre} ({(p.Activo ? "Activo" : "Inactivo")})",
                Selected = p.Id == ProductoId
            })
            .ToList();

        if (ProductoId.HasValue && ProductoId.Value > 0)
        {
            Resultado = await inventarioService.ConsultarExistenciasAsync(
                ProductoId.Value, Metodo, cancellationToken);

            if (!Resultado.Encontrado)
            {
                MensajeError = Resultado.MensajeError ?? "El producto no existe.";
            }
            else
            {
                // Cargar también el método alternativo para mostrar la comparativa en pantalla
                var metodoAlterno = Metodo == MetodoCosteo.Peps 
                    ? MetodoCosteo.PromedioPonderado 
                    : MetodoCosteo.Peps;

                ResultadoComparativo = await inventarioService.ConsultarExistenciasAsync(
                    ProductoId.Value, metodoAlterno, cancellationToken);

                // Cargar el historial tipo kardex según el método seleccionado (SI-032)
                Historial = await inventarioService.ObtenerHistorialMovimientosAsync(
                    ProductoId.Value, Metodo, cancellationToken);

                HistorialComparativo = await inventarioService.ObtenerHistorialComparativoAsync(
                    ProductoId.Value, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Acción con un solo clic para cargar el ejercicio de referencia E-01 (SI-034, CP-08, CP-09).
    /// </summary>
    public async Task<IActionResult> OnPostCargarEjercicioE01Async(CancellationToken cancellationToken = default)
    {
        if (ejercicioReferenciaService is null)
        {
            MensajeError = "El servicio de carga de ejercicio de referencia no está disponible.";
            await OnGetAsync(cancellationToken);
            return Page();
        }

        var resultado = await ejercicioReferenciaService.CargarEjercicioE01Async(cancellationToken);
        if (resultado.Exitoso)
        {
            MensajeExito = resultado.Mensaje;
            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToPage("./Consulta", new { ProductoId = resultado.ProductoId, Metodo });
        }

        MensajeError = resultado.Mensaje;
        await OnGetAsync(cancellationToken);
        return Page();
    }
}
