using Microsoft.EntityFrameworkCore;
using SI.Web.Common;
using SI.Web.Data;
using SI.Web.Data.Entities;

namespace SI.Web.Services;

/// <summary>
/// Servicio de dominio para administración de productos (RF-01, RF-05, RB-10, SI-020, SI-021, SI-022).
/// </summary>
public class ProductoService(InventarioDbContext context)
{
    public const string MensajeProductoNoExiste = "El producto no existe";

    public async Task<ResultadoOperacion> RegistrarAsync(
        string codigo,
        string nombre,
        string unidadMedida,
        string? descripcion = null,
        CancellationToken cancellationToken = default)
    {
        var validacion = ValidadorProducto.Validar(codigo, nombre, unidadMedida, descripcion);
        if (!validacion.EsValido)
        {
            return new ResultadoOperacion(false, validacion.MensajeError);
        }

        var existe = await context.Productos
            .AnyAsync(p => p.Codigo == codigo, cancellationToken);

        if (existe)
        {
            return new ResultadoOperacion(false, $"Ya existe un producto con el código {codigo}");
        }

        var producto = new Producto
        {
            Codigo = codigo.Trim(),
            Nombre = nombre.Trim(),
            UnidadMedida = unidadMedida.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
            Activo = true,
            ExistenciaActual = 0m,
            CostoPromedioActual = 0m,
            ValorInventarioPeps = 0m,
            ValorInventarioPromedio = 0m
        };

        context.Productos.Add(producto);
        await context.SaveChangesAsync(cancellationToken);

        return new ResultadoOperacion(true, EntidadId: producto.Id);
    }

    /// <summary>
    /// Consulta un producto por su clave primaria (Id).
    /// </summary>
    public async Task<ResultadoConsultaProducto> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var producto = await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (producto is null)
        {
            return new ResultadoConsultaProducto(false, MensajeError: MensajeProductoNoExiste);
        }

        return new ResultadoConsultaProducto(true, Producto: producto);
    }

    /// <summary>
    /// Consulta un producto por su clave única de negocio (Codigo).
    /// </summary>
    public async Task<ResultadoConsultaProducto> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        var codigoLimpio = codigo?.Trim() ?? string.Empty;

        var producto = await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Codigo == codigoLimpio, cancellationToken);

        if (producto is null)
        {
            return new ResultadoConsultaProducto(false, MensajeError: MensajeProductoNoExiste);
        }

        return new ResultadoConsultaProducto(true, Producto: producto);
    }

    /// <summary>
    /// Retorna todos los productos registrados mostrando su estado y existencia física.
    /// </summary>
    public async Task<IReadOnlyList<Producto>> ListarTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Productos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Modifica los datos descriptivos y el estado de un producto existente (RF-01, RB-10, SI-022).
    /// El código del producto no se modifica. No altera existencias, costos ni movimientos.
    /// </summary>
    public async Task<ResultadoOperacion> ModificarAsync(
        int id,
        string nombre,
        string unidadMedida,
        string? descripcion = null,
        bool? activo = null,
        CancellationToken cancellationToken = default)
    {
        // Stub inicial para fase RED
        return await Task.FromResult(new ResultadoOperacion(false, "No implementado"));
    }

    /// <summary>
    /// Cambia el estado de activación (Activo/Inactivo) de un producto (RF-01, RB-10, SI-022).
    /// </summary>
    public async Task<ResultadoOperacion> CambiarEstadoAsync(
        int id,
        bool activo,
        CancellationToken cancellationToken = default)
    {
        // Stub inicial para fase RED
        return await Task.FromResult(new ResultadoOperacion(false, "No implementado"));
    }
}
