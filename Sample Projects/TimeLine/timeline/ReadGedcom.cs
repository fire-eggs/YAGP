using System.Collections.Generic;
using SharpGEDParser;
using SharpGEDParser.Model;

namespace timeline
{
    // Read a GEDCOM file using SharpGEDParser. Provides names of individuals, and a list
    // of "timelineentry" objects for each event in the GEDCOM file.
    //
    public class ReadGedcom
    {
        public static List<string> Individuals = new List<string>(); // holds individual names
        private static readonly List<Form1.timelineentry> TEList = new List<Form1.timelineentry>();
        private static FileRead _reader;

        public static string GedcomLines()
        {
            return ""; // TODO YAGP cannot return the GEDCOM text lines
        }

        public static void ReadFile(string filename)
        {
            _reader = new FileRead(); // TODO needs to be reset for each file
            _reader.ReadGed(filename);
        }

        // TODO lost the "individual number" when doing marriage events

        private static string PersonName(IndiRecord indi)
        {
            if (indi == null) // e.g. Marriage may not have father/mother info
                return null;
            string name = string.Format("{0}[{1}]", indi.FullName, indi.Ident);
            return name;
        }

        private static void AddTE(string name, string eType, string eDate, string ePlace)
        {
            // e.g. Marriage may not have father/mother info
            // don't bother recording events with no date or place
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(eDate) || string.IsNullOrEmpty(ePlace)) 
                return;
            Form1.timelineentry te = new Form1.timelineentry();
            te.Name = name;
            te.Date = eDate;
            te.Place = ePlace;
            te.EventType = eType;
            TEList.Add(te);
        }

        public static List<Form1.timelineentry> ReadIndividuals()
        {
            Individuals.Clear();

            foreach (var indi in _reader.AllIndividuals)
            {
                Individuals.Add(PersonName(indi));

                foreach (var evt in indi.Events)
                {
                    AddTE(PersonName(indi), evt.Tag, evt.GedDate.Initialized ? evt.GedDate.ToString() : evt.Date, evt.Place);
                }
                foreach (var evt in indi.Attribs) // For RESI, CENS
                {
                    AddTE(PersonName(indi), evt.Tag, evt.GedDate.Initialized ? evt.GedDate.ToString() : evt.Date, evt.Place);
                }
            }

            foreach (var fam in _reader.AllFamilies)
            {
                foreach (var evt in fam.FamEvents) // MARR, DIV, RESI, etc
                {
                    var father = _reader.GetDad(fam);
                    var mother = _reader.GetMom(fam);
                    AddTE(PersonName(father), evt.Tag, evt.GedDate.Initialized ? evt.GedDate.ToString() : evt.Date, evt.Place);
                    AddTE(PersonName(mother), evt.Tag, evt.GedDate.Initialized ? evt.GedDate.ToString() : evt.Date, evt.Place);
                }
            }
            return TEList;
        }
    
    }
}
