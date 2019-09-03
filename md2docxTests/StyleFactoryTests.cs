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
                  '����': 'heading 1',
                  'Ӣ������': 'Times New Roman',
                  '��������': '����',
                  '�����С': '����',
                  '���뷽ʽ': '����',
                  '¼����': true,
                  '��ٵȼ�': 0,
                  '�Ӵ�': false,
                  'б��': false,
                  '�»���': false,
                  'ɾ����': false,
                  '��������': 0,
                  '��ǰ��ҳ': true,
                  '��ǰ�����': 1,
                  '�о�': 0
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
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "����", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "32" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "32" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '����': 'heading 2',
                  'Ӣ������': 'Times New Roman',
                  '��������': '����',
                  '�����С': 'С��',
                  '���뷽ʽ': '�����',
                  '¼����': true,
                  '��ٵȼ�': 1,
                  '�Ӵ�': false,
                  'б��': false,
                  '�»���': false,
                  'ɾ����': false,
                  '��������': 2,
                  '��ǰ��ҳ': false,
                  '��ǰ�����': 0,
                  '�о�': 0
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
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "����", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '����': 'heading 3',
                  'Ӣ������': 'Times New Roman',
                  '��������': '����',
                  '�����С': 'С��',
                  '���뷽ʽ': '�����',
                  '¼����': true,
                  '��ٵȼ�': 2,
                  '�Ӵ�': false,
                  'б��': false,
                  '�»���': false,
                  'ɾ����': false,
                  '��������': 2,
                  '��ǰ��ҳ': false,
                  '��ǰ�����': 0,
                  '�о�': 0
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
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "����", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '����': 'bodytext',
                  'Ӣ������': 'Times New Roman',
                  '��������': '����',
                  '�����С': 'С��',
                  '���뷽ʽ': '�����',
                  '¼����': false,
                  '��ٵȼ�': 0,
                  '�Ӵ�': false,
                  'б��': false,
                  '�»���': false,
                  'ɾ����': false,
                  '��������': 2,
                  '��ǰ��ҳ': false,
                  '��ǰ�����': 0,
                  '�о�': 1.5
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
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "����", ComplexScript = "Times New Roman" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '����': 'code',
                  'Ӣ������': 'Consolas',
                  '��������': '����',
                  '�����С': 'С��',
                  '���뷽ʽ': '�����',
                  '¼����': false,
                  '��ٵȼ�': 0,
                  '�Ӵ�': false,
                  'б��': false,
                  '�»���': false,
                  'ɾ����': false,
                  '��������': 0,
                  '��ǰ��ҳ': false,
                  '��ǰ�����': 0,
                  '�о�': 0
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
                        RunFonts = new RunFonts() { Ascii = "Consolas", HighAnsi = "Consolas", EastAsia = "����", ComplexScript = "Consolas" },
                        FontSize = new FontSize {  Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                }
            },
            new object[]
            {
                @"{
                  '����': 'reference',
                  'Ӣ������': 'Times New Roman',
                  '��������': '����',
                  '�����С': '���',
                  '���뷽ʽ': '�����',
                  '¼����': false,
                  '��ٵȼ�': 0,
                  '�Ӵ�': false,
                  'б��': false,
                  '�»���': false,
                  'ɾ����': false,
                  '��������': 0,
                  '��ǰ��ҳ': false,
                  '��ǰ�����': 0,
                  '�о�': 1.5
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
                        RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "����", ComplexScript = "Times New Roman" },
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
