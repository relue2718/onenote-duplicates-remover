using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    // Versions up to 1.0.1.11 wrote dumps with Newtonsoft.Json:
    //   JsonConvert.SerializeObject(Dictionary<string, List<Tuple<string, string>>>) via StreamWriter(path, false, Encoding.UTF8),
    // so files start with a UTF-8 BOM and keep non-ASCII text unescaped.
    public class PageGroupDumpCompatibilityTests
    {
        private static readonly PageGroup[] Scan =
        {
            new PageGroup("H1", new[] { new PageRef("p1", "회의록 \"초안\"", @"C:\Notes\A.one"), new PageRef("p2", "회의록 \"초안\"", @"C:\Notes\B.one") }),
            new PageGroup("H2", new[] { new PageRef("p3", "Recipe <b>", @"C:\Notes\C.one") }),
            new PageGroup("H3", new[] { new PageRef("p4", "Other", @"C:\Notes\D.one") }),
        };

        [Fact]
        public void DumpsWrittenByNewtonsoftVersionsStillLoad()
        {
            Dictionary<string, List<Tuple<string, string>>> legacy = new Dictionary<string, List<Tuple<string, string>>>
            {
                ["H1"] = new List<Tuple<string, string>> { Tuple.Create("old-1", "회의록 \"초안\"") },
                ["H2"] = new List<Tuple<string, string>> { Tuple.Create("old-2", "Recipe <b>") },
            };
            string path = Path.GetTempFileName();
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.Write(JsonConvert.SerializeObject(legacy));
                }
                Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, File.ReadAllBytes(path).Take(3));

                // Read the way FormMain reads a dump file.
                string json;
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8, true))
                {
                    json = reader.ReadToEnd();
                }
                List<PageRef> pages = PageGroupDump.SelectPagesWithDumpedContent(Scan, json);

                Assert.Equal(new[] { "p1", "p2", "p3" }, pages.Select(page => page.PageId));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void NewDumpsLoadWithTheNewtonsoftTypes()
        {
            string json = PageGroupDump.Serialize(Scan);

            Dictionary<string, List<Tuple<string, string>>> legacy =
                JsonConvert.DeserializeObject<Dictionary<string, List<Tuple<string, string>>>>(json)!;

            Assert.Equal(new[] { "H1", "H2", "H3" }, legacy.Keys);
            Assert.Equal(Tuple.Create("p1", "회의록 \"초안\""), legacy["H1"][0]);
            Assert.Equal(Tuple.Create("p3", "Recipe <b>"), legacy["H2"][0]);
        }
    }
}
