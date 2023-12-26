using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SUBDLab;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IntField TestField = new IntField("Test");
            TestField.addRow();
            Assert.AreEqual(TestField.ChangeValue(0, "-100"), 0);
        }
        [TestMethod]
        public void TestMethod2()
        {
            RealField TestField=new RealField("Test");
            TestField.addRow();
            Assert.AreEqual(TestField.ChangeValue(1, "5,5"), 2);
        }
        [TestMethod]
        public void TestMethod3()
        {
            ColorField TestField=new ColorField("Test");
            TestField.addRow();
            Assert.AreEqual(TestField.ChangeValue(0, "#000FGF"), 1);
        }
        [TestMethod]
        public void TestMethod4()
        {
            ColorInvlField TestField=new ColorInvlField("Test");
            TestField.addRow();
            Assert.AreEqual(TestField.ChangeValue(0, "#0fAb60-#0FaB61"), 0);
        }
        [TestMethod]
        public void TestMethod5()
        {
            DBMenu menu = new DBMenu();
            menu.CurrentBase = new Base();
            List<Tuple<string,string>> NamesTypes=new List<Tuple<string,string>>();
            NamesTypes.Add(new Tuple<string, string>("1", "Int"));
            menu.CreateTable("Table1", NamesTypes);
            Assert.AreEqual(menu.CreateTable("Table1", NamesTypes), 1);
        }
        [TestMethod]
        public void TestMethod6()
        {
            DBMenu menu = new DBMenu();
            menu.CurrentBase = new Base();
            List<Tuple<string, string>> NamesTypes = new List<Tuple<string, string>>();
            NamesTypes.Add(new Tuple<string, string>("1", "String"));
            NamesTypes.Add(new Tuple<string, string>("2", "String"));
            menu.CreateTable("Table1", NamesTypes);
            NamesTypes = new List<Tuple<string, string>>();
            NamesTypes.Add(new Tuple<string, string>("2", "String"));
            NamesTypes.Add(new Tuple<string, string>("1", "String"));
            menu.CreateTable("Table2", NamesTypes);
            menu.OpenTable("Table1");
            menu.AddRow(); menu.AddRow();
            menu.ChangeRowValue(0, 0, "1"); menu.ChangeRowValue(0, 1, "2");
            menu.ChangeRowValue(1, 0, "1"); menu.ChangeRowValue(1, 1, "1");
            menu.OpenTable("Table2");
            menu.AddRow(); menu.AddRow();
            menu.ChangeRowValue(0, 0, "2"); menu.ChangeRowValue(0, 1, "1");
            menu.ChangeRowValue(1, 0, "2"); menu.ChangeRowValue(1, 1, "2");
            List<string> s = new List<string>(); s.Add("Table1"); s.Add("Table2");
            menu.Union("Table3",s);
            menu.OpenTable("Table3");
            Assert.AreEqual(menu.CurrentTable.Fields[0].Values.Count, 3);
        }
    }
}
