# 02 · Requisitos, Reglas de Negocio y Decisiones

> Fuente única de verdad del alcance. Cualquier cambio a este documento requiere acuerdo de ambos integrantes y se registra en la sección 6 (bitácora de cambios).

## 1. Propósito del MVP

Sistema de inventarios que permite administrar productos, registrar entradas y salidas de mercancía, consultar existencias y calcular el costo de los movimientos mediante **PEPS** y **Costo Promedio Ponderado móvil**. El objetivo es **demostrar mediante evidencia** que los resultados son correctos, consistentes y verificables.

**Fuera de alcance:** facturación, proveedores, compras completas, ventas comerciales, usuarios/roles y cualquier módulo administrativo adicional.

**Stack:** C# · .NET 8 · Razor Pages · EF Core 8 · SQL Server · xUnit.

## 2. Requisitos funcionales

| ID | Requisito |
|---|---|
| RF-01 | Registrar, consultar y modificar productos (identificador único, nombre, descripción opcional, unidad de medida, estado). |
| RF-02 | Registrar entradas: fecha, producto, cantidad, costo unitario de adquisición. Actualiza existencias y valor del inventario. |
| RF-03 | Registrar salidas: fecha, producto, cantidad. Calcula el costo de salida según el método de costeo. |
| RF-04 | Consultar resultados de PEPS y Promedio Ponderado sobre el mismo conjunto de movimientos. |
| RF-05 | Mostrar existencias, costo unitario según método, valor del inventario e historial de entradas y salidas. |

## 3. Requisitos no funcionales

| ID | Requisito |
|---|---|
| RNF-01 | Integridad: cantidades y costos consistentes en los movimientos. |
| RNF-02 | Persistencia: los datos sobreviven al cierre y reinicio de la aplicación. |
| RNF-03 | Validación: datos incorrectos se rechazan con mensajes comprensibles. |
| RNF-04 | Consistencia: una operación fallida no deja actualizaciones parciales. |
| RNF-05 | Mantenibilidad: componentes con responsabilidades identificables. |
| RNF-06 | Trazabilidad: cada RF se relaciona con uno o más casos de prueba. |

## 4. Reglas de negocio

| ID | Regla |
|---|---|
| RB-01 | Cantidades ≤ 0 se rechazan. |
| RB-02 | Costos unitarios negativos o cero se rechazan en entradas. |
| RB-03 | Salidas superiores a la existencia disponible se rechazan. |
| RB-04 | Todo movimiento se asocia a un producto existente y conserva fecha, cantidad y costo. |
| RB-05 | Los movimientos confirmados persisten en la base de datos. |
| RB-06 | Las operaciones rechazadas no modifican existencias ni valor del inventario. |
| RB-07 | El costo de cada salida y el valor restante corresponden al método de costeo utilizado. |
| RB-08 | Importes monetarios con precisión decimal (`decimal`), nunca `float`/`double`. |
| RB-09 | Las reglas de redondeo se documentan (sección 5) y se aplican uniformemente. |
| RB-10 | *(D-05)* No se permiten entradas ni salidas sobre productos inactivos. |
| RB-11 | *(D-04)* No se permite registrar un movimiento con fecha anterior al último movimiento del producto. |

## 5. Política de precisión y redondeo (RB-08, RB-09)

### 5.1 Modo de redondeo: **Bancario (half to even)** — Decisión D-02

Cuando el dígito descartado es **exactamente 5** (sin dígitos distintos de cero después), se redondea al **par más cercano**. En cualquier otro caso se redondea al más cercano de forma normal.

| Valor | 2 decimales | Motivo |
|---|---|---|
| 10.005 | **10.00** | El anterior (0) es par → se conserva |
| 10.015 | **10.02** | El anterior (1) es impar → sube a par |
| 10.0051 | **10.01** | Hay dígitos después del 5 → sube |
| 50.025 | **50.02** | El anterior (2) es par → se conserva |

**Justificación:** evita el sesgo acumulado hacia arriba del redondeo tradicional en volúmenes grandes de operaciones; es el modo por defecto de IEEE 754 y de `Math.Round` en .NET.

### 5.2 Escalas

| Magnitud | Escala | Tipo BD | Función |
|---|---|---|---|
| Cantidades | 4 decimales | `decimal(18,4)` | — (se validan, no se redondean) |
| Costo unitario (entrada, promedio, unitario equivalente PEPS) | 4 decimales | `decimal(18,4)` | `PoliticaRedondeo.CostoUnitario(x)` = `Math.Round(x, 4, MidpointRounding.ToEven)` |
| Importes y saldos (costo total, valor de inventario) | 2 decimales | `decimal(18,2)` | `PoliticaRedondeo.Importe(x)` = `Math.Round(x, 2, MidpointRounding.ToEven)` |

### 5.3 Reglas de aplicación

1. **Siempre** se especifica `MidpointRounding.ToEven` de forma explícita (no se depende del valor por defecto).
2. **Todo redondeo ocurre en C# antes de persistir.** Nunca se delega a la conversión implícita de SQL Server/EF (que puede truncar o redondear con otro criterio).
3. El costo total de un movimiento = `Importe(cantidad × costoUnitario)`.
4. **Promedio móvil:** en cada entrada, `nuevoPromedio = CostoUnitario((valorAnterior + valorEntrada) / (cantidadAnterior + cantidadEntrada))`. Las salidas usan el promedio vigente y **no** lo modifican.
5. **PEPS:** el costo de una salida es la suma de `Importe(unidadesTomadas × costoCapa)` de cada capa consumida, de la más antigua a la más reciente (orden `Fecha ASC, Id ASC`).
6. **Ajuste por residuo (D-03):** cuando una salida deja la existencia en 0, su costo total = **saldo de valor restante**, de modo que existencia 0 ⇒ valor 0.00. Aplica a ambos métodos.

## 6. Bitácora de decisiones

| ID   | Decisión                                                                                                                                                                       | Estado                 | Fecha |
|------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------|---|
| D-01 | Cantidades **decimales** `decimal(18,4)`; cantidades y costos unitarios con más de 4 decimales se **rechazan** (evita que la BD altere silenciosamente el dato capturado)      | Aprobada por el equipo | 2026-10-16 |
| D-02 | Redondeo **bancario** (`ToEven`)                                                                                                                                               | Aprobada por el equipo | 2026-10-16 |
| D-03 | Ajuste por residuo cuando existencia llega a 0                                                                                                                                 | Aprobada por el equipo | 2026-10-16 |
| D-04 | Rechazar fechas anteriores al último movimiento del producto                                                                                                                   | FALTA CONFIRMACIÓN     | 2026-10-16 |
| D-05 | Rechazar movimientos sobre productos inactivos                                                                                                                                 | FALTA CONFIRMACIÓN     | 2026-10-16 |
| D-06 | BD de pruebas dedicada `SistemaInventarioDb_Tests` en SQL Server real                                                                                                          | Aprobada por el equipo | 2026-10-16 |
| D-07 | Flujo **TDD Red → Green** por caso de prueba                                                                                                                                   | Aprobada por el equipo | 2026-10-16 |
| D-08 | 2 integrantes · 1 semana (entrega 24-oct-2026)                                                                                                                                 | Aprobada por el equipo | 2026-10-16 |
| D-09 | El método PEPS asigna a las salidas el costo de las unidades que ingresaron primero.                                                                                           | Aprobado por el equipo | 2026-10-16 |
| D-10 | Cada nueva entrada actualizará el costo promedio con base en el valor y la cantidad del inventario disponible antes de la compra y los correspondientes a la nueva entrada. <br/>`Costo promedio = (Valor anterior + Valor de entrada) / (Cantidad anterior + Cantidad de entrada)`   | Aprobado por el equipo | 2026-10-16 |