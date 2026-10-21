# Evidencia Técnica — SI-021: Consultar y Listar Productos

## 1. Resumen del Ticket
- **ID:** `SI-021`
- **Release:** `v0.3.0` (Administración de productos)
- **Responsable:** Dev B
- **Objetivo:** Consultar productos por Id o Código, listar el catálogo completo con su estado y existencia física, y garantizar la idempotencia/no mutación ante consultas inexistentes (**RF-01**, **RF-05**, **RB-04**, **CP-11**, **CP-01**).
- **Trazabilidad:** `RF-01` (Gestión de productos), `RF-05` (Consulta de catálogo y estado), `RB-04` (Inmutabilidad de datos en consultas), `CP-01`, `CP-11`.

---

## 2. Criterios de Aceptación y Pruebas Verificadas

### 2.1 Consulta de Producto Existente (CP-01, RF-01, RF-05)
- **Por ID:** Recupera la entidad mapeada con todos sus atributos (`Codigo`, `Nombre`, `UnidadMedida`, `Descripcion`, `Activo`, `ExistenciaActual`, costos).
- **Por Código:** Localiza la entidad mediante el índice único `IX_Productos_Codigo`, ignorando espacios en blanco innecesarios.
- `ResultadoConsultaProducto.Encontrado = true`, `MensajeError = null`.

### 2.2 Consulta de Producto Inexistente (CP-11, RF-01, RF-05, RB-04)
- Búsqueda por ID inexistente (`99999`) o Código inexistente (`NO-EXISTE-999`):
  - Retorna `ResultadoConsultaProducto.Encontrado = false`.
  - Mensaje estandarizado claro: `"El producto no existe"`.
  - `Producto = null`.

### 2.3 Listado Completo de Productos (RF-05)
- Retorna la colección ordenada por código de producto (`OrderBy(p => p.Codigo)`).
- Presenta el estado de activación (`Activo`) y la existencia física actual (`ExistenciaActual`).

### 2.4 Idempotencia y Cero Efectos Colaterales (RB-04, RNF-01)
- La ejecución de consultas (exitosas o fallidas) y listados no altera el conteo de la tabla `Productos` ni genera registros en `Movimientos`.
- Uso exclusivo de `.AsNoTracking()` para evitar retención y tracking en memoria de Entity Framework Core.

---

## 3. Implementación en `ProductoService.cs`

```csharp
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
```

---

## 4. Resumen de Pruebas Automatizadas

```text
Correctas! - Con error: 0, Superado: 9, Omitido: 0, Total: 9 - SI.Tests.dll (net8.0)
Suite Completa: Superado: 67, Con error: 0
```
