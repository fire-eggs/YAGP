using System.Collections.Generic;
using System.Windows.Markup;
using GEDWrap;

namespace FamilyGroup
{
    public class Child : IDisplayChild
    {
        private Person _who;

        public Child(Person who, int no)
        {
            _who = who;
            No = no;
        }

        public int No { get; private set; }
        public string Id { get { return _who.Id; } }
        public string Name { get { return _who.Name; } }
        public string Sex { get { return _who.Indi.FullSex; } }
        public string BDate { get { return _who.BirthDate.ToString(); } }

        public string DDate
        {
            get
            {
                // TODO cope with these situations in Person
                // TODO Living?
                if (_who.Death == null)
                    return "-";
                if (_who.Death.GedDate != null) 
                    return _who.Death.GedDate.ToString();
                return _who.Death.Date ?? "-";
            }
        }

        public string BPlace
        {
            get
            {
                // TODO cope with these situations in Person
                if (_who.Birth == null || _who.Birth.Place == null)
                    return "-";
                return _who.Birth.Place;
            }
        }

        public string DPlace
        {
            get
            {
                // TODO cope with these situations in Person
                if (_who.Death == null || _who.Death.Place == null)
                    return "-";
                return _who.Death.Place;
            }
        }

        public string MDate
        {
            get
            {
                if (_who.Marriage == null || _who.Marriage.Date == null)
                    return "-";
                return _who.Marriage.Date; // TODO ged date?
            }
        }

        public string MPlace
        {
            get
            {
                if (_who.Marriage == null || string.IsNullOrEmpty(_who.Marriage.Place))
                    return "-";
                return _who.Marriage.Place;
            }
        }

        public string MSpouse
        {
            get
            {
                Union marr = _who.MarriageUnion;
                if (marr == null)
                    return "-";
                var spouse = marr.Spouse(_who);
                return spouse.Name;
            }
        }

        public string[] ToArray(int num)
        {
            List<string> vals = new List<string>();
            vals.Add(num.ToString());
            vals.Add(Id);
            vals.Add(Name);
            vals.Add(Sex);
            vals.Add(BDate);
            vals.Add(BPlace);
            vals.Add(DDate);
            vals.Add(DPlace);
            vals.Add(MSpouse);
            vals.Add(MDate);
            vals.Add(MPlace);
            return vals.ToArray();
        }
    }
}
