using Microsoft.EntityFrameworkCore;
using SI.Web.Common;
using SI.Web.Data;
using SI.Web.Data.Entities;

namespace SI.Web.Services;

/// <summary>
/// Servicio de dominio para administración de productos (RF-01, SI-020).
/// </summary>
public class ProductoService(InventarioDbContext context)
{
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
}
