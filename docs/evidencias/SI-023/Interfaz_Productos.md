# Evidencia Técnica — SI-023: Interfaz de Administración de Productos

## 1. Resumen del Ticket
- **ID:** `SI-023`
- **Release:** `v0.3.0` (Administración de productos)
- **Responsable:** Dev B
- **Objetivo:** Hacer observable e interactivo en el navegador el comportamiento de **RF-01**, **RF-05**, **RNF-03**, **RB-10** y las decisiones **D-10** (inmutabilidad de código).
- **Trazabilidad:** `RF-01` (Gestión de productos), `RF-05` (Consulta de catálogo), `RNF-03` (Mensajes claros de error), `RB-10` (Estado de activación), `CP-01`, `CP-02`, `CP-11`.

---

## 2. Criterios de Aceptación Verificados

### 2.1 Listado Completo de Productos (`Pages/Productos/Index.cshtml`)
- Visualiza en una tabla limpia y responsiva:
  - `Código`: clave única con tipografía monoespaciada.
  - `Nombre`: descripción nominal.
  - `Descripción`: detalle complementario (o guion `—` si es nulo).
  - `Unidad de Medida`: unidad estándar (`pieza`, `kg`, etc.).
  - `Estado`: indicador visual `Badge` verde (**Activo**) o gris (**Inactivo**).
  - `Existencia`: cantidad física disponible formateada con 4 decimales.
  - `Acciones`: enlaces a **Editar** y botón para **Activar / Desactivar**.

### 2.2 Formularios de Alta y Edición
- **Alta (`Pages/Productos/Crear.cshtml`):**
  - Campos: Código, Nombre, Unidad de Medida, Descripción.
  - Botón "Guardar Producto" y "Cancelar".
- **Edición (`Pages/Productos/Editar.cshtml`):**
  - Código en modo solo lectura (`readonly disabled`), haciendo explícita la inmutabilidad de la clave de negocio (**Decisión D-10**).
  - Permite modificar Nombre, Unidad de Medida, Descripción.
  - Interruptor (`form-switch`) para activar o desactivar el producto.

### 2.3 Manejo y Visualización de Errores de Validación y Duplicados
- Los mensajes provenientes del dominio se presentan en un banner destacado (`alert-danger` con `asp-validation-summary="ModelOnly"`):
  - Código vacío: *"El código del producto es obligatorio."*
  - Código duplicado: *"Ya existe un producto con el código {codigo}"*.
  - Nombre vacío o largo: *"El nombre del producto es obligatorio."* / *"El nombre no puede tener más de 150 caracteres."*
  - Unidad vacía: *"La unidad de medida es obligatoria."*

### 2.4 Activación y Desactivación desde la Interfaz
- El botón de acción en el listado y el interruptor en el formulario de edición permiten alternar el estado del producto mediante peticiones POST seguras contra CSRF.
- La confirmación se notifica mediante mensajes de retroalimentación (`TempData["MensajeExito"]`).

### 2.5 Ninguna Validación Existe Solo en la Interfaz (Defensa en Profundidad)
- Los formularios no dependen de validaciones puramente cosméticas de JavaScript; todas las comprobaciones son ejecutadas por `ValidadorProducto` y `ProductoService` en el backend.

---

## 3. Arquitectura y Código

### Configuración en `Program.cs`
```csharp
builder.Services.AddScoped<ProductoService>();
builder.Services.AddRazorPages();
```

### Enlace de Navegación en `_Layout.cshtml`
```html
<li class="nav-item">
    <a class="nav-link text-dark fw-semibold" asp-area="" asp-page="/Productos/Index">Productos</a>
</li>
```

---

## 4. Resumen de Pruebas Automatizadas

```text
Pruebas funcionales de Razor Pages (SI.Tests.FunctionalTests.ProductosPagesTests):
- 9 de 9 pruebas superadas (100%).
Suite completa de la solución:
- 106 de 106 pruebas superadas (0 errores).
```
