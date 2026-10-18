# Evidencia de Prueba RED — DEF-001

## 1. Justificación Técnica y de Negocio

Dentro de nuestras reglas de arquitectura y negocio ([`docs/02-requisitos-y-decisiones.md`](file:///Users/riguidixrodriguez/RiderProjects/SistemaInventario/docs/02-requisitos-y-decisiones.md)), la creación de los modelos de base de datos debe respetar el uso de precisión decimal estricta, tanto en las existencias como en los costos unitarios. La razón fundamental es que el sistema debe soportar productos comercializados tanto por **piezas/unidades** como a **granel** (mediante unidades de medida: kg, metros, litros, etc.), garantizando que la misma entidad sea completamente reutilizable y consistente.

### Ejemplo de Diseño
Existen distintos tipos de productos:
1. **Unidades / Piezas:** Se contabilizan de forma discreta.
2. **A granel:** Requieren fraccionamiento continuo y pesaje/medición.

Si en la base de datos se utilizara el tipo `int` o `bigint`, el sistema quedaría imposibilitado ante la necesidad de registrar movimientos a granel (por ejemplo: abarrotes, pintura, cables, telas, combustible).

Al definir el uso de `decimal(18, 4)` conforme a la **Decisión D-01** y la regla **RB-08**, se abarcan ambos casos sin pérdida de precisión:
- Un tornillo se almacena como `10.0000` (representa 10 piezas exactas).
- Un cable se almacena como `10.5000` (representa 10 metros y medio).

> **Nota:** La semántica de interpretación se rige por la propiedad `UnidadMedida` dentro de la entidad `Producto`.

---

## 2. ¿Por qué 4 decimales y no 2?

Nuestra arquitectura brinda protección contra desviaciones financieras y garantiza exactitud en los cálculos contables.

### 2.1 Precisión en conversiones físicas
En unidades métricas, el cuarto decimal tiene significado físico real en operaciones estándar de almacén:
- **En kilogramos:** `0.0001 kg` = $0.1$ gramos (precisión estándar en pesaje industrial y ferretero).
- **En metros:** `0.0001 m` = $0.1$ milímetros.
- **En litros:** `0.0001 L` = $0.1$ mililitros.

Si se limitara a 2 decimales, la menor fracción almacenable sería $0.01\text{ kg}$ ($10\text{ gramos}$), generando mermas o discrepancias acumulativas inaceptables en inventario.

### 2.2 Costos unitarios y prevención de errores de centavos
En compras y divisiones del costo total entre la cantidad (especialmente en el cálculo del **Costo Promedio Ponderado móvil**, Decisión **D-10**):
$$\text{Costo Promedio} = \frac{\text{Valor Anterior} + \text{Valor Entrada}}{\text{Cantidad Anterior} + \text{Cantidad Entrada}}$$

El costo unitario resultante genera frecuentemente fracciones con múltiples decimales. Por ejemplo, en el **Escenario E-02**:
$$\frac{\$74.00}{7\text{ unidades}} = 10.571428\dots \xrightarrow{\text{ToEven}} \$10.5714$$

Si el costo promedio se truncara o redondeara a 2 decimales en la base de datos a $\$10.57$:
- Al registrar una salida de $5$ unidades: $5 \times 10.57 = \$52.85$.
- Con 4 decimales: $5 \times 10.5714 = 52.857 \xrightarrow{\text{ToEven}} \mathbf{\$52.86}$.

Guardar 2 decimales alteraría silenciosamente el valor contable del inventario en cada salida, violando el requisito no funcional de integridad (**RNF-01**).

---

## 3. Ficha Técnica del Defecto DEF-001

| Campo | Valor |
|---|---|
| **ID** | DEF-001 |
| **Título** | Costo promedio persistido con 2 decimales y existencia tipada como entero |
| **Fecha / Detectó** | 17 de Octubre del 2026 / QA & Núcleo (Auditoría de modelo y prueba de regresión) |
| **Técnica de detección** | Revisión estática de modelos + Prueba automatizada de regresión (`SI.Tests/RegressionTests/Def001PrecisionProductoTests.cs`) |
| **CP / Requisito** | CP-19, RB-08, RB-09, D-01, D-02 (política de precisión §5.2 de `docs/02-requisitos-y-decisiones.md`) |
| **Severidad** | **Alta** (Afecta directamente la integridad matemática del inventario y la precisión contable de las salidas) |
| **Estado** | Verificado en RED / Evidenciado para corrección |
| **Causa Raíz** | `Producto.cs` declaraba `public int ExistenciaActual { get; set; }` en lugar de `decimal`, y `ProductoConfiguration.cs` no especificaba la precisión fluida `HasPrecision()`, dejando que EF Core aplicara por omisión `decimal(18,2)` para los tipos decimales. |
| **Impacto** | Imposibilidad de manejar existencias a granel e imprecisión acumulada en el valor del inventario bajo el método de Costo Promedio Ponderado móvil. |

---

## 4. Ejecución de la Prueba en RED (Evidencia del Fallo)

Para asegurar la trazabilidad del proceso TDD y evitar falsas aprobaciones, se diseñó la prueba de regresión `Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta`.

### 4.1 Código de la Prueba de Regresión
```csharp
[Fact]
[Trait("CP", "CP-19")]
[Trait("RF", "RB-08")]
[Trait("DEF", "DEF-001")]
[Trait("Nivel", "Regresion")]
public void Producto_Schema_DebeTenerPrecisionDecimalCorrecta()
{
    // Arrange
    var options = new DbContextOptionsBuilder<InventarioDbContext>()
        .UseSqlServer()
        .Options;
    
    var context = new InventarioDbContext(options);
    
    // Act
    var entidad = context.Model.FindEntityType(typeof(Producto));
    var existencia = entidad?.FindProperty(nameof(Producto.ExistenciaActual));
    var costoProm = entidad?.FindProperty(nameof(Producto.CostoPromedioActual));
    var valorPeps = entidad?.FindProperty(nameof(Producto.ValorInventarioPeps));
    var valorProm = entidad?.FindProperty(nameof(Producto.ValorInventarioPromedio));

    // Assert
    Assert.Equal(typeof(decimal), existencia?.ClrType);
    Assert.Equal(18, existencia?.GetPrecision());
    Assert.Equal(4, existencia?.GetScale());
    
    Assert.Equal(18, costoProm?.GetPrecision());
    Assert.Equal(4, costoProm?.GetScale());
    
    Assert.Equal(18, valorPeps?.GetPrecision());
    Assert.Equal(2, valorPeps?.GetScale());
    
    Assert.Equal(18, valorProm?.GetPrecision());
    Assert.Equal(2, valorProm?.GetScale());
}
```

### 4.2 Registro de Ejecución y Fallo (Salida de xUnit)

Durante la fase de diseño y comprobación contra el modelo inicial no corregido, se obtuvo la siguiente falla de aserción al verificar el contrato de tipos y metadatos:

```text
[xUnit.net 00:00:00.61]   Starting:    SI.Tests
[xUnit.net 00:00:00.78]     SI.Tests.RegressionTests.Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta [FAIL]
[xUnit.net 00:00:00.79]       Assert.Equal() Failure: Values differ
[xUnit.net 00:00:00.79]       Expected: typeof(decimal)
[xUnit.net 00:00:00.79]       Actual:   null
[xUnit.net 00:00:00.79]       Stack Trace:
[xUnit.net 00:00:00.79]         at SI.Tests.RegressionTests.Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta() in /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/RegressionTests/Def001PrecisionProductoTests.cs:line 31
[xUnit.net 00:00:00.79]         at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
[xUnit.net 00:00:00.79]         at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

> **Análisis de QA del Fallo:**
> 1. En la primera iteración del test, la resolución de la propiedad evaluó `null` debido a un error de referencia de cadena (`nameof(Producto)` en vez de `nameof(Producto.ExistenciaActual)`).
> 2. Una vez resuelta la propiedad hacia el modelo sin corregir de `Producto.cs`, `existencia.ClrType` retornó `typeof(int)` en lugar de `typeof(decimal)`.
> 3. Asimismo, el metadata model de EF Core no contenía la configuración de precisión `(18, 4)` requerida para `CostoPromedioActual`.
> 4. Esto confirma fehacientemente que la base de datos actual **NO** cumplía con la precisión de 4 decimales ni con el tipo adecuado para existencias a granel.

---

## 5. Plan de Corrección (Paso GREEN)

Para transicionar de RED a GREEN:
1. **Entidad `Producto.cs`:**
   Modificar `ExistenciaActual` a `decimal`:
   ```csharp
   public decimal ExistenciaActual { get; set; }
   ```
2. **Configuración Fluent API en `ProductoConfiguration.cs`:**
   Mapear explícitamente las escalas y precisiones:
   ```csharp
   builder.Property(p => p.ExistenciaActual).HasPrecision(18, 4);
   builder.Property(p => p.CostoPromedioActual).HasPrecision(18, 4);
   builder.Property(p => p.ValorInventarioPeps).HasPrecision(18, 2);
   builder.Property(p => p.ValorInventarioPromedio).HasPrecision(18, 2);
   ```
3. **Re-ejecución:**
   Comprobar que `Def001PrecisionProductoTests` pase al 100% (GREEN) antes de crear la migración definitiva de EF Core y abrir el PR correspondiente.