using SI.Web.Common;
using SI.Web.Interfaces;

namespace SI.Web.Services;

/// <summary>
/// Servicio para carga automatizada e idempotente del ejercicio de referencia E-01 (CP-08, CP-09, SI-034).
/// Diseñado bajo principios MEST: Mantenible, Eficiente, Seguro y Testeable.
/// </summary>
public class EjercicioReferenciaService(
    ProductoService productoService,
    IInventarioService inventarioService) : IEjercicioReferenciaService
{
    /// <summary>
    /// Carga el escenario de referencia E-01 reutilizando los servicios oficiales de dominio (ProductoService e InventarioService).
    /// Si el producto ya existe, no duplica registros ni movimientos (idempotente).
    /// </summary>
    public async Task<ResultadoCargaEjercicio> CargarEjercicioE01Async(CancellationToken ct = default)
    {
        // 1. Validar si ya existe el producto de demostración (Idempotencia)
        var productoExistente = await productoService.ObtenerPorCodigoAsync(
            IEjercicioReferenciaService.CodigoProductoE01, ct);

        if (productoExistente.Encontrado && productoExistente.Producto is not null)
        {
            return ResultadoCargaEjercicio.Existente(
                productoExistente.Producto.Id,
                productoExistente.Producto.Codigo,
                "El ejercicio de referencia E-01 ya se encuentra cargado en el sistema."
            );
        }

        // 2. Registrar el Producto de demostración usando ProductoService
        var registroProducto = await productoService.RegistrarAsync(
            codigo: IEjercicioReferenciaService.CodigoProductoE01,
            nombre: "Tornillo 1/4 (Ejercicio E-01)",
            unidadMedida: "pieza",
            descripcion: "Producto para demostración y evaluación del ejercicio de referencia E-01",
            cancellationToken: ct
        );

        if (!registroProducto.Exitoso || !registroProducto.EntidadId.HasValue)
        {
            return ResultadoCargaEjercicio.Fallido(
                registroProducto.MensajeError ?? "No fue posible registrar el producto de demostración."
            );
        }

        var productoId = registroProducto.EntidadId.Value;

        // 3. Registrar los movimientos oficiales de E-01 en orden cronológico estricto
        // Movimiento 1 (Entrada 1): 10 piezas @ $20.00
        var entrada1 = await inventarioService.RegistrarEntradaAsync(
            productoId: productoId,
            fecha: new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc),
            cantidad: 10.0000m,
            costoUnitario: 20.0000m,
            ct: ct
        );

        if (!entrada1.Exitoso)
        {
            return ResultadoCargaEjercicio.Fallido($"Error en Entrada 1 de E-01: {entrada1.MensajeError}");
        }

        // Movimiento 2 (Entrada 2): 10 piezas @ $30.00
        var entrada2 = await inventarioService.RegistrarEntradaAsync(
            productoId: productoId,
            fecha: new DateTime(2026, 10, 2, 10, 0, 0, DateTimeKind.Utc),
            cantidad: 10.0000m,
            costoUnitario: 30.0000m,
            ct: ct
        );

        if (!entrada2.Exitoso)
        {
            return ResultadoCargaEjercicio.Fallido($"Error en Entrada 2 de E-01: {entrada2.MensajeError}");
        }

        // Movimiento 3 (Salida 1): 5 piezas
        var salida1 = await inventarioService.RegistrarSalidaAsync(
            productoId: productoId,
            fecha: new DateTime(2026, 10, 3, 10, 0, 0, DateTimeKind.Utc),
            cantidad: 5.0000m,
            ct: ct
        );

        if (!salida1.Exitoso)
        {
            return ResultadoCargaEjercicio.Fallido($"Error en Salida 1 de E-01: {salida1.MensajeError}");
        }

        return ResultadoCargaEjercicio.Creado(
            productoId,
            IEjercicioReferenciaService.CodigoProductoE01,
            "Ejercicio de referencia E-01 cargado exitosamente."
        );
    }
}
