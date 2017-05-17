using SharpGEDParser.Model;
using System;

// TODO in 1850, if person lived to age 20, life expectancy was 60

// For GEDCOMs where birth/death dates are missing or un-parseable,
// attempt to estimate birth/death dates. Estimates are performed
// based on available information: child or parent birth, marriage
// date, etc.
//
namespace GEDWrap
{
    public static class DateEstimator
    {
        private static bool NeedsEstimateBirth(Person p)
        {
            var b = p.Birth;
            if (b != null &&
                b.GedDate != null &&
                b.GedDate.Type != GEDDate.Types.Unknown)
                return false;
            return p._estimatedBirth == null;
        }

        private static bool Pass(Forest f)
        {
            // TODO consider putting needs estimate to a list to reduce full-scans

            int tot = 0;
            int count = 0;
            foreach (var person in f.AllPeople)
            {
                tot++;
                if (NeedsEstimateBirth(person))
                {
                    if (!EstimateBirth(person))
                        count++;
                }
            }
            Console.WriteLine("Miss:{0} Tot:{1} ({2}%)", count, tot, 100.0 * count / tot);
            return count > 0;
        }

        // Estimate all missing birth/death dates
        // Make at most three passes; stop if no estimates needed
        public static void Estimate(Forest f)
        {
            bool keepGoing = Pass(f);
            if (keepGoing)
                keepGoing = Pass(f);
            if (keepGoing)
                keepGoing = Pass(f);
        }

        private static void RangeCheck(Person p, ref long lastBorn, ref long firstDead)
        {
            // Determine if a person is born later than existing history
            // Determine if a person is dead sooner? than existing history

            if (p == null)
                return;
            GEDDate birthDate = p.BirthDate; // uses estimated date if necessary
            if (birthDate != null)
            {
                lastBorn = Math.Max(lastBorn, birthDate.JDN);
            }

            if (p.Death != null &&
                p.Death.GedDate != null &&
                p.Death.GedDate.Type != GEDDate.Types.Unknown)
            {
                GEDDate dadBorn = p.Death.GedDate;
                if (dadBorn.JDN > firstDead) // TODO this looks wrong
                    firstDead = dadBorn.JDN;
            }
        }

        // Attempt to estimate a person's birth date range
        // TODO establish range
        // TODO average age of marriage for women before 1800: 20, after: 23
        // TODO average age of marriage for men: 25
        // TODO average age of last birth for women: 40
        // returns true if estimate was made
        public static bool EstimateBirth(Person p)
        {
            // 1. person not born before parent-birth+16
            // 2. person not born after parent-death+1
            // 3. person not born before own-marriage-date-16
            // 4. person not born before parent-marriage-date-1
            // 5. person not born before child-16
            // 6. person not born before spouse-birth-20

            long firstParentMarriage = long.MaxValue;
            long firstOwnMarriage = long.MaxValue;
            long lastParentBorn = long.MinValue;
            long firstParentDead = long.MaxValue;
            long firstSpouseBorn = long.MinValue;
            long firstChildBorn = long.MinValue;
            foreach (var union in p._childIn)
            {
                // TODO this is not taking adoption into account!

                RangeCheck(union.Husband, ref lastParentBorn, ref firstParentDead);
                RangeCheck(union.Wife, ref lastParentBorn, ref firstParentDead);

                GEDDate md = union.MarriageDate;
                if (md != null && md.Type != GEDDate.Types.Unknown && md.JDN < firstParentMarriage)
                    firstParentMarriage = md.JDN;
            }
            foreach (var union in p._spouseIn)
            {
                long junk = 0;
                GEDDate md = union.MarriageDate;
                if (md != null && md.Type != GEDDate.Types.Unknown && md.JDN < firstOwnMarriage)
                    firstOwnMarriage = md.JDN;
                if (union.Husband != p) // TODO 'otherspouse' accessor?
                    RangeCheck(union.Husband, ref firstSpouseBorn, ref junk);
                else
                    RangeCheck(union.Wife, ref firstSpouseBorn, ref junk);
                foreach (var child in union.Childs)
                {
                    RangeCheck(child, ref firstChildBorn, ref junk);
                }
            }

            long result = 0;
            if (lastParentBorn != long.MinValue)
            {
                // 1. person not born before parent-birth+16
                result = Math.Max(result, lastParentBorn + 16 * 365);
            }
            if (firstParentMarriage != long.MaxValue)
            {
                // 4. person not born before parent-marriage-date+1
                result = Math.Max(result, firstParentMarriage + 365);
            }
            if (firstParentDead != long.MaxValue)
            {
                // 2. person not born after parent-death+1
                result = Math.Min(result, firstParentDead + 365);
            }
            if (firstOwnMarriage != long.MaxValue)
            {
                // 3. person not born before own-marriage-date-16
                result = Math.Max(result, firstOwnMarriage - 16 * 365);
            }
            if (firstChildBorn != long.MinValue)
            {
                // 5. person not born before child-16
                if (result == 0) // TODO init to MAXVALUE?
                    result = firstChildBorn - 16 * 365;
                else
                    result = Math.Min(result, firstChildBorn - 16 * 365);
            }
            if (firstSpouseBorn != long.MinValue)
            {
                // 6. person not born before spouse-birth-20
                result = Math.Max(result, firstSpouseBorn - 20 * 365);
            }
            if (result == 0)
                return false;

            GEDDate output = new GEDDate(GEDDate.Types.Estimated);
            output.JDN = result;
            p._estimatedBirth = output;
            return true;
        }
    }
}
