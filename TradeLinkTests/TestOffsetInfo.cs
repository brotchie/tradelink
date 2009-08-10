using System;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
       [TestFixture]
    public class TestOffsetInfo
    {
           public TestOffsetInfo()
           {
           }

           [Test]
           public void SerializeDeserialize()
           {
               OffsetInfo oi = new OffsetInfo(2, 1);
               string msg = OffsetInfo.Serialize(oi);
               OffsetInfo co = OffsetInfo.Deserialize(msg);
               Assert.AreEqual(2,oi.ProfitDist);
               Assert.AreEqual(1, oi.StopDist);
               
           }
    }
}
