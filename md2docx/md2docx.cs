using System;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;

namespace md2docx
{
    class Md2Docx
    {
        static void Main(String[] args)
        {
            string md = System.IO.File.ReadAllText("test.md");
            Console.WriteLine(md);

            MarkdownDocument mddoc = new MarkdownDocument();
            mddoc.Parse(md);

            string name = "";
            string id = "";
            string teacher = "";
            string department = "";
            string title = "";
            string clas = "";

            foreach (var element in mddoc.Blocks)
            {
                if (element is YamlHeaderBlock yaml)
                {
                    name = yaml.Children["name"];
                    id = yaml.Children["id"];
                    teacher = yaml.Children["teacher"];
                    department = yaml.Children["department"];
                    title = yaml.Children["title"];
                    clas = yaml.Children["class"];
                }
                else if (element is ParagraphBlock para)
                {
                    foreach (var e in para.Inlines)
                    {
                        Console.WriteLine($"Para({e.GetType()}) {e.ToString()}");
                    }
                }
                else if (element is ListBlock list) //我就操了为什么list里面可以放block，我丢
                {
                    foreach (var e in list.Items)
                    {
                        Console.WriteLine($"wtf({e.GetType()}) {e.Blocks[0].ToString()}");
                    }
                }
                else if (element is HeaderBlock head)
                {
                    Console.Write($"H({head.HeaderLevel},)");
                    foreach (var e in head.Inlines)
                    {
                        Console.Write($"[{e.GetType()}, {e.ToString()}]");
                    }
                    Console.WriteLine("");
                }
            }
            Console.WriteLine($"name {name}");
            Console.WriteLine($"id {id}");
            Console.WriteLine($"teacher {teacher}");
            Console.WriteLine($"department {department}");
            Console.WriteLine($"title {title}");
            Console.WriteLine($"class {clas}");
        }
    }
}
