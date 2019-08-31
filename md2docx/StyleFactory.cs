using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace md2docx
{
    public class StyleFactory
    {
        public Style GenerateStyle(JObject jObject)
        {
            return new Style
            {
                Type = StyleValues.Paragraph,
                StyleId = (string)jObject["名称"],
                StyleName = new StyleName { Val = (string)jObject["名称"] },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = (string)jObject["英文字体"], HighAnsi = (string)jObject["英文字体"], ComplexScript = (string)jObject["英文字体"], EastAsia = (string)jObject["中文字体"] },
                    FontSize = new FontSize { Val = (string)jObject["字体大小"] },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = (string)jObject["字体大小"] }
                }
            };
        }
    }
}
