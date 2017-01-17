using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Models
{
    public class TestStepResult
    {
        public int TestCaseId { get; set; }

        public string TestCaseTitle { get; set; }

        public int TestRunId { get; set; }

        public int StepNo { get; set; }

        public string Outcome { get; set; }

        public string Action { get; set; }

        public string ExpectedResult { get; set; }

        public string Comment { get; set; }

        public string Attachments { get; set; }
    }
}
