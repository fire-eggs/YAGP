Sample : "time line"

Timeline is a neat little program which displays GEDCOM events
on Google maps in chronological order. It is open source and
can be found at https://sourceforge.net/projects/time-line/

As written, the program has a few limitations due to how it
reads GEDCOM files. Here I have reworked the project to use
SharpGedParser to read GEDCOM files instead. As a result I 
fixed several problems in SharpGedParser and solved some of
the limitations in Timeline. 

In this repository, I have forked the Timeline project (the 
2017-06-15 version), separated the GEDCOM reading code into
a separate class, then used SharpGedParser. This process
took only a few man-hours spread across a couple of days.

Improvements from SharpGedParser

1. Marriage events. The original project did not handle these
   correctly.
1. Less Memory: the original program reads all GEDCOM lines into
   memory. For non-trivial GEDCOM files this could cause problems.
1. Better support for GEDCOM variants: linefeed terminated,
   tags in unexpected order, other encodings, etc.

Lost Features

1. Viewing the GEDCOM file.
2. Custom events: Timeline currently doesn't support custom events
   (e.g. _FA1, _FA2, _MILT, etc). However, should users desire to 
   see custom events, extra work will be required to access them
   via SharpGedParser.
3. I replaced the original generated person numbers with the GEDCOM 
   INDI record id. This approach was easier for marriage events.

