# Workflow Operativo

Para lograr el mejor enfoque ante nuestra calidad de software haremos los siguiente:

Este flujo "loop" debe ser usado cada vez que se tome un ticket del tablero, ejecutandolo estrictamente paso a paso:

- Paso 1: Mover ticket a `In Progress` en GitHub y crear rama: `feat/SI-XXX-nombre`.
- Paso 2: Escribir la prueba que FALLE (RED):
  - Tomamos el dato y el oráculo exacto de `docs/03-diseno-casos-de-prueba.md`.
  - Escribimos el test en `SI.Tests` con sus tags: `[Trait("CP", "CP-XX")]`.
  - Ejecutamos dotnet test: falla → commit: test(SI-XXX): red - <descripción>.
- Paso 3: Implementar lo mínimo para que PASE (GREEN):
  - Escribimos la lógica en `SI.Web`.
  - Ejecutamos `dotnet test`: pasa → commit: feat(SI-XXX): green - <descripción>.
- Paso 4: Generar la Evidencia en el acto:
  - Si es test unitario/integración: guardamos el log o .trx en `evidencias/CP-XX/.
  - Si toca la BD: ejecutamos un query SQL que demuestre que los datos cambiaron correctamente.
  - Si es pantalla: captura de pantalla clara con la URL y los datos del ejercicio.
- Paso 5: Actualizar la documentación viva:
  - Marcamos [x] en el Issue de GitHub.
  - En docs/03-diseno-casos-de-prueba.md, llenamos en la ficha: Resultado Obtenido, Fecha, Ejecutor y Estado: Aprobado.
  - En docs/04-matriz-trazabilidad.md, colocamos el nombre del método de prueba en la columna correspondiente.
- Paso 6: Pull Request y Merge:
  - Se abre PR hacia dev. El compañero que no lo programó lo revisa y aprueba.