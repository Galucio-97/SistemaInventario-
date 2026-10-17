# Evidencia de Prueba GREEN — DEF-001

## 1. Resumen de la Transición (TDD: RED → GREEN)

- **Defecto Asociado:** `DEF-001` — *Costo promedio persistido con 2 decimales y existencia tipada como entero*.
- **Ticket Backlog:** `SI-006` — *Corregir precisión del modelo de producto (DEF-001)*.
- **Requisitos Satisfechos:** **RB-08** (Precisión decimal), **RB-09** (Redondeo documentado y uniforme), **D-01** (Cantidades `decimal(18,4)`), **RNF-01** (Integridad).
- **Resultado:** Tras aplicar la configuración fluida de EF Core y modificar el tipo de datos en la entidad, la prueba de regresión pasó exitosamente (**GREEN**), y se generó la migración que materializa el esquema en la base de datos SQL Server.

---

## 2. Cambios Implementados

### 2.1 Entidad `Producto.cs`
Se corrigió la propiedad `ExistenciaActual` pasando de `int` a `decimal`:

```csharp
public decimal ExistenciaActual { get; set; }
public decimal CostoPromedioActual { get; set; }
public decimal ValorInventarioPeps { get; set; }
public decimal ValorInventarioPromedio { get; set; }
```

### 2.2 Configuración Fluida en `ProductoConfiguration.cs`
Se especificaron las precisiones numéricas estrictas exigidas por la arquitectura:

```csharp
// Precisiones numéricas estrictas (Corrección DEF-001 y RB-08)
builder.Property(p => p.ExistenciaActual)
    .HasPrecision(18, 4)
    .HasDefaultValue(0m);

builder.Property(p => p.CostoPromedioActual)
    .HasPrecision(18, 4)
    .HasDefaultValue(0m);

builder.Property(p => p.ValorInventarioPeps)
    .HasPrecision(18, 2)
    .HasDefaultValue(0m);

builder.Property(p => p.ValorInventarioPromedio)
    .HasPrecision(18, 2)
    .HasDefaultValue(0m);
```

### 2.3 Migración Generada (`20261017224554_AgregarProductos.cs`)
La migración de EF Core generada confirma la asignación exacta de tipos y precisiones a nivel DDL:

```csharp
ExistenciaActual = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
CostoPromedioActual = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
ValorInventarioPeps = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
ValorInventarioPromedio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m)
```

---

## 3. Evidencia de Ejecución en GREEN (Salida xUnit)

La prueba de regresión `SI.Tests.RegressionTests.Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta` valida en tiempo de prueba que el modelo de metadata de EF Core expone exactamente las precisiones y tipos configurados.

```text
Iniciando la ejecución de pruebas...
Información de ejecución de pruebas:
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.0
[xUnit.net 00:00:00.32]   Starting:    SI.Tests
[xUnit.net 00:00:00.48]   Finished:    SI.Tests
  Aprobado! - Con error: 0, Aprobado: 1, Omitido: 0, Total: 1, Duración: 162 ms
  - SI.Tests.RegressionTests.Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta [PASS]
```

### Tabla Comparativa de Verificación de Esquema

| Propiedad | Tipo Esperado | Escala Esperada | Tipo Obtenido | Escala Obtenida | Estado |
|---|---|---|---|---|---|
| `ExistenciaActual` | `System.Decimal` | `4` (`decimal(18,4)`) | `System.Decimal` | `4` | **PASS** |
| `CostoPromedioActual` | `System.Decimal` | `4` (`decimal(18,4)`) | `System.Decimal` | `4` | **PASS** |
| `ValorInventarioPeps` | `System.Decimal` | `2` (`decimal(18,2)`) | `System.Decimal` | `2` | **PASS** |
| `ValorInventarioPromedio` | `System.Decimal` | `2` (`decimal(18,2)`) | `System.Decimal` | `2` | **PASS** |

---

## 4. Certificación de QA

- [x] **No-regresión:** La prueba de regresión `Def001PrecisionProductoTests` queda incorporada en la suite permanente en `SI.Tests/RegressionTests/`.
- [x] **Consistencia de modelo:** La base de datos y las entidades quedan preparadas para el soporte de productos a granel y el cálculo sin pérdida de centavos del promedio ponderado móvil (**Escenario E-02**).
- [x] **Cierre de Ciclo:** Se autoriza la actualización de estado a **Corregido / Verificado** en el registro de defectos y cierre del ticket en el backlog.
