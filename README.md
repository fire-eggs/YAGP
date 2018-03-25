# YAGP
"Yet Another GEDCOM Parser" - newer/faster/complete GEDCOM parser in C#

The intent is to provide a library to parse and validate 5.5/5.5.1 GEDCOM files. The library needs to do so quickly and with low 
memory consumption. Non-standard tags and data need to be preserved.

Several "demo" programs and WinForms controls will be provided to show off and exercise the library's capabilities.

Very much a work-in-progress.

General status:
- Released V0.2-Alpha: stable, performant, nearly complete GEDCOM parsing.
- GEDCOM writing in-progress.
- Library reference documentation in-progress.
- Documentation, samples, localization, and memory usage all need improvement.
- Date parsing/estimation is partially working: needs to be more robust, handle more cases and calendars.
- Demo programs and controls are crude and not yet refactored cleanly.
- Demo programs need improved navigation/searching amongst INDIs.
- Demo programs need print/preview consistently implemented.
- A couple of statistics programs exist but still need a _useful_ validation program.

Additional details and documentation-in-progress can be found in the [Wiki](../../wiki).

Some related material in my [Github pages (http://fire-eggs.github.io/ )]
