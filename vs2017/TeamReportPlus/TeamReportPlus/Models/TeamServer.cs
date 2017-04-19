using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Models
{
    public class TeamServer
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }

        public IEnumerable<TeamProject> Projects { get; set; } 
    }
}
