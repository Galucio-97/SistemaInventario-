# Evidencia Fase GREEN — SI-036: Pruebas funcionales de flujo completo (RF-01 a RF-05, E-01, E-02)

## 1. Comando ejecutado (Filtro Funcional con TRX)
```bash
dotnet test --filter "Nivel=Funcional" --logger "trx;LogFileName=ciclo1-funcional.trx"
```

## 2. Salida de pruebas funcionales
```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
Archivo de resultados: /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/TestResults/ciclo1-funcional.trx

Correctas! - Con error:     0, Superado:    28, Omitido:     0, Total:    28, Duración: 1 s - SI.Tests.dll (net8.0)
```

## 3. Comando ejecutado (Suite Completa)
```bash
dotnet test
```

## 4. Salida Suite Completa
```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   171, Omitido:     0, Total:   171, Duración: 3 s - SI.Tests.dll (net8.0)
```

---

## 5. Verificación de Flujos de Usuario de Punta a Punta

### 5.1 Escenario E-01 (`FlujoCompleto_EscenarioE01_EjecutaCicloDeVidaCompletoYVerificaOraculoYBaseDeDatos`)
1. **Alta de Producto (`RF-01`):** Se registró `P-001` (*Tornillo 1/4*, pieza) con éxito.
2. **Entrada 1 (`RF-02`, `CP-03`):** 10 @ $20.00 $\rightarrow$ Existencia 10, Saldos $200.00 / $200.00.
3. **Entrada 2 (`RF-02`):** 10 @ $30.00 $\rightarrow$ Existencia 20, Saldos $500.00 / $500.00.
4. **Defensa en profundidad y rechazos (`CP-07`):** Intento de salida de 25 u. rechazado con mensaje *"Existencia insuficiente..."* sin alterar base de datos.
5. **Salida 1 (`RF-03`, `CP-04`):** 5 u. $\rightarrow$ Existencia 15, Saldo PEPS $400.00 (primera capa a $20), Saldo Promedio $375.00 ($25/u.).
6. **Consulta dual e Historial Kardex (`RF-04`, `RF-05`, `CP-08`, `CP-09`, `CP-10`):**
   - PEPS: Existencia 15.0000, Valor $400.00, Costo unitario vigente $26.6667, 2 capas activas (5 @ $20.00 y 10 @ $30.00).
   - Promedio: Existencia 15.0000 (invarianza física), Valor $375.00, Costo unitario $25.0000.
   - Kardex: 3 movimientos coincidentes fila por fila con el oráculo.
7. **Auditoría directa en SQL Server (Contexto desacoplado):**
   - `Productos`: 1 registro persistido con existencia 15.0000, PEPS $400.00, Promedio $375.00.
   - `Movimientos`: 3 registros confirmados.
   - `CapasInventarioPeps`: 2 capas activas con saldo remanente exacto.

### 5.2 Escenario E-02 (`FlujoCompleto_EscenarioE02_EjecutaAgotamientoMulticapaResiduoD03YVerificaBD`)
1. **Alta y Entradas:** `P-002`, E1 (3 @ $10.00) y E2 (4 @ $11.00).
2. **Salida Multicapa (`CP-15`):** Salida de 5 u. consumiendo 3 de la Capa 1 y 2 de la Capa 2 (Costo PEPS $52.00, Saldo remanente $22.00).
3. **Agotamiento Total y Ajuste D-03 (`CP-16`, `CP-17`):** Salida de 2 u. llevando la existencia física exactamente a 0.0000 y ambos métodos a saldo total $0.00.
4. **Auditoría en SQL Server:** 0 capas activas remanentes, 4 movimientos confirmados y producto con stock y saldos en cero.
