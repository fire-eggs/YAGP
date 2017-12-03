using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class Events : GedWriteTest
    {
        [Test]
        public void Text()
        {
            var inp = "0 @I1@ INDI\n1 DEAT Y";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void Dscr()
        {
            var inp = "0 @I1@ INDI\n1 DSCR He's a big man then?";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void DscrCont()
        {
            var inp = "0 @I1@ INDI\n1 DSCR He's a big man then?\n2 CONT I don't know the secret handshake";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void DscrConc()
        {
            var inp = "0 @I1@ INDI\n1 DSCR He's a big man then\n2 CONC ? I don't know the secret handshake";
            var exp = "0 @I1@ INDI\n1 DSCR He's a big man then? I don't know the secret handshake\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void LongNote()
        {
            // A real world very long note
            // From a RootsMagic file where they have done CONC correctly
            var inp = "0 @N1@ NOTE " +
                      "MR PERCY JAMES HATCHER was born at Brent Knoll, a village over the othe\n" +
                      "1 CONC r side of the hill from Rooksbridge, last but one in a family of ten br\n" +
                      "1 CONC others and sisters. On leaving school Percy helped for a couple of year\n" +
                      "1 CONC s in the pork butchery business of his late father. However, being very f\n" +
                      "1 CONC ond of horses, he left the district to go to Cromhall, a village in Glo\n" +
                      "1 CONC ucestershire, and for a period of three years looked after the hunters b\n" +
                      "1 CONC elonging to a family there. His father had by that time retired from th\n" +
                      "1 CONC e butchery and, the brother who had been running it<b> </b>having been c\n" +
                      "1 CONC alled up to join the Army, Percy returned home to manage the business. B\n" +
                      "1 CONC esides doing the buying, the slaughtering and the selling in the shop, h\n" +
                      "1 CONC e drove a horse-and-trap to deliver to customers in the surrounding cou\n" +
                      "1 CONC ntry­side. It was not long, though.before he himself was of<b> </b>age t\n" +
                      "1 CONC o<b> </b>join the Army, and in October 1916 Was enrolled in the Royal B\n" +
                      "1 CONC erkshire Regiment.\n" +
                      "1 CONT After a short period of training he was posted to France and took part i\n" +
                      "1 CONC n the fierce engagements that followed in<b> </b>that country and in<b> <\n" +
                      "1 CONC /b>Belgium. In June 1918, during the fighting near Albert, he was wound\n" +
                      "1 CONC ed in the groin by a hand grenade and had to spend eight months in hosp\n" +
                      "1 CONC ital in England From February 1919, when he was demobilised, Percy ran<\n" +
                      "1 CONC b> </b>the butchery business for seven years in partnership with his br\n" +
                      "1 CONC other. He<b> </b>then took a variety of jobs around his borne district u\n" +
                      "1 CONC ntil, in March 1934, he joined the Cheddar Valley Dairy Co to work in t\n" +
                      "1 CONC he Cheese room.\n" +
                      "1 CONT For the following twelve years, besides this work, lie helped in variou\n" +
                      "1 CONC s departments as occasion required, and then divided his time between m\n" +
                      "1 CONC ilk reception and relief boiler man. In 1954 Percy was given charge of t\n" +
                      "1 CONC he Boiler house and its two Cornish boilers, both of which are now oil-\n" +
                      "1 CONC fired.\n" +
                      "1 CONT Mr. and Mrs. Hatcher (she was a sister of the late Manager of the Cream\n" +
                      "1 CONC ery, Mr. Charlie Emery) had three children, all married: a son, who liv\n" +
                      "1 CONC es in<b> </b>Canada, and two daughters: and each of them has two childr\n" +
                      "1 CONC en. Percy was an active committee member of the Brent Knoll branch of t\n" +
                      "1 CONC he British Legion. He used to play a lot of skittles, with considerable s\n" +
                      "1 CONC uccess; and he and Mrs. Hatcher did much ballroom dancing.\n" +
                      "1 CONT";
            var fr = ReadItHigher(inp);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            Assert.AreEqual(inp, res); // TODO split long lines

        }
    }
}
