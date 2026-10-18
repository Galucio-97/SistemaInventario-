# Evidencia Técnica — SI-010: Base de Datos de Pruebas Aislada

## 1. Resumen del Ticket
- **ID:** `SI-010`
- **Release:** `v0.1.0` (Datos y ambiente de pruebas)
- **Objetivo:** Que las pruebas de integración corran contra SQL Server real (D-06) sin contaminar la BD principal de la aplicación ni depender del orden de ejecución entre pruebas.
- **Trazabilidad:** `D-06` (BD dedicada en SQL Server real), base para `CP-01`, `CP-02`, `CP-12`, `CP-13`, `CP-19`, `RNF-02` (Persistencia), `RNF-04` (Consistencia).

---

## 2. Criterios de Aceptación Verificados

1. **BD dedicada con auto-creación y migración:**
   - Base de datos dedicada: `SistemaInventarioDb_Tests`.
   - `TestDbContextFixture.EnsureDatabaseMigrated()` ejecuta automáticamente `context.Database.Migrate()` en la primera invocación bajo cerrojo thread-safe (`lock`), asegurando que las tablas existan con el esquema al día.

2. **Aislamiento y estado conocido entre pruebas:**
   - Métodos `ResetDatabase()` y `ResetDatabaseAsync()` que limpian las tablas en orden inverso de claves foráneas (`CapasPeps` $\rightarrow$ `Movimientos` $\rightarrow$ `Productos`) antes o después de cada ejecución.

3. **Cadena de conexión configurable:**
   - Lee de la variable de entorno `ConnectionStrings__TestConnection` (utilizada tanto en local como en los runners de GitHub Actions CI), con fallback a la instancia local de Docker Compose.

4. **Sin proveedores InMemory:**
   - Configurado estrictamente con `.UseSqlServer()` contra el motor relacional de SQL Server.

5. **Prueba de humo con instancias independientes de DbContext:**
   - Se implementó `SmokeDatabaseTests.InsertarYLeerProducto_ConContextosNuevos_PersisteEnBaseDeDatosReal`:
     - Inserta una entidad `Producto` con `contextEscritura`.
     - Lee la entidad desde SQL Server usando `contextLectura` completamente nuevo (sin memoria caché local de primer nivel de EF Core) y valida la persistencia íntegra de sus campos.

---

## 3. Evidencia de Ejecución de Pruebas Automatizadas

Comando ejecutado:
```bash
dotnet test --filter "Nivel=Integracion" --verbosity normal
```

Salida de la consola:
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Pasa   SI.Tests.IntegrationTests.SchemaTests.CapasPepsSchemaTests.CapasInventarioPeps_Schema_DebeExistirEnContexto [1 ms]
Pasa   SI.Tests.IntegrationTests.SchemaTests.CapasPepsSchemaTests.CapasInventarioPeps_Schema_DebeTenerPrecisionesDecimalesCorrectas [< 1 ms]
Pasa   SI.Tests.IntegrationTests.SchemaTests.CapasPepsSchemaTests.CapasInventarioPeps_Schema_DebeTenerRelacionesYCheckConstraint [< 1 ms]
Pasa   SI.Tests.IntegrationTests.SchemaTests.MovimientoSchemaTests.Movimiento_Schema_DebeExistirEnContexto [< 1 ms]
Pasa   SI.Tests.IntegrationTests.SchemaTests.MovimientoSchemaTests.Movimiento_Schema_DebeTenerPrecisionDecimalCorrecta [< 1 ms]
Pasa   SI.Tests.IntegrationTests.SchemaTests.ProductoSchemaTests.Producto_Schema_DebeExistirEnContexto [< 1 ms]
Pasa   SI.Tests.IntegrationTests.SmokeDatabaseTests.InsertarYLeerProducto_ConContextosNuevos_PersisteEnBaseDeDatosReal [1 s]

Correctas! - Con error:     0, Superado:     7, Omitido:     0, Total:     7, Duración: 1 s - SI.Tests.dll (net8.0)
```

Suite completa (`dotnet test`):
```text
Correctas! - Con error:     0, Superado:     9, Omitido:     0, Total:     9, Duración: 996 ms - SI.Tests.dll (net8.0)
```

---

## 4. Certificación
- [x] BD de pruebas `SistemaInventarioDb_Tests` operativa en SQL Server real.
- [x] Auto-migración y limpieza entre pruebas garantizadas en `TestDbContextFixture`.
- [x] Prueba de humo de persistencia ejecutada y validada en verde.
- [x] Cero uso de InMemory.
