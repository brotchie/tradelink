using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    public class MyResponse : ResponseTemplate
    {
        public int EntrySize = 100;
    }

    [TestFixture]
    public class TestSkin
    {
        public TestSkin() 
        {
        }


        [Test]
        public void SerializationDeserialization()
        {
            // create a test response
            MyResponse rt = new MyResponse();
            // our test response is located here
            const string dll = "TestTradeLink.exe";
            // change a property on the test
            rt.Name = "testing123";
            // change custom value
            rt.EntrySize = 333;
            // make sure repsonse is good
            Assert.IsTrue(rt.isValid);
            // make sure custom value holds
            Assert.AreEqual(333, rt.EntrySize);

            // Skin it
            string skin = SkinImpl.Skin(rt, "TestTradeLink.MyResponse",Environment.CurrentDirectory+"\\"+dll);

            // deskin it
            object o = SkinImpl.Deskin(skin);
            // cast it back
            MyResponse me = (MyResponse)o;
            // check response is valid
            Assert.IsTrue(me.isValid);
            // check that property was persisted
            Assert.AreEqual(rt.Name, me.Name);
            // check that custom value persisted
            Assert.AreEqual(rt.EntrySize, me.EntrySize);




            
        }
    }
}
