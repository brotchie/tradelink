using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestGZip
    {
        public TestGZip()
        {
        }

        [Test]
        public void CompressDecompress()
        {
            // create a random seed so we can vary the data
            Random r = new Random((int)DateTime.Now.Ticks);
            // parts to the input string 
            int parts = r.Next(100, 300);
            // repeitions in each part of string
            int reps = r.Next(5, 50);
            // length of string
            int len = reps * parts;
            // prepare our input string
            StringBuilder os = new StringBuilder(len);
            // keep count how many characters we've written
            int chars = 0;
            // build each part
            for (int p = 0; p < parts; p++)
            {
                // pick a character
                char c = (char)(r.Next(1, 26) + 64);
                // add it to string 'reps' number of times
                while (chars++ % reps != 0)
                    os.Append(c);
            }
            // get our data to compress
            string original = os.ToString();
            // compress it
            string compress = GZip.Compress(original);
            // uncompress it
            string uncompress = GZip.Uncompress(compress);
            // verify no data was lost
            Assert.AreEqual(original, uncompress);
            // verify some compression occured
            Assert.Less(compress.Length, original.Length);


        }



    }
}
