# Evidencia Fase RED — SI-036: Pruebas funcionales de flujo completo (RF-01 a RF-05, E-01, E-02)

## Objetivo de la fase RED
Diseñar y formular la suite de pruebas funcionales de flujo completo `FlujoCompletoFunctionalTests` (`Nivel=Funcional`), garantizando la cobertura de punta a punta de los escenarios `E-01` y `E-02` interactuando a través de los PageModels reales de la aplicación (`CrearModel`, `EntradaModel`, `SalidaModel`, `ConsultaModel`) y auditando la persistencia directa en SQL Server.

## Casos de prueba funcionales formulados

### 1. `FlujoCompleto_EscenarioE01_EjecutaCicloDeVidaCompletoYVerificaOraculoYBaseDeDatos`
- **Trazabilidad:** `CP-03`, `CP-04`, `CP-07`, `CP-08`, `CP-09`, `CP-10`, `CP-11`, `CP-18`, `Nivel=Funcional`.
- **Flujo evaluado:**
  1. Alta de producto `P-001` mediante `CrearModel` y verificación de redirección.
  2. Registro de Entrada 1 (10 @ $20.00) mediante `EntradaModel`.
  3. Registro de Entrada 2 (10 @ $30.00) mediante `EntradaModel`.
  4. Intento de salida inválida por 25 unidades (`CP-07`) verificando rechazo y ausencia de mutaciones en BD.
  5. Registro de Salida 1 (5 unidades) mediante `SalidaModel`.
  6. Consulta y verificación en `ConsultaModel` de valuación PEPS ($400.00), Promedio Ponderado ($375.00) y conservación de existencia física en 15.0000 unidades (`CP-10`).
  7. Auditoría SQL directa en base de datos desacoplada para constatar 1 producto, 3 movimientos y 2 capas PEPS activas.

### 2. `FlujoCompleto_EscenarioE02_EjecutaAgotamientoMulticapaResiduoD03YVerificaBD`
- **Trazabilidad:** `CP-15`, `CP-16`, `Nivel=Funcional`.
- **Flujo evaluado:**
  1. Alta de producto `P-002` mediante `CrearModel`.
  2. Entradas consecutivas (3 @ $10.00 y 4 @ $11.00).
  3. Salida 1 de 5 unidades consumiendo múltiples capas (`CP-15`).
  4. Salida 2 de 2 unidades llevando la existencia física exactamente a cero y saldos monetarios PEPS y Promedio a $0.00 con ajuste por residuo `D-03` (`CP-16`).
  5. Auditoría directa en base de datos.

## Comando ejecutado (Fase RED)
```bash
dotnet test --filter "FullyQualifiedName~FlujoCompletoFunctionalTests"
```

## Salida de pruebas (RED)
```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.29]     SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.FlujoCompleto_EscenarioE01_EjecutaCicloDeVidaCompletoYVerificaOraculoYBaseDeDatos [FAIL]
[xUnit.net 00:00:00.30]     SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.FlujoCompleto_EscenarioE02_EjecutaAgotamientoMulticapaResiduoD03YVerificaBD [FAIL]
  Con error SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.FlujoCompleto_EscenarioE01_EjecutaCicloDeVidaCompletoYVerificaOraculoYBaseDeDatos [2 ms]
  Mensaje de error:
   System.NotImplementedException : Fase RED (SI-036): Orquestación de flujo completo de inicio a fin E-01 pendiente de implementar en la fase GREEN.
  Seguimiento de la pila:
     at SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.EjecutarFlujoE01Async() in SI.Tests/FunctionalTests/FlujoCompletoFunctionalTests.cs:line 97
     at SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.FlujoCompleto_EscenarioE01_EjecutaCicloDeVidaCompletoYVerificaOraculoYBaseDeDatos() in SI.Tests/FunctionalTests/FlujoCompletoFunctionalTests.cs:line 73
  Con error SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.FlujoCompleto_EscenarioE02_EjecutaAgotamientoMulticapaResiduoD03YVerificaBD [< 1 ms]
  Mensaje de error:
   System.NotImplementedException : Fase RED (SI-036): Orquestación de flujo completo de inicio a fin E-02 pendiente de implementar en la fase GREEN.
  Seguimiento de la pila:
     at SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.EjecutarFlujoE02Async() in SI.Tests/FunctionalTests/FlujoCompletoFunctionalTests.cs:line 104
     at SI.Tests.FunctionalTests.FlujoCompletoFunctionalTests.FlujoCompleto_EscenarioE02_EjecutaAgotamientoMulticapaResiduoD03YVerificaBD() in SI.Tests/FunctionalTests/FlujoCompletoFunctionalTests.cs:line 91

Con error! - Con error:     2, Superado:     0, Omitido:     0, Total:     2, Duración: 3 ms - SI.Tests.dll (net8.0)
```
