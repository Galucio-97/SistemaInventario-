# Evidencia Técnica — SI-022: Modificar Producto y Cambiar su Estado

## 1. Resumen del Ticket
- **ID:** `SI-022`
- **Release:** `v0.3.0` (Administración de productos)
- **Responsable:** Dev B
- **Objetivo:** Permitir corregir datos descriptivos (nombre, unidad de medida, descripción) y activar o desactivar productos sin alterar existencias, costos, valores ni movimientos (**RF-01**, **RB-10**, **D-10**, base de **CP-18**).
- **Trazabilidad:** `RF-01` (Gestión de catálogo), `RB-10` (Estado de activación y productos inactivos), `RNF-03` (Validación con mensajes claros), base de `CP-18`.

---

## 2. Criterios de Aceptación y Pruebas Verificadas

### 2.1 Modificación de Datos Descriptivos y Mismas Reglas del Alta
- Modificación de `Nombre`, `UnidadMedida`, `Descripcion`:
  - `Nombre`: obligatorio (`null`, `""`, `"   "` rechazados con `"El nombre del producto es obligatorio."`), longitud máxima 150 caracteres.
  - `UnidadMedida`: obligatoria (`null`, `""`, `"   "` rechazados con `"La unidad de medida es obligatoria."`), longitud máxima 30 caracteres.
  - `Descripcion`: opcional, longitud máxima 500 caracteres.
- Sanitización: `Trim()` aplicado a todas las cadenas para evitar espacios en blanco espurios.

### 2.2 Inmutabilidad del Código de Producto (D-10)
- En `ModificarAsync`, el parámetro del método no permite alterar el `Codigo`.
- En base de datos, el valor de `Codigo` permanece estrictamente idéntico tras la modificación.

### 2.3 Inmutabilidad de Existencias, Costos y Movimientos
- La modificación de atributos descriptivos y estado garantiza que:
  - `ExistenciaActual` permanece idéntica.
  - `CostoPromedioActual`, `ValorInventarioPeps` y `ValorInventarioPromedio` no cambian.
  - No se insertan registros en la tabla `Movimientos`.

### 2.4 Cambio de Estado (Activo / Inactivo — Base CP-18)
- `CambiarEstadoAsync` permite deshabilitar (`Activo = false`) y habilitar (`Activo = true`) el producto.
- Permite preparar las condiciones para que `InventarioService` rechace operaciones futuras sobre productos inactivos con la regla `RB-10` (`CP-18`).

### 2.5 Rechazo ante Producto Inexistente
- Intentos de modificar o cambiar estado sobre identificadores no registrados devuelven `Exitoso = false` y el mensaje `"El producto no existe"`.

---

## 3. Implementación

### `ValidadorProducto.ValidarModificacion`
```csharp
public static ResultadoValidacion ValidarModificacion(string? nombre, string? unidadMedida, string? descripcion = null)
{
    if (string.IsNullOrWhiteSpace(nombre))
        return new ResultadoValidacion(false, MensajeNombreObligatorio);

    if (nombre.Length > MaxLongitudNombre)
        return new ResultadoValidacion(false, MensajeNombreLargo);

    if (string.IsNullOrWhiteSpace(unidadMedida))
        return new ResultadoValidacion(false, MensajeUnidadObligatoria);

    if (unidadMedida.Length > MaxLongitudUnidad)
        return new ResultadoValidacion(false, MensajeUnidadLarga);

    if (!string.IsNullOrEmpty(descripcion) && descripcion.Length > MaxLongitudDescripcion)
        return new ResultadoValidacion(false, MensajeDescripcionLarga);

    return ResultadoValido;
}
```

### `ProductoService.ModificarAsync` y `ProductoService.CambiarEstadoAsync`
```csharp
public async Task<ResultadoOperacion> ModificarAsync(
    int id,
    string nombre,
    string unidadMedida,
    string? descripcion = null,
    bool? activo = null,
    CancellationToken cancellationToken = default)
{
    var validacion = ValidadorProducto.ValidarModificacion(nombre, unidadMedida, descripcion);
    if (!validacion.EsValido)
    {
        return new ResultadoOperacion(false, validacion.MensajeError);
    }

    var producto = await context.Productos
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    if (producto is null)
    {
        return new ResultadoOperacion(false, MensajeProductoNoExiste);
    }

    producto.Nombre = nombre.Trim();
    producto.UnidadMedida = unidadMedida.Trim();
    producto.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();

    if (activo.HasValue)
    {
        producto.Activo = activo.Value;
    }

    await context.SaveChangesAsync(cancellationToken);

    return new ResultadoOperacion(true, EntidadId: producto.Id);
}

public async Task<ResultadoOperacion> CambiarEstadoAsync(
    int id,
    bool activo,
    CancellationToken cancellationToken = default)
{
    var producto = await context.Productos
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    if (producto is null)
    {
        return new ResultadoOperacion(false, MensajeProductoNoExiste);
    }

    producto.Activo = activo;
    await context.SaveChangesAsync(cancellationToken);

    return new ResultadoOperacion(true, EntidadId: producto.Id);
}
```

---

## 4. Resumen de Pruebas Automatizadas

```text
Pruebas del dominio Producto: 38 superadas, 0 con error.
Suite completa de la solución: 97 superadas, 0 con error.
```
