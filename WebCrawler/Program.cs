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
using System.IO;
using OfficeOpenXml;
using WebCrawler.Models;

namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            WebCrawlAsync();
            Console.ReadLine();
        }
        private static void CreateSpreadSheet(List<JobModel> jobList)
        {
            string spreadSheetPath = "C://Users//Blaire//Desktop//JobList.xlsx";
            File.Delete(spreadSheetPath);
            FileInfo spreadSheetInfo = new FileInfo(spreadSheetPath);
            ExcelPackage pck = new ExcelPackage(spreadSheetInfo);

            var resultsWorkSheet = pck.Workbook.Worksheets.Add("Results");
            resultsWorkSheet.Cells["A1"].Value = "Company";
            resultsWorkSheet.Cells["B1"].Value = "Title";
            resultsWorkSheet.Cells["C1"].Value = "Location";
            resultsWorkSheet.Cells["D1"].Value = "Job Link";
            resultsWorkSheet.Cells["E1"].Value = "Company Link";
            resultsWorkSheet.Cells["F1"].Value = "isResponsive";
            resultsWorkSheet.Cells["A1:F1"].Style.Font.Bold = true;

            var linkText = "View";
            // Populate
            int currentRow = 2;
            foreach (var job in jobList)
            {
                resultsWorkSheet.Cells["A" + currentRow.ToString()].Value = job.Company;
                resultsWorkSheet.Cells["B" + currentRow.ToString()].Value = job.Title;
                resultsWorkSheet.Cells["C" + currentRow.ToString()].Value = job.Location;
                resultsWorkSheet.Cells["D" + currentRow.ToString()].Formula = "HYPERLINK(\"" + job.JobLink + "\",\"" + linkText + "\")";
                resultsWorkSheet.Cells["E" + currentRow.ToString()].Formula = "HYPERLINK(\"" + job.CompanyLink + "\",\"" + linkText + "\")";
                resultsWorkSheet.Cells["F" + currentRow.ToString()].Value = job.isResponsive;

                currentRow++;
            }

            // Fixed on scroll
            resultsWorkSheet.View.FreezePanes(2, 1);

            pck.Save();
        }

        private static async Task WebCrawlAsync()
        {
            var keyWord = "Full Stack";
            var location = "New York, NY";
            var radius = "0";
            string[] pages = { "0", "10" };

            // KEYWORD
            Console.WriteLine(String.Format("KeyWord: "));
            if (String.IsNullOrEmpty(keyWord))
            {
                keyWord = Console.ReadLine();
            }
            else
            {
                Console.WriteLine(keyWord);
            }
            // LOCATION
            Console.WriteLine(String.Format("Location: "));
            if (String.IsNullOrEmpty(location))
            {
                location = Console.ReadLine();
            }
            else
            {
                Console.WriteLine(location);
            }
            // RADIUS
            Console.WriteLine(String.Format("Radius: "));
            if (String.IsNullOrEmpty(radius))
            {
                radius = Console.ReadLine();
            }
            else
            {
                Console.WriteLine(radius);
            }


            List<HtmlNode> nodes;
            List<JobModel> JobList = new List<JobModel>();

            var webGet = new HtmlWeb();
            Console.WriteLine(String.Format("INDEED LINK: https://www.indeed.com/jobs?q={0}&l={1}&radius={2}&start=0", keyWord, location, radius));
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
                        JobModel newJob = new JobModel();

                        var company = node.CssSelect(".company").FirstOrDefault().InnerText.ToString().Trim();
                        if (!String.IsNullOrEmpty(company))
                        {
                            Console.WriteLine(String.Format("COMPANY: {0}", company));
                            newJob.Company = company;
                        }
                        var job = node.CssSelect(".jobtitle").FirstOrDefault().InnerText.ToString().Trim();
                        if (!String.IsNullOrEmpty(job))
                        {
                            Console.WriteLine(String.Format("JOB: {0}", job));
                            newJob.Title = job;
                        }
                        var loc = node.CssSelect(".location").FirstOrDefault().InnerText.ToString().Trim();
                        if (!String.IsNullOrEmpty(loc))
                        {
                            Console.WriteLine(String.Format("LOCATION: {0}", loc));
                            newJob.Location = loc;
                        }

                        string lnk = String.Format("https://www.indeed.com/cmp/{0}/jobs", company);
                        if (!String.IsNullOrEmpty(company))
                        {
                            Console.WriteLine(String.Format("COMPANY LINK: {0}", lnk));
                            newJob.CompanyLink = lnk;
                        }

                        if (node.CssSelect(".jobtitle").FirstOrDefault().Attributes["href"] != null)
                        {
                            var link = String.Format("https://www.indeed.com/{0}", node.CssSelect(".jobtitle").FirstOrDefault().Attributes["href"].Value);
                            if (!String.IsNullOrEmpty(link))
                            {
                                Console.WriteLine(String.Format("JOB LINK: {0}", link));
                                newJob.JobLink = link;
                            }
                        }




                        var resp = "FALSE";
                        if (node.CssSelect(".serp-ResponsiveEmployer").FirstOrDefault() != null)
                        {
                            resp = "TRUE";
                            Console.WriteLine(String.Format("RESPONSIVE: {0}", resp));
                            newJob.isResponsive = resp;
                        }


                        // TEST

                        Console.WriteLine(Environment.NewLine);

                        JobList.Add(newJob);
                    }
                }
            }
            CreateSpreadSheet(JobList);
        }
    }
}
