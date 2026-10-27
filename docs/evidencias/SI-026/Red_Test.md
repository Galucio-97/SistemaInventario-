# Evidencia Fase RED — SI-026: Atomicidad ante fallos (RNF-04, RB-06, CP-13)

## Objetivo de la fase RED
Formular las pruebas de integración que demuestran que un fallo de infraestructura simulado en el momento exacto previo a la confirmación de la transacción (`TransactionCommittingAsync`) provoca un rollback atómico completo en SQL Server, impidiendo que cambios parciales queden persistidos en la base de datos (RNF-04, RB-06).

## Casos de prueba diseñados
1. **`SalidaConFalloEnCommit_HaceRollbackCompletoYNoDejaActualizacionesParciales` (CP-13)**:
   - **Precondición**: Estado E1 + E2 de E-01 (Producto con 20 unidades, 500.00 promedio, 500.00 PEPS, 2 capas de 10 unidades c/u y 2 movimientos).
   - **Acción**: Ejecución de salida S1 de 5 unidades bajo un contexto con `SimuladorFalloTransaccionInterceptor` activo, lanzando `InvalidOperationException` al invocar `TransactionCommittingAsync`.
   - **Aserción esperada**:
     - Lanzamiento controlado de la excepción del interceptor.
     - En un `DbContext` nuevo e independiente, verificar que el producto mantiene 20 unidades, $500.00 en ambos métodos, las dos capas PEPS intactas (10 y 10 disponibles) y exactamente 2 movimientos.
2. **`EntradaConFalloEnCommit_HaceRollbackCompletoYNoDejaActualizacionesParciales`**:
   - **Precondición**: Producto nuevo con existencia 0 y saldos 0.00.
   - **Acción**: Intento de entrada de 10 unidades @ $20.00 bajo contexto con interceptor de fallo.
   - **Aserción esperada**:
     - Lanzamiento controlado de la excepción.
     - En un `DbContext` nuevo e independiente, verificar que el producto permanece en existencia 0, saldos en 0.00, cero movimientos y cero capas PEPS creadas.

## Compilación y enlace
- Se implementó `SimuladorFalloTransaccionInterceptor` derivado de `DbTransactionInterceptor` en `SI.Tests/Fixtures/SimuladorFalloTransaccionInterceptor.cs`.
- Se extendió `TestDbContextFixture.CreateDbContext(params IInterceptor[] interceptors)` para inyectar interceptores en pruebas sin alterar el código ni las firmas públicas de producción (`IInventarioService`).
- Se crearon las pruebas en `SI.Tests/IntegrationTests/TransaccionRollbackTests.cs`.
