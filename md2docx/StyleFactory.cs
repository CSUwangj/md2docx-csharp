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
            string size;
            if (int.TryParse((string)jObject["字体大小"], out int sz))
            {
                size = sz.ToString();
            }
            else
            {
                size = fontmap[(string)jObject["字体大小"]];
            }
            Style style = new Style
            {
                Type = StyleValues.Paragraph,
                StyleId = (string)jObject["名称"],
                StyleName = new StyleName { Val = (string)jObject["名称"] },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Justification = new Justification { Val = justmap[(string)jObject["对齐方式"]] }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = (string)jObject["英文字体"], HighAnsi = (string)jObject["英文字体"], ComplexScript = (string)jObject["英文字体"], EastAsia = (string)jObject["中文字体"] },
                    FontSize = new FontSize { Val = size },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = size }
                }
            };
            if ((bool)jObject["录入大纲"])
            {
                style.StyleParagraphProperties.OutlineLevel = new OutlineLevel { Val = (int)jObject["大纲等级"] };
            }
            if ((bool)jObject["加粗"])
            {
                style.StyleRunProperties.Bold = new Bold();
                style.StyleRunProperties.BoldComplexScript = new BoldComplexScript();
            }
            if ((bool)jObject["斜体"])
            {
                style.StyleRunProperties.Italic = new Italic();
                style.StyleRunProperties.ItalicComplexScript = new ItalicComplexScript();
            }
            if ((bool)jObject["下划线"])
            {
                style.StyleRunProperties.Underline = new Underline();
            }
            if ((bool)jObject["删除线"])
            {
                style.StyleRunProperties.Strike = new Strike();
            }
            if ((bool)jObject["段前分页"])
            {
                style.StyleParagraphProperties.PageBreakBefore = new PageBreakBefore();
            }
            
            if ((float)jObject["首行缩进"] != 0f)
            {
                style.StyleParagraphProperties.Indentation = new Indentation
                {
                    FirstLineChars = (int)((float)jObject["首行缩进"] * 100)
                };
            }
            if ((float)jObject["段前后空行"] != 0f)
            {
                style.StyleParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines
                {
                    BeforeLines = (int)((float)jObject["段前后空行"] * 100),
                    AfterLines = (int)((float)jObject["段前后空行"] * 100)
                };
            }
            if ((float)jObject["行距"] != 0f)
            {
                style.StyleParagraphProperties.SpacingBetweenLines = style.StyleParagraphProperties.SpacingBetweenLines ?? new SpacingBetweenLines();
                style.StyleParagraphProperties.SpacingBetweenLines.Line = ((int)((float)jObject["行距"] * 240)).ToString();
                style.StyleParagraphProperties.SpacingBetweenLines.LineRule = LineSpacingRuleValues.Auto;
            }

            return style;
        }
        #region Chinese font mapping
        static readonly Dictionary<string, string> fontmap = new Dictionary<string, string>
        {
            {"初号", "84"},
            {"小初", "72"},
            {"一号", "52"},
            {"小一", "48"},
            {"二号", "44"},
            {"小二", "36"},
            {"三号", "32"},
            {"小三", "30"},
            {"四号", "28"},
            {"小四", "24"},
            {"五号", "21"},
            {"小五", "18"},
            {"六号", "15"},
            {"小六", "13"},
            {"七号", "11"},
            {"八号", "10"}
        };
        #endregion
        #region justification mapping
        static readonly Dictionary<string, JustificationValues> justmap = new Dictionary<string, JustificationValues>
        {
            { "左对齐", JustificationValues.Left },
            { "居中", JustificationValues.Center },
            { "右对齐", JustificationValues.Right },
            { "分散对齐", JustificationValues.Distribute }
        };
        #endregion
    }
}
