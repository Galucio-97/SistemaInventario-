## 📌 Descripción del Cambio
<!-- Explica brevemente el propósito de este PR y qué problema resuelve -->
- **Ticket / Issue**: Closes # <!-- ej. #7 o SI-006 -->
- **Tipo de cambio**:
  - [ ] Nueva funcionalidad (`feat`)
  - [ ] Corrección de defecto (`fix`)
  - [ ] Pruebas / Calidad (`test`)
  - [ ] Documentación / Configuración (`docs` / `chore`)

---

## 🎯 Trazabilidad y Calidad
- **Casos de Prueba (CP)**: <!-- ej. CP-01, CP-19 -->
- **Requisitos / Reglas**: <!-- ej. RF-01, RB-08, RNF-01 -->
- **Defecto asociado (si aplica)**: <!-- ej. DEF-001 -->

---

## 🔴🟢 Ciclo TDD
<!-- Registrar los commits o referencias que demuestran el flujo TDD -->
- [ ] **Commit RED**: `test(SI-XXX): red - <descripción>` <!-- hash o mensaje -->
- [ ] **Commit GREEN**: `feat(SI-XXX): green - <descripción>` <!-- hash o mensaje -->

---

## ✅ Checklist Definition of Done (DoD)
Para integrar este PR hacia `dev`, se deben cumplir todas las condiciones siguientes:

- [ ] **TDD estricto**: La rama contiene el ciclo demostrable RED → GREEN.
- [ ] **Etiquetado `[Trait]`**: Todas las pruebas añadidas incluyen `[Trait("CP", ...)]`, `[Trait("RF", ...)]` y `[Trait("Nivel", ...)]`.
- [ ] **Compilación limpia**: `dotnet build` finaliza sin errores ni advertencias nuevas.
- [ ] **Suite en Verde**: `dotnet test` pasa al 100% (toda la suite en verde, sin tests ignorados).
- [ ] **Arquitectura limpia**: Ninguna regla de negocio vive únicamente en la interfaz o controladores.
- [ ] **Evidencia generada**: Registro o captura guardada en `evidencias/CP-XX/` o `evidencias/DEF-XXX/`.
- [ ] **Documentación viva actualizada**:
  - [ ] Ficha en `docs/03-diseno-casos-de-prueba.md` (Resultado Obtenido, Fecha, Estado).
  - [ ] Matriz en `docs/04-matriz-trazabilidad.md` (nombre del método xUnit registrado).
  - [ ] Registro en `docs/05-registro-defectos.md` (si corrige un DEF).
  - [ ] Backlog en `docs/backlog.md` (estado del ticket actualizado).

---

## 👥 Revisión Cruzada (Peer Review)
- [ ] **Revisado por**: <!-- Nombre del compañero que NO programó el ticket -->
- [ ] **Pruebas manuales/independientes verificadas**: <!-- Confirmación de ejecución -->
