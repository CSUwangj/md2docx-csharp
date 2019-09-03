using Xunit.Abstractions;
using System.Collections.Generic;
using Xunit;
using md2docx;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace md2docxTests
{
    public class StyleFactoryTests
    {
        private readonly ITestOutputHelper output;

        public StyleFactoryTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [ClassData(typeof(CSUStyleTestData))]
        public void CSUStyleTests(string json, Style expected)
        {
            StyleFactory styleFactory = new StyleFactory();
            JObject jObject = JObject.Parse(json);
            
            Style result = styleFactory.GenerateStyle(jObject);

            output.WriteLine(expected.OuterXml);
            output.WriteLine(result.OuterXml);

            Assert.Equal(result.OuterXml, expected.OuterXml);
        }
    }

    public class CSUStyleTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[]
            {
                @"{
                  '名称': 'heading 1',
                  '英文字体': 'Times New Roman',
                  '中文字体': '黑体',
                  '字体大小': '三号',
                  '对齐方式': '居中',
                  '录入大纲': true,
                  '大纲等级': 0,
                  '加粗': false,
                  '斜体': false,
                  '下划线': false,
                  '删除线': false,
                  '首行缩进': 0,
                  '段前分页': true,
                  '段前后空行': 1,
                  '行距': 0
                }",
                new Style
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "heading 1",
                    StyleName = new StyleName { Val = "heading 1" },
                    StyleParagraphProperties = new StyleParagraphProperties
                    {
                        PageBreakBefore = new PageBreakBefore(),
                        SpacingBetweenLines = new SpacingBetweenLines() { BeforeLines = 100, AfterLines = 100 },
                        Justification = new Justification() { Val = JustificationValues.Center },
                        OutlineLevel = new OutlineLevel() { Val = 0 }
                    },
                    StyleRunProperties = new StyleRunProperties
                    {
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "黑体", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "32" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "32" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '名称': 'heading 2',
                  '英文字体': 'Times New Roman',
                  '中文字体': '黑体',
                  '字体大小': '小四',
                  '对齐方式': '左对齐',
                  '录入大纲': true,
                  '大纲等级': 1,
                  '加粗': false,
                  '斜体': false,
                  '下划线': false,
                  '删除线': false,
                  '首行缩进': 2,
                  '段前分页': false,
                  '段前后空行': 0,
                  '行距': 0
                }",
                new Style
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "heading 2",
                    StyleName = new StyleName { Val = "heading 2" },
                    StyleParagraphProperties = new StyleParagraphProperties
                    {
                        Justification = new Justification() { Val = JustificationValues.Left },
                        OutlineLevel = new OutlineLevel() { Val = 1 },
                        Indentation = new Indentation() { FirstLineChars = 200 },
                    },
                    StyleRunProperties = new StyleRunProperties
                    {
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "黑体", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '名称': 'heading 3',
                  '英文字体': 'Times New Roman',
                  '中文字体': '楷体',
                  '字体大小': '小四',
                  '对齐方式': '左对齐',
                  '录入大纲': true,
                  '大纲等级': 2,
                  '加粗': false,
                  '斜体': false,
                  '下划线': false,
                  '删除线': false,
                  '首行缩进': 2,
                  '段前分页': false,
                  '段前后空行': 0,
                  '行距': 0
                }",
                new Style
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "heading 3",
                    StyleName = new StyleName { Val = "heading 3" },
                    StyleParagraphProperties = new StyleParagraphProperties
                    {
                        Justification = new Justification() { Val = JustificationValues.Left },
                        OutlineLevel = new OutlineLevel() { Val = 2 },
                        Indentation = new Indentation() { FirstLineChars = 200 }
                    },
                    StyleRunProperties = new StyleRunProperties
                    {
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "楷体", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '名称': 'bodytext',
                  '英文字体': 'Times New Roman',
                  '中文字体': '宋体',
                  '字体大小': '小四',
                  '对齐方式': '左对齐',
                  '录入大纲': false,
                  '大纲等级': 0,
                  '加粗': false,
                  '斜体': false,
                  '下划线': false,
                  '删除线': false,
                  '首行缩进': 2,
                  '段前分页': false,
                  '段前后空行': 0,
                  '行距': 1.5
                }",
                new Style
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "bodytext",
                    StyleName = new StyleName { Val = "bodytext" },
                    StyleParagraphProperties = new StyleParagraphProperties
                    {
                        Justification = new Justification() { Val = JustificationValues.Left },
                        Indentation = new Indentation() { FirstLineChars = 200 },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "360", LineRule = LineSpacingRuleValues.Auto }
                    },
                    StyleRunProperties = new StyleRunProperties
                    {
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "宋体", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '名称': 'code',
                  '英文字体': 'Consolas',
                  '中文字体': '黑体',
                  '字体大小': '小四',
                  '对齐方式': '左对齐',
                  '录入大纲': false,
                  '大纲等级': 0,
                  '加粗': false,
                  '斜体': false,
                  '下划线': false,
                  '删除线': false,
                  '首行缩进': 0,
                  '段前分页': false,
                  '段前后空行': 0,
                  '行距': 0
                }",
                new Style
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "code",
                    StyleName = new StyleName { Val = "code" },
                    StyleParagraphProperties = new StyleParagraphProperties
                    {
                        Justification = new Justification() { Val = JustificationValues.Left },
                    },
                    StyleRunProperties = new StyleRunProperties
                    {
                        RunFonts = new RunFonts() { Ascii = "Consolas", HighAnsi = "Consolas", EastAsia = "黑体", ComplexScript = "Consolas" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '名称': 'reference',
                  '英文字体': 'Times New Roman',
                  '中文字体': '黑体',
                  '字体大小': '五号',
                  '对齐方式': '左对齐',
                  '录入大纲': false,
                  '大纲等级': 0,
                  '加粗': false,
                  '斜体': false,
                  '下划线': false,
                  '删除线': false,
                  '首行缩进': 0,
                  '段前分页': false,
                  '段前后空行': 0,
                  '行距': 1.5
                }",
                new Style
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "reference",
                    StyleName = new StyleName { Val = "reference" },
                    StyleParagraphProperties = new StyleParagraphProperties
                    {
                        Justification = new Justification() { Val = JustificationValues.Left },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "360", LineRule = LineSpacingRuleValues.Auto }
                    },
                    StyleRunProperties = new StyleRunProperties
                    {
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "黑体", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "21" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "21" }
                    }
                }
            }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
