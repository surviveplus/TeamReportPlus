using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Models
{
    public class TestCaseResult
    {
        public int TestCaseId { get; set; }

        public string RequirementTitle { get; set; }

        public string TestCaseTitle { get; set; }

        public string Descriptions { get; set; }

        public int? TestRunId { get; set; }

        public string Outcome { get; set; }

        public int SuiteId { get; set; }

        public string RunByName { get; set; }

        public string ComputerName { get; set; }

        public DateTime? DateStarted { get; set; }

        public TimeSpan? Duration { get; set; }

        public string AnalysisOwnerName { get; set; }

        public string Comment { get; set; }
    }
}
