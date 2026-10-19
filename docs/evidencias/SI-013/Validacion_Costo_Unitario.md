# Evidencia Técnica — SI-013: Validación de Costo Unitario

## 1. Resumen del Ticket
- **ID:** `SI-013`
- **Release:** `v0.2.0` (Reglas y motor de costeo)
- **Responsable:** Dev B (Gustavo Lucio)
- **Objetivo:** Impedir entradas con costo unitario negativo, cero o con precisión no soportada antes de persistir o procesar movimientos de inventario (**RB-02**, **RB-06**, **RNF-03**, **D-01**).
- **Trazabilidad:** `RB-02` (Costo > 0), `RB-06` (Rechazo sin efectos), `RNF-03` (Validación con mensajes comprensibles), `D-01` (Máximo 4 decimales), `CP-06`.

---

## 2. Análisis de Particiones de Equivalencia y Valores Límite

| Variable | Tipo de Caso | Valor Probado | Resultado Esperado | Mensaje Esperado / Comportamiento |
|---|---|:---:|:---:|---|
| Costo Unitario | Inválido (Límite negativo) | `-0.01` | **Rechazado** | *"El costo unitario debe ser mayor que cero."* |
| Costo Unitario | Inválido (Límite cero) | `0` | **Rechazado** | *"El costo unitario debe ser mayor que cero."* |
| Costo Unitario | Inválido (Escala > 4 dec.) | `0.00001` | **Rechazado** | *"El costo unitario no puede tener más de 4 decimales."* |
| Costo Unitario | Inválido (Escala > 4 dec.) | `1.00001` | **Rechazado** | *"El costo unitario no puede tener más de 4 decimales."* |
| Costo Unitario | Inválido (Escala > 4 dec.) | `10.12345` | **Rechazado** | *"El costo unitario no puede tener más de 4 decimales."* |
| Costo Unitario | Válido (Frontera inferior) | `0.0001` | **Aceptado** | `EsValido = true`, sin error |
| Costo Unitario | Válido (Dos decimales) | `0.01` | **Aceptado** | `EsValido = true`, sin error |
| Costo Unitario | Válido (Caso E-01 Entrada) | `20.00` | **Aceptado** | `EsValido = true`, sin error |
| Costo Unitario | Válido (Límite 4 decimales) | `100.1234` | **Aceptado** | `EsValido = true`, sin error |

---

## 3. Implementación del Componente de Validación

### `ValidadorCostoUnitario.cs` (`SI.Web/Common/ValidadorCostoUnitario.cs`)
```csharp
namespace SI.Web.Common;

/// <summary>
/// Valida las reglas de dominio para costo unitario en movimientos de entrada (RB-02, D-01).
/// </summary>
public static class ValidadorCostoUnitario
{
    public const string MensajeCostoMenorOIgualACero = "El costo unitario debe ser mayor que cero.";
    public const string MensajePrecisionExcedida = "El costo unitario no puede tener más de 4 decimales.";

    private static readonly ResultadoValidacion ResultadoValido = new(true);

    public static ResultadoValidacion Validar(decimal costoUnitario)
    {
        if (costoUnitario <= 0)
        {
            return new ResultadoValidacion(false, MensajeCostoMenorOIgualACero);
        }

        if (decimal.Round(costoUnitario, 4) != costoUnitario)
        {
            return new ResultadoValidacion(false, MensajePrecisionExcedida);
        }

        return ResultadoValido;
    }
}
```

---

## 4. Suite de Pruebas Unitarias (`SI.Tests/UnitTests/Validadores/ValidadorCostoUnitarioTests.cs`)

```csharp
using SI.Web.Common;

namespace SI.Tests.UnitTests.Validadores;

public class ValidadorCostoUnitarioTests
{
    [Theory]
    [InlineData(-0.01)]
    [InlineData(0)]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RB-02")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCostoUnitario_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(decimal costoInvalido)
    {
        var resultado = ValidadorCostoUnitario.Validar(costoInvalido);

        Assert.False(resultado.EsValido);
        Assert.Equal("El costo unitario debe ser mayor que cero.", resultado.MensajeError);
    }

    [Theory]
    [InlineData(0.00001)]
    [InlineData(1.00001)]
    [InlineData(10.12345)]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RB-02")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(decimal costoInvalido)
    {
        var resultado = ValidadorCostoUnitario.Validar(costoInvalido);

        Assert.False(resultado.EsValido);
        Assert.Equal("El costo unitario no puede tener más de 4 decimales.", resultado.MensajeError);
    }

    [Theory]
    [InlineData(0.0001)]
    [InlineData(0.01)]
    [InlineData(20.00)]
    [InlineData(100.1234)]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RB-02")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCostoUnitario_CuandoEsValido_DebeAceptar(decimal costoValido)
    {
        var resultado = ValidadorCostoUnitario.Validar(costoValido);

        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }
}
```

---

## 5. Ejecución y Resultados

```bash
dotnet test --filter "CP=CP-06" --no-restore
```
- Total de pruebas: 9
- Superadas: 9 (100%)
- Fallidas: 0
- Duración: ~6 ms

---

## 6. Conclusión y Trazabilidad
El validador cumple con **RB-02** y **RNF-03**, asegurando que ninguna adquisición registre un costo negativo, cero o con precisión que supere los 4 decimales permitidos (`decimal(18,4)`), manteniendo la integridad del inventario de acuerdo a la decisión de diseño **D-01**.
