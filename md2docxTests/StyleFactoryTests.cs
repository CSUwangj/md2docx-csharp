using Microsoft.VisualStudio.TestTools.UnitTesting;
using md2docx;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;

namespace md2docxTests
{
    [TestClass]
    public class StyleFactoryTests
    {
        [TestMethod]
        public void TestStyle()
        {
            Style style = new Style
            {
                Type = StyleValues.Paragraph,
                StyleId = "heading 1",
                StyleName = new StyleName { Val = "heading 1" },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "����", ComplexScript = "Times New Roman" },
                    FontSize = new FontSize { Val = "32" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "32" }
                }
            };
            JObject jObject = JObject.Parse(@"{""����"":""heading 1"",""Ӣ������"":""Times New Roman"",""��������"":""����"",""�����С"":""32""}");
            StyleFactory factory = new StyleFactory();
            Assert.AreEqual(factory.GenerateStyle(jObject).OuterXml, style.OuterXml);
        }
    }
}
