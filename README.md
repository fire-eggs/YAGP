# YAGP
"Yet Another GEDCOM Parser" - newer/faster/complete GEDCOM parser in C#

WIP
Added a couple of "test" programs which consume the results from the GEDCOM parser. Crude and not yet refactored cleanly.

Both attempt to connect up a family tree using the records parsed from a GED file. A few "outlier" situations are sort of handled, needing to be completed or improved. For example:
o An individual who is part of multiple families (i.e. birth and adoption).
o A family record references an INDI record which doesn't exist.
o Duplicated family record.
o An individual record with FAMS / FAMC references, but no corresponding family record reference.

The "DrawAnce" project is a little bit interesting, drawing the ancestry tree for any given individual.
