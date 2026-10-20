# Evidencia Técnica — SI-020: Registrar Producto

## 1. Resumen del Ticket
- **ID:** `SI-020`
- **Release:** `v0.3.0` (Administración de productos)
- **Responsable:** Dev B (Gustavo Lucio)
- **Objetivo:** Dar de alta productos válidos y rechazar los inválidos o duplicados con validación en aplicación y base de datos (**RF-01**, **RNF-02**, **RNF-03**, **CP-01**, **CP-02**).
- **Trazabilidad:** `RF-01` (Gestión de productos), `RNF-02` (Persistencia), `RNF-03` (Validación con mensajes claros), `CP-01`, `CP-02`.

---

## 2. Criterios de Aceptación y Pruebas Verificadas

### 2.1 Validación de Campos Obligatorios y Longitudes Máximas
| Campo | Regla Probada | Valor de Entrada | Resultado | Mensaje Obtenido |
|---|---|:---:|:---:|---|
| Código | Obligatorio | `null`, `""`, `"   "` | **Rechazado** | *"El código del producto es obligatorio."* |
| Código | Longitud máx 50 | 51 caracteres | **Rechazado** | *"El código no puede tener más de 50 caracteres."* |
| Nombre | Obligatorio | `null`, `""`, `"   "` | **Rechazado** | *"El nombre del producto es obligatorio."* |
| Nombre | Longitud máx 150 | 151 caracteres | **Rechazado** | *"El nombre no puede tener más de 150 caracteres."* |
| Unidad Medida | Obligatoria | `null`, `""`, `"   "` | **Rechazado** | *"La unidad de medida es obligatoria."* |
| Unidad Medida | Longitud máx 30 | 31 caracteres | **Rechazado** | *"La unidad de medida no puede tener más de 30 caracteres."* |
| Descripción | Longitud máx 500 | 501 caracteres | **Rechazado** | *"La descripción no puede tener más de 500 caracteres."* |
| Descripción | Opcional | `null` o vacía | **Aceptado** | `EsValido = true` |

### 2.2 Estado Inicial Innegociable y Persistencia Real (CP-01)
- Registro de `P-001` ("Tornillo 1/4", unidad "pieza"):
  - `Activo`: `true`
  - `ExistenciaActual`: `0.0000`
  - `CostoPromedioActual`: `0.0000`
  - `ValorInventarioPeps`: `0.00`
  - `ValorInventarioPromedio`: `0.00`
  - Verificado en SQL Server real mediante lectura con un `DbContext` independiente.

### 2.3 Rechazo de Clave Duplicada (CP-02)
- Intento de registrar `P-001` duplicado vía `ProductoService`:
  - **Rechazado** con mensaje: *"Ya existe un producto con el código P-001"*.
  - En base de datos se mantiene exactamente **1 fila**.
- Intento de inserción directa saltando validaciones de aplicación:
  - Dispara `DbUpdateException` por el índice único relacional `IX_Productos_Codigo`.

---

## 3. Implementación del Servicio (`SI.Web/Services/ProductoService.cs`)

```csharp
namespace SI.Web.Services;

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
```

---

## 4. Ejecución y Resultados

```bash
dotnet test --filter "CP=CP-01|CP=CP-02" --no-restore
```
- Total de pruebas: 18
- Superadas: 18 (100%)
- Fallidas: 0
- Duración total: 2.41 s
