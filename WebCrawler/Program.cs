using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp;
using ScrapySharp.Network;
using ScrapySharp.Cache;
using ScrapySharp.Core;
using ScrapySharp.Exceptions;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            WebCrawlAsync();
            Console.ReadLine();
        }

        private static async Task WebCrawlAsync()
        {
            // INPUTS
            var keyWord = "";
            var location = "New York";
            var radius = "";
            string[] pages = { "0", "10" };
            // INPUTS

            Console.WriteLine(String.Format("KeyWord: "));
            if (String.IsNullOrEmpty(keyWord))
                keyWord = Console.ReadLine();
            Console.WriteLine(String.Format("Location: "));
            if (String.IsNullOrEmpty(location))
            {
                location = Console.ReadLine();
            }
            else
            {
                Console.WriteLine(location);
            }
            Console.WriteLine(String.Format("Radius: "));
            if (String.IsNullOrEmpty(radius))
                radius = Console.ReadLine();


            List<HtmlNode> nodes;
            var webGet = new HtmlWeb();

            for (int i = 0; i < pages.Length; i++)
            {
                var url = String.Format("https://www.indeed.com/jobs?q={0}&l={1}&radius={2}&start={3}", keyWord, location, radius, pages[i]);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(String.Format("----- INDEED {0} -----", pages[i].Replace("0", "")));
                Console.WriteLine(Environment.NewLine);
                if (webGet.Load(url) is HtmlDocument document)
                {
                    nodes = document.DocumentNode.CssSelect("#resultsCol .row").ToList();
                    foreach (var node in nodes)
                    {
                        var company = node.CssSelect(".company").FirstOrDefault().InnerText.ToString().Trim();
                        if (!String.IsNullOrEmpty(company))
                        {
                            Console.WriteLine(String.Format("COMPANY: {0}", company));
                        }
                        var job = node.CssSelect(".jobtitle").FirstOrDefault().InnerText.ToString().Trim();
                        if (!String.IsNullOrEmpty(job))
                        {
                            Console.WriteLine(String.Format("JOB: {0}", job));
                        }
                        var loc = node.CssSelect(".location").FirstOrDefault().InnerText.ToString().Trim();
                        if (!String.IsNullOrEmpty(loc))
                        {
                            Console.WriteLine(String.Format("LOCATION: {0}", loc));
                        }

                        Console.WriteLine(Environment.NewLine);
                    }
                }
            }





        }


    }
}
