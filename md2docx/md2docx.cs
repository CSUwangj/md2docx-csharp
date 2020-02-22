using System;
using System.IO;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using A = DocumentFormat.OpenXml.Drawing;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using NDesk.Options;

namespace md2docx
{
    class Md2Docx
    {
        static Dictionary<string, string> info = new Dictionary<string, string>();
        static Dictionary<string, string> correspondecs;
        static Dictionary<string, bool> optionalParts;
        static Dictionary<int, byte[]> imageDatas = new Dictionary<int, byte[]>();
        static Dictionary<int, string> imageType = new Dictionary<int, string>();
        static List<int> failImage = new List<int>();
        static int imageCount = 1;
        static bool hasFailImage = false;

        /// <summary>
        /// Print usage without exit
        /// </summary>
        private static void Usage(OptionSet p)
        {
            Console.WriteLine(@"Usage: md2docx [OPTIONS]
Convert markdown to docx with specified options.
Opntions:");
            p.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">{markdown} {config_json} {output}</param>
        private static void Main(string[] args)
        {
            string docPath = "";
            string mdPath = "input.md";
            string configPath = "config.json";
            bool showHelp = false;
            int useDefault = 3;
            bool quiet = false;
            var p = new OptionSet
            {
                {   "i|input=", "the {INPUT} markdown file path. Default value is input.md.",
                    v => { mdPath = v; useDefault -= 1; } },
                {   "o|output=", "the {OUTPUT} docx file path. Default value is <id><name><filename>.docx.",
                    v => { docPath = v; useDefault -= 1; } },
                {   "c|config=", "the {CONFIG} json path. Default value is config.json.",
                    v => { configPath = v; useDefault -= 1; } },
                {   "h|help", "show this message and exit",
                    v => showHelp = v != null },
                {   "q|quiet", "ignore missing args message",
                    v => quiet = v != null }
            };
            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("md2docx: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try`md2docx --help' for more information.");
                return;
            }

            if (showHelp)
            {
                Usage(p);
                return;
            }

            if (useDefault != 0 && !quiet)
            {
                Console.WriteLine("Some arguments are missing, will use default value for you.");
                Console.WriteLine("Try`md2docx --help' for more information or use -q stop this message.");
            }

            string md = System.IO.File.ReadAllText(mdPath);
            JObject config = JObject.Parse(System.IO.File.ReadAllText(configPath));
            correspondecs = config["对应关系"].ToObject<Dictionary<string, string>>();
            optionalParts = config["可选部分"].ToObject<Dictionary<string, bool>>();
            MarkdownDocument mddoc = new MarkdownDocument();
            mddoc.Parse(md);

            foreach (var element in mddoc.Blocks)
            {
                if (element is YamlHeaderBlock yaml)
                {
                    info = yaml.Children;
                }
                if (docPath == "")
                {
                    docPath = info["id"] + info["name"] + info["filename"] + ".docx";
                }
            }

            using (WordprocessingDocument document = WordprocessingDocument.Create(docPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainDocumentPart1 = document.AddMainDocumentPart();
                GenerateMainDocumentPart1Content(mainDocumentPart1, mddoc);
              
                StyleDefinitionsPart styleDefinitionsPart1 = mainDocumentPart1.AddNewPart<StyleDefinitionsPart>("rId1");
                GenerateStyleDefinitionsPart1Content(styleDefinitionsPart1, (JArray)config["样式"]);

                FontTablePart fontTablePart1 = mainDocumentPart1.AddNewPart<FontTablePart>("rId0");
                GeneratedCode.GenerateFontTablePartContent(fontTablePart1);

                SetPackageProperties(document);

                foreach(int index in imageType.Keys)
                {
                    ImagePart imagePart = mainDocumentPart1.AddNewPart<ImagePart>($"image/{imageType[index]}", $"image{index}");
                    imagePart.FeedData(new MemoryStream(imageDatas[index]));
                }
            }

            if (hasFailImage && !quiet)
            {
                Console.WriteLine("Some image failed to insert, please check and insert it manually. Their index are(count from 1):");
                foreach(int index in failImage)
                {
                    Console.Write($"{index} ");
                }
                Console.WriteLine("\nThis warning can also be closed by -q.");
            }
        }
        
        /// <summary>
        /// Generate document body
        /// </summary>
        /// <param name="mainDocumentPart1">main body</param>
        /// <param name="document">word document</param>
        /// <param name="correspondecs">how markdown blocks are mapping to styles</param>
        /// <param name="optionalParts">double check if user need these parts</param>
        private static void GenerateMainDocumentPart1Content(MainDocumentPart mainDocumentPart1, MarkdownDocument document)
        {
            Document document1 = new Document() { MCAttributes = new MarkupCompatibilityAttributes() };

            Body docBody = new Body();

            if (optionalParts["封面"])
            {
                GenerateCoverImage(mainDocumentPart1.AddNewPart<ImagePart>("image/jpeg", "rId2"));
                GeneratedCode.GenerateCover(ref docBody, info);
            }

            if (optionalParts["摘要"] && info.ContainsKey("c_title"))
            {
                AddAbstract(info["c_title"], info["c_abs"], info["c_kew"], true, ref docBody);
            }
            if (optionalParts["摘要"] && info.ContainsKey("e_title"))
            {
                AddAbstract(info["e_title"], info["e_abs"], info["e_kew"], false, ref docBody);
            }

            if (optionalParts["目录"])
            {
                GeneratedCode.GenerateTOC(ref docBody);
            }

            // rendering body text(paragraph/heading, others are TBD)
            foreach (var block in document.Blocks)
            {
                CovertMarkdownBlock(block, ref docBody);
            }

            SectionProperties sectionProperties1 = new SectionProperties();
            PageSize pageSize1 = new PageSize() { Width = 11906U, Height = 16838U };
            PageMargin pageMargin1 = new PageMargin() { Top = 1418, Right = 1134U, Bottom = 1418, Left = 1701U, Header = 851U, Footer = 992U, Gutter = 0U };
            Columns columns1 = new Columns() { Space = "425" };
            DocGrid docGrid1 = new DocGrid() { Type = DocGridValues.Lines, LinePitch = 312 };

            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(docGrid1);
            
            docBody.Append(sectionProperties1);

            document1.Append(docBody);

            mainDocumentPart1.Document = document1;
        }

        /// <summary>
        /// As name said
        /// </summary>
        /// <param name="imagePart1">image</param>
        private static void GenerateCoverImage(ImagePart imagePart1)
        {
            System.IO.Stream data = GetBinaryDataStream(CoverImageData);
            imagePart1.FeedData(data);
            data.Close();
        }

        /// <summary>
        /// Covert MarkdonwInlines using dfs
        /// </summary>
        /// <param name="rp">Current Run Properties, because special style may be nesting so we need keep it</param>
        /// <param name="inline">Current Inline element</param>
        /// <param name="paragraph">In which we append out text</param>
        private static void CovertMDInlines(RunProperties rp, IList<MarkdownInline> inlines, ref Paragraph paragraph,ref List<Paragraph> paragraphs)
        {
            foreach(MarkdownInline inline in inlines)
            {
                switch (inline)
                {
                    case TextRunInline mtxt:
                        RunProperties newtrp = (RunProperties)rp.Clone();
                        Run trun = new Run();
                        Text dtext = new Text { Text = mtxt.Text, Space = SpaceProcessingModeValues.Preserve };
                        trun.Append(newtrp);
                        trun.Append(dtext);
                        paragraph.Append(trun);
                        break;
                    case CodeInline mcode:
                        RunProperties newcrp = (RunProperties)rp.Clone();
                        newcrp.RunFonts = new RunFonts() { Ascii = "Consolas", HighAnsi = "Consolas" };
                        Run crun = new Run();
                        Text dcode = new Text { Text = mcode.Text };
                        crun.Append(newcrp);
                        crun.Append(dcode);
                        paragraph.Append(crun);
                        break;
                    case BoldTextInline bd:
                        RunProperties newbrp = (RunProperties)rp.Clone();
                        newbrp.Bold = new Bold();
                        newbrp.BoldComplexScript = new BoldComplexScript();
                        CovertMDInlines(newbrp, bd.Inlines, ref paragraph, ref paragraphs);
                        break;
                    case ItalicTextInline it:
                        RunProperties newirp = (RunProperties)rp.Clone();
                        newirp.Italic = new Italic();
                        newirp.ItalicComplexScript = new ItalicComplexScript();
                        CovertMDInlines(newirp, it.Inlines, ref paragraph, ref paragraphs);
                        break;
                    case StrikethroughTextInline st:
                        RunProperties newstrp = (RunProperties)rp.Clone();
                        newstrp.Strike = new Strike();
                        CovertMDInlines(newstrp, st.Inlines, ref paragraph, ref paragraphs);
                        break;
                    case SubscriptTextInline sb:
                        RunProperties newsbrp = (RunProperties)rp.Clone();
                        newsbrp.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Subscript };
                        CovertMDInlines(newsbrp, sb.Inlines, ref paragraph, ref paragraphs);
                        break;
                    case SuperscriptTextInline sp:
                        RunProperties newsprp = (RunProperties)rp.Clone();
                        newsprp.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript };
                        CovertMDInlines(newsprp, sp.Inlines, ref paragraph, ref paragraphs);
                        break;
                    case ImageInline img:
                        ParagraphProperties newpp = (ParagraphProperties)paragraph.ParagraphProperties.Clone();
                        paragraphs.Add(paragraph);
                        paragraph = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                ParagraphStyleId = new ParagraphStyleId { Val = "Image Title" }
                            }
                        };
                        ImageGetter image = new ImageGetter();
                        if (!image.Load(img.Url))
                        {
                            failImage.Add(imageCount);
                            hasFailImage = true;
                        }
                        imageType.Add(imageCount, image.Type);
                        imageDatas.Add(imageCount, image.ImageData);
                        Run run = GeneratedCode.GenerateImageReference(imageCount, image.Width, image.Height);
                        paragraph.Append(run);
                        paragraphs.Add(paragraph);
                        imageCount += 1;
                        paragraph = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                ParagraphStyleId = new ParagraphStyleId { Val = "Image Title" }
                            }
                        };
                        run = new Run();
                        Text txt = new Text { Text = img.Tooltip, Space = SpaceProcessingModeValues.Preserve };
                        run.Append(txt);
                        paragraph.Append(run);
                        paragraphs.Add(paragraph);
                        paragraph = new Paragraph { ParagraphProperties = newpp };
                        break;
                    default:
                        Console.WriteLine(inline.ToString());
                        throw new Exception($"Rendering {inline.GetType()} not implement yet");
                }
            }
        }

        /// <summary>
        /// Currently we only convert quote block
        /// </summary>
        /// <param name="block">Paragraph block, when block is not paragraph block, it throw a exception</param>
        /// <param name="docBody">In which we append our text</param>
        private static List<Paragraph> CovertQuoteRefer(QuoteBlock refer)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();

            foreach (MarkdownBlock block in refer.Blocks)
            {

                if (!(block is ParagraphBlock))
                {
                    throw new Exception($"Rendering {block.GetType()} in reference in quote not support");
                }
                Paragraph paragraph = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = correspondecs["引用"] }
                    }
                };
                Run run = new Run { RunProperties = new RunProperties() };
                Text txt = new Text { Text = block.ToString(), Space = SpaceProcessingModeValues.Preserve };
                run.Append(txt);
                paragraph.Append(run);
                paragraphs.Add(paragraph);
            }

            return paragraphs;
        }

        private static List<Paragraph> CovertCodeBlock(CodeBlock code)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();
            string[] lines = code.Text.Split('\n');

            foreach (string line in lines)
            {
                Paragraph paragraph = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = correspondecs["代码段"] }
                    }
                };
                Run run = new Run { RunProperties = new RunProperties() };
                Text txt = new Text { Text = line, Space = SpaceProcessingModeValues.Preserve };
                run.Append(txt);
                paragraph.Append(run);
                paragraphs.Add(paragraph);
            }

            return paragraphs;
        }

        private static void CovertMarkdownBlock(MarkdownBlock block, ref Body docBody)
        {

            if (block is ParagraphBlock mpara)
            {
                Paragraph docPara = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = correspondecs["正文"] }
                    }
                };
                List<Paragraph> paragraphs = new List<Paragraph>();
                CovertMDInlines(new RunProperties(), mpara.Inlines, ref docPara, ref paragraphs);
                docBody.Append(docPara);
                foreach (Paragraph paragraph in paragraphs)
                {
                    docBody.Append(paragraph);
                }
            }
            else if (block is CodeBlock mcode)
            {
                List<Paragraph> paragraphs = CovertCodeBlock(mcode);
                foreach(Paragraph paragraph in paragraphs)
                {
                    docBody.Append(paragraph);
                }
            }
            else if (block is HeaderBlock mhead)
            {
                Paragraph docPara = new Paragraph { ParagraphProperties = new ParagraphProperties() };
                List<Paragraph> paragraphs = new List<Paragraph>();
                switch (mhead.HeaderLevel)
                {
                    case int i when i < 10:
                        docPara.ParagraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = correspondecs[chineseNumber[i] + "级标题"] };
                        break;
                    default:
                        throw new Exception($"Rendering {block.GetType()} not implement yet");
                }
                CovertMDInlines(new RunProperties(), mhead.Inlines, ref docPara, ref paragraphs);
                paragraphs.Add(docPara);
                foreach (Paragraph paragraph in paragraphs)
                {
                    docBody.Append(paragraph);
                }
            }
            else if (block is QuoteBlock refer)
            {
                List<Paragraph> paragraphs = CovertQuoteRefer(refer);
                foreach (Paragraph paragraph in paragraphs)
                {
                    docBody.Append(paragraph);
                }
            }
            else if (!(block is YamlHeaderBlock))
            {
                throw new Exception($"Rendering {block.GetType()} not implement yet");
            }
        } 
        /// <summary>
        /// Generate abstract and add it to document
        /// </summary>
        /// <param name="title">Abstract title</param>
        /// <param name="abs">Abstract body</param>
        /// <param name="keyWords">Key words</param>
        /// <param name="isCN">If it's Chinese abstract</param>
        /// <param name="docBody">In which we append our text</param>
        private static void AddAbstract(string title, string abs, string keyWords, bool isCN, ref Body docBody)
        {
            string subtitle = isCN ? "摘要" : "ABSTRACT";
            string keyWT = isCN ? "关键词：" : "Key words: ";
            Paragraph para = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId() { Val = "Abstract Title" }
                }
            };
            Run run = new Run { RunProperties = new RunProperties() };
            if (!isCN)
            {
                run.RunProperties.Append(new Bold());
                run.RunProperties.Append(new BoldComplexScript());
            }
            Text txt = new Text { Text = title, Space = SpaceProcessingModeValues.Preserve };
            run.Append(txt);
            para.Append(run);
            docBody.Append(para);


            para = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = "Abstract Subtitle" }
                }
            };
            run = new Run
            {
                RunProperties = (RunProperties)run.RunProperties.Clone()
            };
            txt = new Text { Text = subtitle, Space = SpaceProcessingModeValues.Preserve };
            run.Append(txt);
            para.Append(run);
            docBody.Append(para);

            para = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = correspondecs["正文"] }
                }
            };
            run = new Run { RunProperties = new RunProperties() };
            txt = new Text { Text = abs, Space = SpaceProcessingModeValues.Preserve };
            run.Append(txt);
            para.Append(run);
            docBody.Append(para);

            para = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = correspondecs["正文"] }
                }
            };
            run = new Run { RunProperties = new RunProperties() };
            txt = new Text { Text = "", Space = SpaceProcessingModeValues.Preserve };
            run.Append(txt);
            para.Append(run);
            docBody.Append(para);

            para = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = correspondecs["正文"] }
                }
            };
            run = new Run
            {
                RunProperties = new RunProperties
                {
                    Bold = new Bold(),
                    BoldComplexScript = new BoldComplexScript()
                }
            };
            txt = new Text { Text = keyWT, Space = SpaceProcessingModeValues.Preserve };
            run.Append(txt);
            para.Append(run);
            run = new Run { RunProperties = new RunProperties() };
            txt = new Text { Text = keyWords, Space = SpaceProcessingModeValues.Preserve };
            run.Append(txt);
            para.Append(run);
            docBody.Append(para);
        }
        
        /// <summary>
        /// Generate styles from config json object
        /// </summary>
        /// <param name="styleDefinitionsPart1">Styles object</param>
        /// <param name="styleConfig">Config json object</param>
        /// <param name="latent">If user need latent style</param>
        private static void GenerateStyleDefinitionsPart1Content(StyleDefinitionsPart styleDefinitionsPart1, JArray styleConfig)
        {
            Styles styles = new Styles() { MCAttributes = new MarkupCompatibilityAttributes() };

            DocDefaults docDefaults = new DocDefaults
            {
                RunPropertiesDefault = new RunPropertiesDefault
                {
                    RunPropertiesBaseStyle = new RunPropertiesBaseStyle
                    {
                        RunFonts = new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "宋体", ComplexScript = "Times New Roman" },
                        Kern = new Kern { Val = 2U },
                        Languages = new Languages { Val = "en-US", EastAsia = "zh-CN", Bidi = "ar-SA" },
                        FontSize = new FontSize { Val = "24" },
                        FontSizeComplexScript = new FontSizeComplexScript { Val = "24" }
                    }
                },
                ParagraphPropertiesDefault = new ParagraphPropertiesDefault()
            };

            styles.Append(docDefaults);

            if (optionalParts["延迟样式"])
            {
                styles.Append(GeneratedCode.GenerateLatentStyles());
            }

            StyleFactory styleFactory = new StyleFactory();
            foreach(JObject jObject in styleConfig)
            {
                Style style = styleFactory.GenerateStyle(jObject);
                styles.Append(style);
            }

            styleDefinitionsPart1.Styles = styles;
        }

        /// <summary>
        /// Set document's properties like title, creator, etc.
        /// </summary>
        /// <param name="document">Document file</param>
        private static void SetPackageProperties(OpenXmlPackage document)
        {
            document.PackageProperties.Creator = info["name"];
            document.PackageProperties.Title = info["filename"];
            document.PackageProperties.Revision = "3";
            document.PackageProperties.Created = DateTime.Now;
            document.PackageProperties.Modified = DateTime.Now;
            document.PackageProperties.LastModifiedBy = "md2docx_by_CSUwangj";
        }

        #region Binary Data
        private static readonly string CoverImageData = "/9j/4AAQSkZJRgABAQEAlgCWAAD/2wBDAAoHBwgHBgoICAgLCgoLDhgQDg0NDh0VFhEYIx8lJCIfIiEmKzcvJik0KSEiMEExNDk7Pj4+JS5ESUM8SDc9Pjv/2wBDAQoLCw4NDhwQEBw7KCIoOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozv/wAARCAFwBXoDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD2aiiigAooooAKKKKACiiigAooooAKKKKACiikoAWkpk00UEZlmkSNF6s7AAfjXMan8RdB08lIZXvZB2gHy/8AfR4qowlL4URKcY/EzqqK8zk+IHiPWHMeiaVsX+8qGVh+PQVk3w1y5P8AxPPEcFqD/wAsnudzf98R1usNL7TsYPEx+yrnq13rOmWOftWoW0OOzygH8qx7j4g+GrfOL/zj/wBMo2b+leeWnh6wnOYY9Y1NvW3tPKQ/8Cetm38H3bAGDwmij+9f6gSfyXFX7GlHd/oR7arLZfqbU3xT0RM+VbXkn/AAv8zVST4s2Y/1elTN/vSqP8afD4M1cY22nh60/wB21aU/mxq3H4Q1of8AMYsYvaLTYx/Oi1Bf8OF8Q/8AhjM/4W3F20Y/+BI/+JpV+LcBPzaO4+lwD/Stb/hEdY/6GTH0sY6Q+EdZxx4gib/f0+M0XodvxYf7R3/BFOP4r6Yx/eafdJ/ulW/rV2D4meHpSA7XMOf78PH6ZqtJ4N1o/wDL1o1wPSbTgP5VQn8GamPv+H9EuPe3lkhP88UctB/8OHNXX/DHWWvi/wAPXhAi1a33H+F22H9a1opop03xSJIvqjAivKLrwqseftHhvVrb1a1nS4UfgRms0adbWsv+g+IHspeyXkUls35jIo9hB/C/6+QfWJr4l/XzPbKK8rttY8caXH5iONTtx/GpW4XH1XkVqaf8VIC3larp8kDDhnhO4D/gJ5FZvDz6amqxMHvoegUtZmmeItJ1gD7DfRSsf+WecOP+AnmtKsGmtGbpp6oWiiikMKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKSgBaSo7i5htIHnuJUiiQZZ3OAK4LW/iQ8032Hw5btNK52idkJyf9le/1NXCnKexnOpGC1O21HVbDSbcz391HAnbceT9B1NcNqfxLnupvsnh6xeSRuFkkQsx+iD+tc6+ly3N+H127uL3UJOljanzJv+BH7sYrrdK8IajLDsmaPRLNuttZHMzj/blPP5V0qnTp6y1/I5nUqVHaOhyV9a6hfTq/iXWTHIx+W1X99MfYRrwv41saX4UupdrafoKQr2utXbc31EQ4H413mleHtK0VMWNnHG56yEbnb6seadqevaVowB1C9jhJ6ITlj+A5pOvJ6RQ1h4rWbMWLwSbhANY1e7u1H/LCEiCEe21a2LDw5o2mAfY9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FEhd3YKqqMliegFAF3RtJm1vU47KE7Q3zSSHpGg6sa623sbHxfeyWdnqgsbfTEEWnRKfmcj70hHfJ9OapXNvJoGjy6RZwyXF5IqyatNAM+TGekQPb3Na8Gi+FvF9qkuizHTb+FB8i8MMDqV7/UVxVJ397odtOFvd6jm1TxZ4TUxaxaDV9OHBmX5iB7n/AOKH40R6V4M8YfvNOl/s6+POxPkYH/d6H8KE8QeJPCDi38Q2rX9j91bpOTj69/ocH3q1J4d8LeMYTeaROLW56loPlKn/AGk/qKx21enmtvuNt9Fr5Pcr+R448Lf6mRdZsl/hOWcD6dR+Ga0tK+I2k3j+RqCyabcDhlmHy5+vb8QKzBfeMfB3F7D/AGvp6f8ALVSSyj69R+OfrTdb8ReG/EuiMYLNZ9UlIighdMSB24ByOoFJx5t1fzQ1Ll2dvJkPibXVuNRutUhkDQachtbIg5D3Dj5nHrtX+lavwz0X7Foz6jKuJb05XPURjp+Zyfyrj100avrth4Zsn3WtllZZF6M3WV/z4H0FewwxRwQpDEoWONQqqOwHSis1CCguoUU5zc30JKKKK4ztCiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKAGSxpLE8cihkcFWU9CD1FeP3mnp4Z8Q3WjXxI0y/XaHP8Kk/I491P9a9jrm/GvhweINHbylH2y3y8J/veq/j/ADxW9Cooys9mc9enzRut0c/ZeM4fDekLpMmnFtVt2MRigTakn918jrkYNLH4d8SeL5FuPEN01jZZytpGMEj6dvqcmsLQtVm3QXsab9S0lSskbD5ri2/iH+8n8vpW5NrviHxrK1roMLWOn5xJdPwSPr/Qc+9dEouL93TuznjNSVpa9kXrzW/D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vKgfyjZrhcjpnNR+E9b1yfxDqGi6w8U7Wig+bGoGDkccdcg1qwzp4Z8J+Zei2jNnEd6W+Qm7sBnnnI/OqXgTTJ4NMm1W9B+2apJ5756hT90f1/GouuWV16FpPmik/U6qkpaayhlKkZBGCK5zpOe1Txzoumym3WV7y5Bx5Nsu859M9K5HwpbeKb6yu30W6t7GymuXYmUbmVu4HHbitvX4dJ8F6a76PYImpXzGKDaCzAnqRnpjPQd8Vs+EdGk0bw1BZ3GfPfdJNzyGbkjPtXUpRhTulv3ORqU6lpPbsN8M+G5dDNxcXeoy313dY8x3JwMegP1rerzjU9Q8ReBL7JmfUdJkbMZnOSv+yW6g/oa9Asbpb6xgu0VlWaNXCsMEAjPNZVIy+Ju9zWnKPwpWsWKz9Y0Wy1y0W1vo2eNJBIu1sEEe9XyayNR8VaLpj+XPfI83aGH945P0FRFSv7ppJxt7xqSq5gdYSFfaQhIyAccV55D4O1OOyaDXL6wt7H7T9puZwSZZiOxY4wK09S8X6kYt0NtBo8B6XGpN85HqsQ5P41x17r1vc3SkJc67eZ+SS8GIlP8AsQr/AFrppU5rY5atSD3O4vPGRnicaFbLLFHw1/dHy7aP8Ty30FcTf68k16rxvJrmpE4jmmj/AHMR/wCmUXf6mtKz8GeI/EkiT63ctaQD7sbAZUeioOFrutE8MaVoEeLK3HmkYaZ/mdvx/wAKd6dLbV/11Fy1au+i/rocZpHgHUdYuf7R8TXEo3c+SWzIw9Cf4R7D9K9Cs7K20+2S2tIEghT7qIMCp6WuedSU9zop0ow2EpaKKzNQooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigApkkaSxmORFdG4KsMg0+igDk9W+HWiajue3RrGU94fun6qePyxXNS+EfF3h8N/ZV39rt+8aNwR7xtwa9QoraNecdHr6mEqEJO60fkeLPqi2dz/wATTQnsbnPM1kzWr/XH3TW1Y+MJowBa+JW/65ara5/8iJXpc9tBcxmOeGOVD1V1DD9awL3wD4cvct9h+zsf4oGKfp0rX21OXxIy9hUj8LKdp4u1VwC2mWd+v9/T71WP/fLYNLH4j0u21WXUrzS9Vs7qWJYnaW2YqFBJGMZ9azLv4UW5Jay1SWM9hLGG/UYqp/wg3i/Tx/oGsBgOgWd0/Q8U+Wi9mLmrLdFi9PhHVtc+2zeIfLtncSzWMmUSSQDAJz7da7GLxHoTqBHq1kR2AmX/ABrgpNN+IcIxIguVH94xSfzqpJB4tX/XeGreU+p0+Nv5U3SUvtfiJVZRv7v4Hp41nSz01K0/7/L/AI019d0hB82qWY+s6/415b5XiDv4Otif+wYacsXiQ8J4Stk/7hgH86n6uu4/rMu35nok/inwyjq82rWJdOVO8MV+npVdvHeg5xBPPdH0t7d3/pXGRWnjhjiDSYrf3W2hT+dWB4e+IN5xNfNCp/6eQv6KKPYwW8vxD21R7R/A6W48VTXEREHhq9lib+K7CwIfruNZN74x1JQVk1HRtNA/hjZrqQfgvFU4/hhqd02/UdZUk9cBpD+ZNa9l8L9Et8G5lubkjsWCL+Q/xo/cR63D9/LpY5G/8TWlwSLm81XVif4HkFtCf+Aryak0+PxRqC7NE0hNMgb+OGLys/WRvmNemWHh3R9Mx9j06CIj+IJlvzPNaNDrxXwx+8aw8n8UvuPPNO+F7zSfaNb1J5HblkhJJP1c/wCFdnpWgaXoybbCyjiOOXxlj9Sea0KWsJ1Zz3ZvClCGyCiiiszUKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/2Q==";

        private static readonly Dictionary<int, string> chineseNumber = new Dictionary<int, string>
        {
            { 1, "一" },
            { 2, "二" },
            { 3, "三" },
            { 4, "四" },
            { 5, "五" },
            { 6, "六" },
            { 7, "七" },
            { 8, "八" },
            { 9, "九" }
        };

        /// <summary>
        /// Get Binary Data from base64 string
        /// </summary>
        /// <param name="base64String">base64 string</param>
        /// <returns>Binary stream data</returns>
        private static System.IO.Stream GetBinaryDataStream(string base64String)
        {
            return new System.IO.MemoryStream(System.Convert.FromBase64String(base64String));
        }

        #endregion

    }
}
