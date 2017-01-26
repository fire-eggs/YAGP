﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDWrap.Tests
{
    class zzFileTest
    {
        [Test]
        public void SimpleGed()
        {
            var path = @"E:\projects\YAGP\Sample GED\export_ged_919.ged";// TODO project-relative path

            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(0, ged.Errors.Count);

            var indis = ged.Indi;
            Assert.AreEqual(2, indis.Count);

            var indi = ged.FindIndiByIdent("I919");
            Assert.IsNotNull(indi);

            Assert.AreEqual('M', indi.Sex);
            Assert.IsNotNull(indi.CHAN.Date);
            Assert.AreEqual(1, indi.Names.Count);
        }

        [Test]
        public void SimpleGed2()
        {
            var path = @"E:\projects\YAGP\Sample GED\ege.ged"; // TODO project-relative path
            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(0, ged.Errors.Count);

            var indi = ged.FindIndiByIdent("I26");
            Assert.IsNotNull(indi);
        }

        [Test]
        public void SimpleGed3()
        {
            var path = @"Z:\HOST_E\projects\GED\GED files\01small\pallanezf.ged"; // TODO project-relative path
            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(0, ged.Errors.Count);

            var indi = ged.FindIndiByIdent("I30");
            Assert.IsNotNull(indi);
        }

    }
}