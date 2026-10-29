using Microsoft.EntityFrameworkCore;
using SI.Web.Common;
using SI.Web.Data;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Interfaces;

namespace SI.Web.Services;

/// <summary>
/// Servicio de dominio para registro transaccional de movimientos de inventario (RF-02, RF-03, RB-04..07, SI-024, SI-025).
/// </summary>
public class InventarioService(InventarioDbContext context) : IInventarioService
{
    public const string MensajeProductoNoExiste = "El producto no existe";
    public const string MensajeProductoInactivo = "El producto estÃ¡ inactivo";

    /// <summary>
    /// Registra una entrada de inventario de forma atÃ³mica actualizando existencias, capas PEPS y promedio ponderado (RF-02, SI-024).
    /// </summary>
    public async Task<ResultadoOperacion> RegistrarEntradaAsync(
        int productoId, DateTime fecha, decimal cantidad, decimal costoUnitario, CancellationToken ct = default
    )
    {
        var cantidadEsValida = ValidadorCantidad.Validar(cantidad);

        if (!cantidadEsValida.EsValido)
            return new ResultadoOperacion(false, cantidadEsValida.MensajeError);
        
        var costoUnitarioEsValido = ValidadorCostoUnitario.Validar(costoUnitario);
        
        if (!costoUnitarioEsValido.EsValido)
            return new ResultadoOperacion(false, costoUnitarioEsValido.MensajeError);
        
        var producto = await context.Productos.FirstOrDefaultAsync(p => p.Id == productoId, ct);

        if (producto == null) 
            return new ResultadoOperacion(false, MensajeProductoNoExiste);
        
        if (!producto.Activo)
            return new ResultadoOperacion(false, MensajeProductoInactivo);

        var calculoPromedioPonderado = CalculadoraPromedioPonderado.CalcularEntrada(
            producto.ExistenciaActual, producto.ValorInventarioPromedio, cantidad, costoUnitario
        );
        
        var valorEntrada = PoliticaRedondeo.Importe(cantidad * costoUnitario);
        var nuevoValorPeps = PoliticaRedondeo.Importe(producto.ValorInventarioPeps + valorEntrada);
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        // Crear Movimiento
        var movimiento = new Movimiento
        {
            ProductoId = producto.Id,
            Fecha = fecha,
            Tipo = TipoMovimiento.Entrada,
            Cantidad = cantidad,
            CostoUnitarioEntrada = costoUnitario,
            ExistenciaResultante = calculoPromedioPonderado.Existencia,
            CostoUnitarioPeps = costoUnitario,
            CostoTotalPeps = valorEntrada,
            SaldoValorPeps = nuevoValorPeps,
            CostoUnitarioPromedio = calculoPromedioPonderado.CostoPromedio,
            CostoTotalPromedio = valorEntrada,
            SaldoValorPromedio = calculoPromedioPonderado.SaldoValor
        };
        context.Movimientos.Add(movimiento);
        await context.SaveChangesAsync(ct);

        // Crear Capa PEPS
        var capa = CalculadoraPeps.CrearCapa(fecha, cantidad, costoUnitario, producto.Id, movimiento.Id);
        context.CapasInventarioPeps.Add(capa);

        // Actualizar saldos del Producto
        producto.ExistenciaActual = calculoPromedioPonderado.Existencia;
        producto.CostoPromedioActual = calculoPromedioPonderado.CostoPromedio;
        producto.ValorInventarioPromedio = calculoPromedioPonderado.SaldoValor;
        producto.ValorInventarioPeps = nuevoValorPeps;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new ResultadoOperacion(true, null, movimiento.Id);
    }

    /// <summary>
    /// Registra una salida de inventario de forma atÃ³mica consumiendo capas PEPS, aplicando costo promedio vigente
    /// y recalculando saldos patrimoniales (RF-03, SI-025).
    /// </summary>
    public async Task<ResultadoOperacion> RegistrarSalidaAsync(
        int productoId, DateTime fecha, decimal cantidad, CancellationToken ct = default
    )
    {
        var cantidadEsValida = ValidadorCantidad.Validar(cantidad);

        if (!cantidadEsValida.EsValido)
            return new ResultadoOperacion(false, cantidadEsValida.MensajeError);
        
        var producto = await context.Productos
                                    .FirstOrDefaultAsync(p => p.Id == productoId, ct);

        if (producto == null) 
            return new ResultadoOperacion(false, MensajeProductoNoExiste);
        
        if (!producto.Activo)
            return new ResultadoOperacion(false, MensajeProductoInactivo);
        
        var existenciaValida = ValidadorExistencia.Validar(cantidad, producto.ExistenciaActual);
        if (!existenciaValida.EsValido)
            return new ResultadoOperacion(false, existenciaValida.MensajeError);
        
        var calculoPromedioPonderado = CalculadoraPromedioPonderado.CalcularSalida(
            producto.ExistenciaActual,
            producto.ValorInventarioPromedio,
            producto.CostoPromedioActual,
            cantidad
        );
        
        var capas = await context.CapasInventarioPeps
            .Where(c => c.ProductoId == productoId && c.CantidadDisponible > 0)
            .OrderBy(c => c.FechaEntrada)
            .ThenBy(c => c.Id)
            .ToListAsync(ct);

        var calculoSalidaPEPS = CalculadoraPeps.CalcularSalida(capas, cantidad);
        
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var movimiento = new Movimiento
        {
            ProductoId = producto.Id,
            Fecha = fecha,
            Tipo = TipoMovimiento.Salida,
            Cantidad = cantidad,
            ExistenciaResultante = calculoPromedioPonderado.Existencia,
            CostoUnitarioPeps = calculoSalidaPEPS.CostoUnitarioEquivalente,
            CostoTotalPeps = calculoSalidaPEPS.CostoTotalSalida,
            SaldoValorPeps = calculoSalidaPEPS.SaldoValorRestante,
            CostoUnitarioPromedio = calculoPromedioPonderado.CostoPromedio,
            CostoTotalPromedio = calculoPromedioPonderado.CostoTotalSalida,
            SaldoValorPromedio = calculoPromedioPonderado.SaldoValor
        };
        
        context.Movimientos.Add(movimiento);

        // Actualizar los saldos del Producto
        producto.ExistenciaActual = calculoPromedioPonderado.Existencia;
        producto.CostoPromedioActual = calculoPromedioPonderado.CostoPromedio;
        producto.ValorInventarioPromedio = calculoPromedioPonderado.SaldoValor;
        producto.ValorInventarioPeps = calculoSalidaPEPS.SaldoValorRestante;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new ResultadoOperacion(true, null, movimiento.Id);
    }

        /// <summary>
    /// Consulta existencias y valuación de inventario por método de costeo (RF-04, RF-05, CP-10, CP-11, D-09).
    /// Operación idempotente de solo lectura, sin efectos colaterales en la base de datos.
    /// </summary>
    public async Task<ResultadoConsultaExistencias> ConsultarExistenciasAsync(
        int productoId, MetodoCosteo metodo, CancellationToken ct = default)
    {
        var producto = await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productoId, ct);

        if (producto is null)
        {
            return ResultadoConsultaExistencias.NoEncontrado("El producto no existe.");
        }

        if (metodo == MetodoCosteo.PromedioPonderado)
        {
            return ResultadoConsultaExistencias.Exitoso(
                productoId: producto.Id,
                codigo: producto.Codigo,
                nombre: producto.Nombre,
                unidadMedida: producto.UnidadMedida,
                metodo: MetodoCosteo.PromedioPonderado,
                existenciaActual: producto.ExistenciaActual,
                costoUnitario: producto.CostoPromedioActual,
                valorInventario: producto.ValorInventarioPromedio,
                capasActivas: Array.Empty<CapaPepsDto>()
            );
        }

        if (metodo == MetodoCosteo.Peps)
        {
            var capas = await context.CapasInventarioPeps
                .AsNoTracking()
                .Where(c => c.ProductoId == productoId && c.CantidadDisponible > 0)
                .OrderBy(c => c.FechaEntrada)
                .ThenBy(c => c.Id)
                .Select(c => new CapaPepsDto
                {
                    Id = c.Id,
                    FechaEntrada = c.FechaEntrada,
                    CantidadOriginal = c.CantidadInicial,
                    CantidadDisponible = c.CantidadDisponible,
                    CostoUnitario = c.CostoUnitario
                })
                .ToListAsync(ct);

            // D-09: Costo unitario promedio efectivo en PEPS = ValorTotal / Existencia (redondeado a 4 decimales)
            var costoUnitarioPeps = producto.ExistenciaActual > 0
                ? Math.Round(producto.ValorInventarioPeps / producto.ExistenciaActual, 4, MidpointRounding.AwayFromZero)
                : 0.0000m;

            return ResultadoConsultaExistencias.Exitoso(
                productoId: producto.Id,
                codigo: producto.Codigo,
                nombre: producto.Nombre,
                unidadMedida: producto.UnidadMedida,
                metodo: MetodoCosteo.Peps,
                existenciaActual: producto.ExistenciaActual,
                costoUnitario: costoUnitarioPeps,
                valorInventario: producto.ValorInventarioPeps,
                capasActivas: capas
            );
        }

        throw new InvalidOperationException($"Método de costeo no soportado: {metodo}");
    }
}