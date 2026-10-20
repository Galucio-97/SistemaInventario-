# 03 · Diseño de Casos de Prueba

> Los **resultados esperados** de este documento se calcularon **a mano, antes de programar**, aplicando la política de `02-requisitos-y-decisiones.md §5` (redondeo bancario). El campo *Resultado obtenido* se llena **solo al ejecutar**.

## 1. Técnicas de diseño aplicadas (justificación)

| Técnica | Dónde se aplica | Por qué es adecuada |
|---|---|---|
| **Partición de equivalencia** | Cantidad (RB-01), costo unitario (RB-02), código de producto (RF-01) | Los dominios de entrada se dividen en clases con un comportamiento uniforme: un representante de cada clase basta para cubrirla. |
| **Análisis de valores límite** | Cantidad, costo unitario y cantidad de salida vs existencia (RB-01, RB-02, RB-03) | Los errores típicos están en las fronteras (`<` vs `<=`). Se prueban el límite y sus vecinos inmediatos. |
| **Tabla de decisión** | Registro de salida (RF-03, RB-03, RB-04, RB-10) | Varias condiciones combinadas determinan una única acción válida; la tabla garantiza cubrir todas las combinaciones relevantes. |
| **Pruebas basadas en escenarios** | Métodos de costeo (RF-04), persistencia y transacciones (RNF-02, RNF-04) | El costeo depende de la **secuencia** de movimientos; solo un escenario completo verifica capas, promedios y saldos. |

### 1.1 Particiones de equivalencia

| Variable | Clase válida | Clases inválidas |
|---|---|---|
| Cantidad (entrada/salida) | `0 < q`, máx. 4 decimales | `q < 0` · `q = 0` · más de 4 decimales |
| Costo unitario (entrada) | `0 < c`, máx. 4 decimales | `c < 0` · `c = 0` · más de 4 decimales |
| Cantidad de salida vs existencia `E` | `q ≤ E` | `q > E` |
| Código de producto | No vacío, ≤ 50 caracteres, no registrado | Vacío · > 50 caracteres · duplicado |
| Producto del movimiento | Existe y activo | No existe · inactivo |

### 1.2 Valores límite

| Variable | Valores a probar | Esperado |
|---|---|---|
| Cantidad | `-1` · `0` · `0.0001` · `1` | ✗ · ✗ · ✓ · ✓ |
| Cantidad (escala) | `0.00001` | ✗ (más de 4 decimales) |
| Costo unitario | `-0.01` · `0` · `0.0001` · `0.01` | ✗ · ✗ · ✓ · ✓ |
| Salida con `E = 15` | `14.9999` · `15` · `15.0001` | ✓ · ✓ (saldo 0) · ✗ |

### 1.3 Tabla de decisión — registro de salida

| Condición | R1 | R2 | R3 | R4 | R5 |
|---|---|---|---|---|---|
| Producto existe | N | S | S | S | S |
| Producto activo | – | N | S | S | S |
| Cantidad > 0 | – | – | N | S | S |
| Cantidad ≤ existencia | – | – | – | N | S |
| **Acción** | Rechazar | Rechazar | Rechazar | Rechazar | **Registrar** |
| **Caso** | CP-11 | CP-18 | CP-05 | CP-07 | CP-04 |

## 2. Oráculos de costeo (escenarios)

### E-01 · Ejercicio de referencia obligatorio (Sección 6.3) — CP-08, CP-09, CP-10

> ✅ **Confirmado y validado al 100% contra la Sección 6.3 del enunciado oficial.** Coincidencia exacta en todas las cifras y conceptos.

| Mov. | Cant. | C.U. | PEPS costo | PEPS saldo | Prom. C.U. | Prom. costo | Prom. saldo | Exist. |
|---|---|---|---|---|---|---|---|---|
| E1 | 10 | 20.00 | 200.00 | 200.00 | 20.0000 | 200.00 | 200.00 | 10 |
| E2 | 10 | 30.00 | 300.00 | 500.00 | 25.0000 | 300.00 | 500.00 | 20 |
| S1 | 5 | — | **100.00** | **400.00** | 25.0000 | **125.00** | **375.00** | 15 |

Capas PEPS finales: `[5 @ 20.00, 10 @ 30.00]`.

### E-02 · Salida multicapa y promedio periódico (CP-15)

| Mov. | Cant. | C.U. | PEPS costo | PEPS saldo | Prom. C.U. | Prom. costo | Prom. saldo | Exist. |
|---|---|---|---|---|---|---|---|---|
| E1 | 3 | 10.00 | 30.00 | 30.00 | 10.0000 | 30.00 | 30.00 | 3 |
| E2 | 4 | 11.00 | 44.00 | 74.00 | 10.5714 ¹ | 44.00 | 74.00 | 7 |
| S1 | 5 | — | **52.00** ² | 22.00 | 10.5714 | **52.86** ³ | 21.14 | 2 |
| S2 | 2 | — | 22.00 | **0.00** | 10.5714 | 21.14 ⁴ | **0.00** | 0 |

¹ 74 / 7 = 10.571428… → 10.5714 · ² 3×10.00 + 2×11.00 · ³ 5×10.5714 = 52.857 → 52.86 · ⁴ 2×10.5714 = 21.1428 → 21.14 = saldo restante (D-03 no altera el valor)

### E-03 · Redondeo bancario y residuo (CP-17)

| Mov. | Cant. | C.U. | PEPS costo | PEPS saldo | Prom. C.U. | Prom. costo | Prom. saldo | Exist. |
|---|---|---|---|---|---|---|---|---|
| E1 | 3 | 10.00 | 30.00 | 30.00 | 10.0000 | 30.00 | 30.00 | 3 |
| E2 | 3 | 10.01 | 30.03 | 60.03 | 10.0050 ¹ | 30.03 | 60.03 | 6 |
| S1 | 1 | — | 10.00 | 50.03 | 10.0050 | **10.00** ² | 50.03 | 5 |
| S2 | 5 | — | 50.03 | 0.00 | 10.0050 | **50.03** ³ | **0.00** | 0 |

¹ 60.03 / 6 = 10.005 exacto · ² 1×10.005 = 10.005 → bancario → **10.00** (half-up daría 10.01) · ³ 5×10.005 = 50.025 → bancario → 50.02; como la salida agota existencia, **D-03** ajusta el costo al saldo restante: **50.03**.

> Sin D-03 el inventario quedaría con **0 unidades y 0.01 de valor** → violación de RNF-01. Este escenario demuestra que la política está documentada y se aplica uniformemente (RB-09).

## 3. Fichas de casos de prueba

**Datos base:** producto `P-001` "Tornillo 1/4", unidad `pieza`, activo, salvo que se indique otra cosa.

---

### CP-01 — Registrar un producto válido
| Campo | Valor |
|---|---|
| Requisito | RF-01, RNF-02 |
| Técnica / Nivel | Partición (clase válida) / Integración |
| Precondiciones | No existe producto con código `P-001` |
| Datos | Código `P-001`, Nombre `Tornillo 1/4`, Descripción vacía, Unidad `pieza` |
| Pasos | 1. Registrar el producto. 2. Consultar la tabla `Productos` con un `DbContext` nuevo |
| Esperado | 1 fila con los datos exactos, `Activo = 1`, existencia `0`, valores `0.00` |
| Verificación BD | `SELECT * FROM Productos WHERE Codigo = 'P-001'` |
| Obtenido / Estado / Evidencia | Persistencia y lectura íntegra en SQL Server real verificada mediante prueba de humo con contextos desacoplados · **Aprobado (Humo)** · 18-oct-2026 · Dev A · `evidencias/SI-010/BD_Pruebas_Aisladas.md` |

### CP-02 — Rechazar producto con clave duplicada
| Campo | Valor |
|---|---|
| Requisito | RF-01, RNF-03 |
| Técnica / Nivel | Partición (clase inválida: duplicado) / Integración |
| Precondiciones | Existe `P-001` |
| Datos | Código `P-001`, Nombre `Otro` |
| Pasos | 1. Intentar registrar vía servicio. 2. Intentar insertar directo vía `DbContext` (valida índice único) 3. Contar filas |
| Esperado | Mensaje "Ya existe un producto con el código P-001"; el índice único lanza excepción; sigue habiendo **1** fila con ese código |
| Verificación BD | `SELECT COUNT(*) FROM Productos WHERE Codigo = 'P-001'` → 1 |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-03 — Registrar una entrada válida
| Campo | Valor |
|---|---|
| Requisito | RF-02, RB-04, RB-05 |
| Técnica / Nivel | Escenario / Funcional |
| Precondiciones | `P-001` activo sin movimientos |
| Datos | Fecha 2026-10-01, cantidad 10, costo 20.00 |
| Pasos | 1. Registrar entrada. 2. Consultar producto, movimientos y capas |
| Esperado | Existencia 10 · valor PEPS 200.00 · valor Prom 200.00 · promedio 20.0000 · 1 movimiento ENTRADA · 1 capa `10 @ 20.00` disponible 10 |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-04 — Registrar una salida válida
| Campo | Valor |
|---|---|
| Requisito | RF-03, RB-07 |
| Técnica / Nivel | Tabla de decisión R5 / Funcional |
| Precondiciones | E1 y E2 de E-01 registradas |
| Datos | Fecha 2026-10-03, cantidad 5 |
| Pasos | 1. Registrar salida. 2. Consultar producto, movimiento y capas |
| Esperado | Existencia 15 · costo PEPS 100.00 · costo Prom 125.00 · capa 1 disponible 5 · capa 2 disponible 10 |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-05 — Rechazar cantidad cero o negativa
| Campo | Valor |
|---|---|
| Requisito | RB-01, RB-06, RNF-03 |
| Técnica / Nivel | Valores límite / Unitaria + Integración |
| Precondiciones | `P-001` con existencia 10 (E1 de E-01) |
| Datos | Cantidades `-1`, `0`, `0.00001` en entrada y salida; control válido `0.0001` |
| Pasos | 1. Intentar cada operación. 2. Comparar estado de BD antes/después |
| Esperado | Rechazo con mensaje "La cantidad debe ser mayor que cero" (o "máximo 4 decimales"); conteo de movimientos, existencia y valores **idénticos** al estado previo. `0.0001` se acepta en la prueba unitaria del validador |
| Obtenido / Estado / Evidencia | Rechazo correcto con mensajes en español esperados para valores <= 0 y > 4 decimales; aceptación para valores válidos (0.0001, 1, 10.5, 100.1234) · **Aprobado (Unitaria)** · 18-oct-2026 · Dev B · `evidencias/SI-012/Validacion_Cantidades.md` |

### CP-06 — Rechazar costo de entrada negativo o cero
| Campo | Valor |
|---|---|
| Requisito | RB-02, RB-06, RNF-03 |
| Técnica / Nivel | Valores límite / Unitaria |
| Datos | Costos `-0.01`, `0`; controles válidos `0.0001`, `0.01` |
| Esperado | Rechazo con mensaje "El costo unitario debe ser mayor que cero"; sin cambios en BD |
| Obtenido / Estado / Evidencia | Rechazo correcto con mensajes en español esperados para valores <= 0 y > 4 decimales; aceptación para valores válidos (0.0001, 0.01, 20.00, 100.1234) · **Aprobado (Unitaria)** · 18-oct-2026 · Dev B · `evidencias/SI-013/Validacion_Costo_Unitario.md` |

### CP-07 — Rechazar salida mayor a la existencia
| Campo | Valor |
|---|---|
| Requisito | RB-03, RB-06, RNF-04 |
| Técnica / Nivel | Valores límite + Tabla R4 / Funcional |
| Precondiciones | Estado final de E-01 (existencia 15) |
| Datos | Cantidad `15.0001` y `16` |
| Esperado | Rechazo "Existencia insuficiente: disponible 15"; existencia 15, valores 400.00 / 375.00, capas y número de movimientos sin cambios |
| Obtenido / Estado / Evidencia | Rechazo validado con mensaje "Existencia insuficiente: disponible 15." ante 15.0001 y 20; aceptación en 14.9999 y 15.0000 · **Aprobado (Unitaria)** · 18-oct-2026 · Dev B · `evidencias/SI-014/Green_Test.md` |

### CP-08 — Costeo de salida por PEPS (ejercicio de referencia)
| Campo | Valor |
|---|---|
| Requisito | RF-04, RB-07 |
| Técnica / Nivel | Escenario E-01 / Unitaria + Funcional |
| Esperado | Costo salida **100.00** · valor restante **400.00** · capas `[5 @ 20, 10 @ 30]` |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-09 — Costeo de salida por Promedio (ejercicio de referencia)
| Campo | Valor |
|---|---|
| Requisito | RF-04, RB-07 |
| Técnica / Nivel | Escenario E-01 / Unitaria + Funcional |
| Esperado | Promedio vigente **25.0000** · costo salida **125.00** · valor restante **375.00** · promedio tras salida 25.0000 |
| Obtenido / Estado / Evidencia | Entradas E1 (promedio 20.0000, saldo 200.00) y E2 (promedio 25.0000, saldo 500.00) calculadas correctamente con redondeo bancario; validado con E-02 (10.5714), E-03 (10.0050) y caso borde cero · **Aprobado parcial (Unitaria - Entradas SI-015)** · 18-oct-2026 · Dev B · `evidencias/SI-015/Promedio_Ponderado_Entradas.md` |

### CP-10 — Cambiar de método de costeo
| Campo | Valor |
|---|---|
| Requisito | RF-04, RF-05 |
| Técnica / Nivel | Escenario E-01 / Funcional |
| Pasos | 1. Consultar inventario con PEPS. 2. Consultar con Promedio. 3. Comparar con el estado de BD previo a las consultas |
| Esperado | Existencia 15 en ambas; valor 400.00 (PEPS) vs 375.00 (Prom); C.U. de la salida 20.0000 vs 25.0000; **número de movimientos y sus fechas/cantidades/costos de entrada sin cambios** |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-11 — Consultar un producto inexistente
| Campo | Valor |
|---|---|
| Requisito | RF-01, RF-05, RB-04 |
| Técnica / Nivel | Tabla R1 / Funcional |
| Datos | Id `99999` / código `NO-EXISTE`; además intentar entrada sobre él |
| Esperado | Mensaje "El producto no existe"; `COUNT(*)` de movimientos sin cambios |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-12 — Persistencia tras reinicio
| Campo | Valor |
|---|---|
| Requisito | RNF-02, RB-05 |
| Técnica / Nivel | Escenario / Integración + manual |
| Pasos | Auto: registrar E-01, desechar el `DbContext`, abrir uno nuevo, consultar. Manual: detener la app (`Ctrl+C`), volver a ejecutar `dotnet run`, consultar en la UI |
| Esperado | Producto, 3 movimientos, capas y valores idénticos a E-01 |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-13 — Fallo durante una operación sin actualizaciones parciales
| Campo | Valor |
|---|---|
| Requisito | RNF-04, RB-06 |
| Técnica / Nivel | Escenario / Integración |
| Precondiciones | Estado E1+E2 de E-01 |
| Pasos | Registrar la salida S1 con un punto de fallo inyectado **después** de actualizar capas/movimiento y **antes** del commit |
| Esperado | Excepción controlada; existencia 20, valores 500.00/500.00, capas `[10 @ 20, 10 @ 30]` disponibles completas, 2 movimientos |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-14 — Regresión tras corrección de defectos
| Campo | Valor |
|---|---|
| Requisito | Todos |
| Nivel | Regresión |
| Pasos | Por cada defecto corregido: 1. agregar prueba que lo reproduzca en `Regresion/`. 2. ejecutar `dotnet test` completo. 3. re-ejecutar manualmente los CP de UI afectados |
| Esperado | Suite completa en verde; los CP aprobados en Ciclo 1 siguen aprobados |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-15 — Salida que abarca varias capas PEPS
| Campo | Valor |
|---|---|
| Requisito | RF-04 |
| Técnica / Nivel | Escenario E-02 / Unitaria |
| Esperado | S1 = **52.00** (capa 1 agotada, capa 2 con 2 disponibles) · Prom S1 = **52.86** · saldo final 0.00 en ambos |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-16 — Salida igual a la existencia exacta
| Campo | Valor |
|---|---|
| Requisito | RB-03 |
| Técnica / Nivel | Valores límite / Funcional |
| Precondiciones | Estado final de E-01 (existencia 15) |
| Datos | Cantidad 15 |
| Esperado | Aceptada; existencia 0; valor 0.00 en ambos métodos; costo PEPS 400.00; costo Prom 375.00 |
| Obtenido / Estado / Evidencia | Aceptación validada para salida exacta de 15 con existencia 15 en ValidadorExistencia · **Aprobado (Unitaria)** · 18-oct-2026 · Dev B · `evidencias/SI-014/Green_Test.md` |

### CP-17 — Redondeo bancario y ajuste por residuo
| Campo | Valor |
|---|---|
| Requisito | RB-08, RB-09, RNF-01 |
| Técnica / Nivel | Escenario E-03 / Unitaria |
| Esperado | Prom S1 = **10.00** (no 10.01) · Prom S2 = **50.03** (ajuste D-03) · saldo final **0.00** con existencia 0 |
| Obtenido / Estado / Evidencia | Redondeo bancario (ToEven) a 2 y 4 decimales validado al 100% en función pura · **Aprobado (Unitaria - Redondeo)** · 18-oct-2026 · Dev B · `evidencias/SI-011/Green_Test.md` (ajuste por residuo D-03 pendiente en SI-016) |

### CP-18 — Movimiento sobre producto inactivo
| Campo | Valor |
|---|---|
| Requisito | RF-01, RB-10 |
| Técnica / Nivel | Tabla R2 / Funcional |
| Precondiciones | `P-002` inactivo |
| Datos | Entrada 5 @ 10.00 y salida 1 |
| Esperado | Rechazo "El producto está inactivo"; sin movimientos nuevos |
| Obtenido / Estado / Evidencia | _pendiente_ |

### CP-19 — Invariante de valores almacenados
| Campo | Valor |
|---|---|
| Requisito | RNF-01, RF-05 |
| Técnica / Nivel | Escenario E-01, E-02 / Integración |
| Esperado | Tras cada movimiento: `Productos.ValorInventarioPeps = Σ(CantidadDisponible × CostoUnitario)` de capas · `= SaldoValorPeps` del último movimiento · `Productos.ExistenciaActual = Σ entradas − Σ salidas` · `ValorInventarioPromedio = SaldoValorPromedio` del último movimiento |
| Obtenido / Estado / Evidencia | _pendiente_ |
