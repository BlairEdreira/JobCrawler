using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Models
{
    class JobModel
    {
        public string Company { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string JobLink { get; set; }
        public string CompanyLink { get; set; }
        public string isResponsive { get; set; }
    }
}
