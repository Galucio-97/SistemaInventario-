# Evidencia Técnica — SI-012: Validación de Cantidades

## 1. Resumen del Ticket
- **ID:** `SI-012`
- **Release:** `v0.2.0` (Reglas y motor de costeo)
- **Responsable:** Dev B (Gustavo Lucio)
- **Objetivo:** Rechazar cantidades inválidas con mensajes claros y comprensibles en español antes de persistir o procesar movimientos de inventario (**RB-01**, **RNF-03**).
- **Trazabilidad:** `RB-01` (Cantidad > 0), `RNF-03` (Validación con mensajes comprensibles), `D-01` (Máximo 4 decimales), `CP-05`.

---

## 2. Análisis de Particiones de Equivalencia y Valores Límite

| Variable | Tipo de Caso | Valor Probado | Resultado Esperado | Mensaje Esperado / Comportamiento |
|---|---|:---:|:---:|---|
| Cantidad | Inválido (Límite negativo) | `-1` | **Rechazado** | *"La cantidad debe ser mayor que cero."* |
| Cantidad | Inválido (Límite cero) | `0` | **Rechazado** | *"La cantidad debe ser mayor que cero."* |
| Cantidad | Inválido (Escala > 4 dec.) | `0.00001` | **Rechazado** | *"La cantidad no puede tener más de 4 decimales."* |
| Cantidad | Inválido (Escala > 4 dec.) | `1.00001` | **Rechazado** | *"La cantidad no puede tener más de 4 decimales."* |
| Cantidad | Inválido (Escala > 4 dec.) | `10.12345` | **Rechazado** | *"La cantidad no puede tener más de 4 decimales."* |
| Cantidad | Válido (Frontera inferior) | `0.0001` | **Aceptado** | `EsValido = true`, sin error |
| Cantidad | Válido (Unidad entera) | `1` | **Aceptado** | `EsValido = true`, sin error |
| Cantidad | Válido (Un decimal) | `10.5` | **Aceptado** | `EsValido = true`, sin error |
| Cantidad | Válido (Límite 4 decimales) | `100.1234` | **Aceptado** | `EsValido = true`, sin error |

---

## 3. Implementación del Componente de Validación

### 3.1 Record `ResultadoValidacion.cs` (`SI.Web/Common/ResultadoValidacion.cs`)
```csharp
namespace SI.Web.Common;

public record ResultadoValidacion(bool EsValido, string? MensajeError = null);
```

### 3.2 Lógica en `ValidadorCantidad.cs` (`SI.Web/Common/ValidadorCantidad.cs`)
```csharp
namespace SI.Web.Common;

public static class ValidadorCantidad
{
    public static ResultadoValidacion Validar(decimal cantidad)
    {
        if (cantidad <= 0)
        {
            return new ResultadoValidacion(false, "La cantidad debe ser mayor que cero.");
        }

        if (decimal.Round(cantidad, 4) != cantidad)
        {
            return new ResultadoValidacion(false, "La cantidad no puede tener más de 4 decimales.");
        }

        return new ResultadoValidacion(true);
    }
}
```

---

## 4. Evidencia de Ejecución de Pruebas Automatizadas (xUnit)

Comando ejecutado:
```bash
dotnet test --filter "FullyQualifiedName~ValidadorCantidadTests"
```

Salida de la consola:
```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
VSTest 17.11.1 (arm64)

- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(cantidadInvalida: -1) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(cantidadInvalida: 0) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 0.00001) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 1.00001) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 10.12345) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 0.0001) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 1) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 10.5) [PASS]
- SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 100.1234) [PASS]

Total: 9. Aprobadas: 9. Fallidas: 0. Omitidas: 0. Duración: 2 ms.
```

Ejecución de la suite completa (`dotnet test`):
```text
Total: 18. Aprobadas: 18. Fallidas: 0. Omitidas: 0. Duración: 997 ms.
```

---

## 5. Certificación QA
- [x] **RB-01 verificado:** Rechazo comprobado para `-1` y `0`.
- [x] **RNF-03 cumplido:** Mensajes descriptivos en español devueltos en el contrato de validación.
- [x] **D-01 validado:** Rechazo comprobado para cantidades con más de 4 decimales (`0.00001`).
- [x] **Fronteras válidas aprobadas:** `0.0001` y `1` aceptados sin error.
