using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.ViewModels
{
    public class TestPlanListItem
    {
        public string ProjectName { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }

        public ITestPlan Plan { get; set; }

        public static TestPlanListItem FromITestPlan(ITestPlan plan) {
            return new TestPlanListItem
            {
                Id = plan.Id,
                ProjectName = plan.Project.TeamProjectName,
                Name = plan.Name,
                Plan = plan
            };
        }

    }
}
