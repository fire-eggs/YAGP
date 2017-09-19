using GEDWrap;
using SharpGEDParser.Model;

namespace FamilyGroup
{
    public class Child : IDisplayChild
    {
        private readonly Person _who;

        public Child(Person who, int no, string fill)
        {
            _who = who;
            No = no;
            Filler = fill;
        }

        public int No { get; private set; }
        public string Id { get { return _who.Id; } }
        public string Name { get { return _who.Name; } }
        public string Sex { get { return _who.Indi.FullSex; } }

        // TODO all this stuff should be in Person ?

        private string date(EventCommon what)
        {
            if (what == null)
                return Filler;
            if (what.GedDate != null)
                return what.GedDate.ToString(); // TODO format
            return what.Date ?? Filler;
        }

        private string place(EventCommon what)
        {
            if (what == null || string.IsNullOrEmpty(what.Place))
                return Filler;
            return what.Place;
        }

        public string BDate { get { return date(_who.Birth); } }

        public string DDate { get { return date(_who.Death); } }

        public string BPlace { get { return place(_who.Birth); } }
        public string DPlace { get { return place(_who.Death); } }

        public string MDate { get { return date(_who.Marriage); } }

        public string MPlace { get { return place(_who.Marriage); } }

        public string MSpouse
        {
            get
            {
                Union marr = _who.MarriageUnion;
                if (marr == null)
                    return Filler;
                var spouse = marr.Spouse(_who);
                if (spouse == null || spouse.Name == null)
                    return "";
                return spouse.Name;
            }
        }

        public string Filler { get; set; }
    }
}
