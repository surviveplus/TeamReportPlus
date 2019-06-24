using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamReportPlus.Models;
using TeamReportPlus.Properties;

namespace TeamReportPlus.Core
{
    public class TeamClient
    {
        public static IEnumerable<TeamServer> GetServersWithPicker()
        {

            IEnumerable<TeamServer> servers = new TeamServer[] { };
            using (var p = new Microsoft.TeamFoundation.Client.TeamProjectPicker(Microsoft.TeamFoundation.Client.TeamProjectPickerMode.MultiProject, false))
            {
                if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var server = new TeamServer { Uri = p.SelectedTeamProjectCollection.Uri, Name = p.SelectedTeamProjectCollection.DisplayName };
                    server.Projects = (from project in p.SelectedProjects orderby project.Name select new TeamProject { Name = project.Name });

                    servers = server.Singleton();
                } // end if
            } // end using(p)

            return servers;
        } // end function

        public static IEnumerable<ITestPlan> GetTestPlans( IEnumerable<TeamServer> servers)
        {
            var plans =
                (from server in servers
                 from project in server.Projects
                 from p in TfsTeamProjectCollectionFactory.GetTeamProjectCollection(server.Uri).AllTestManagementTeamProjects()
                 where p.WitProject.Name == project.Name
                 from plan in p.AllTestPlans()
                 select plan).ToList();

            return plans;
        } // end function

        public static IEnumerable<TestCaseResult> GetTestCaseResults(IEnumerable<ITestPlan> plans)
        {
            var results =
            from plan in plans
            from suite in plan.AllTestSuiteEntries()
            from point in suite.AllTestPoints()
            let outcome = (point.MostRecentResult != null) ? point.MostRecentResult.Outcome.ToString() : point.State.ToString()
            let runByName = (point.MostRecentResult != null) ? point.MostRecentResult.RunByName : point.AssignedToName
            select new TestCaseResult
            {
                TestCaseId = point.TestCaseId,
                RequirementTitle = (suite.TestObject as IRequirementTestSuite)?.Title,
                TestCaseTitle = point.TestCaseWorkItem.Title,
                Descriptions = point.TestCaseWorkItem.Description,
                TestRunId = point.MostRecentResult?.TestRunId,
                Outcome = outcome,
                SuiteId = point.SuiteId,
                RunByName = runByName,
                ComputerName = point.ConfigurationName,
                DateStarted = point.MostRecentResult?.DateStarted,
                Duration = point.MostRecentResult?.Duration,
                AnalysisOwnerName = point.MostRecentResult?.OwnerName,
                Comment = point.MostRecentResult?.Comment
            };
            return results;
        } 

        public static IEnumerable<TestStepResult> GetTestStepResutls(IEnumerable<ITestPlan> plans)
        {
            var attachFolder = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd HHmmss"));
            if (System.IO.Directory.Exists(attachFolder) == false)
            {
                System.IO.Directory.CreateDirectory(attachFolder);
                try
                {
                    System.Diagnostics.Process.Start("EXPLORER.EXE", "/select,\"" + attachFolder + "\"");
                }
                catch { }
            } // end if

            Func<int, int, IAttachmentCollection, IEnumerable<string>> download = (testCaseId, stepNo, attachments) =>
            {
                if (attachments == null) return new string[] { };

                var files = from attachment in attachments
                            select new
                            {
                                Attachment = attachment,
                                NewFileName = Settings.Default.DownloadAttachmentFileNamePrefix + string.Format("test{0}-step{1}", testCaseId, stepNo) + "-" + attachment.Name
                                //NewFileName = "prefix" + string.Format("test{0}-step{1}", testCaseId, stepNo) + "-" + attachment.Name
                            };

                foreach (var file in files)
                {
                    var path = System.IO.Path.Combine(attachFolder, file.NewFileName);
                    file.Attachment.DownloadToFile(path);
                }
                return (from file in files select file.NewFileName);
            };


            var results =
                from plan in plans
                 from suite in plan.AllTestSuiteEntries()
                 from point in suite.AllTestPoints()
                 where point.MostRecentResult != null
                 let testCase = point.TestCaseWorkItem
                 from iteration in point.MostRecentResult.Iterations
                 from stepResult in iteration.Actions
                 from step in testCase.Actions.ToEnumerable<ITestStep>().Select((step, index) => new { Step = step, No = index + 1 })
                 where step.Step.Id == stepResult.ActionId
                 select new TestStepResult
                 {
                     TestCaseId = point.TestCaseId,
                     TestCaseTitle = point.TestCaseWorkItem.Title,
                     TestRunId = point.MostRecentResult.TestRunId,
                     StepNo = step.No,
                     Outcome = stepResult.Outcome.ToString(),
                     Action = step.Step.Title,
                     ExpectedResult = step.Step.ExpectedResult,
                     Comment = stepResult.ErrorMessage,
                     Attachments = string.Join(",\n", download(point.TestCaseId, step.No, stepResult.Attachments))

                 };
            return results;
        }

        public static IEnumerable<TestAction> GetTestActions(IEnumerable<ITestPlan> plans)
        {
            var results =
                    from plan in plans
                    from suite in plan.AllTestSuiteEntries()
                    from point in suite.AllTestPoints()
                    let testCase = point.TestCaseWorkItem
                    from step in testCase.Actions.ToEnumerable<ITestStep>().Select((step, index) => new { Step = step, No = index + 1 })
                    select new TestAction
                    {
                        SuiteId = point.SuiteId,
                        TestCaseId = point.TestCaseId,
                        TestCaseTitle = point.TestCaseWorkItem.Title,
                        StepNo = step.No,
                        Action = step.Step.Title,
                        ExpectedResult = step.Step.ExpectedResult,
                    };
            return results;
        } // end function

        public static void SaveTestResults( IEnumerable<ITestPlan> plans, IEnumerable<TestResultForImport> results, string imagesFolder)
        {
            Func<ITestRun, ITestPoint, bool> runSave = (run,point) =>
             {
                 run.AddTestPoint(point, point.AssignedTo);
                 run.Save();
                 return true;
             };

            var cases =
                from r in results
                group r by r.SuiteId into rGroup
                from plan in plans
                from suite in plan.AllTestSuiteEntries()
                where suite.Id == rGroup.FirstOrDefault()?.SuiteIdForSave
                from point in suite.AllTestPoints()
                let testCase = point.TestCaseWorkItem
                let run = plan.CreateTestRun(false)
                let runSaved = runSave(run, point)
                let runResult = run.QueryResults().FirstOrDefault()
                let iteration = runResult.CreateIteration(1)
                select new {
                    TestCase =testCase,
                    Run = run,
                    Point = point,
                    RunResult = runResult,
                    Iteration = iteration,
                    ResultGroup = rGroup.Where(r => r.TestCaseIdForSave == testCase.Id),
                };

            cases.ForEach(c => {
                var steps = (
                    from r in c.ResultGroup
                    from step in c.TestCase.Actions.ToEnumerable<ITestStep>().Select((step, index) => new { Step = step, No = index + 1 })
                    where step.No == r.StepNoForSave
                    let stepResult = c.Iteration.CreateStepResult(step.Step.Id)
                    select new {
                        Step = step.Step,
                        No = step.No,
                        StepResult = stepResult,
                        StepResultForImport = r
                    }).ToArray();

                c.RunResult.Iterations.Add(c.Iteration);
                c.RunResult.Save(false);

                steps.ForEach(s =>
                {
                    var to = s.StepResult;
                    var from = s.StepResultForImport;

                    from.ImportResult = "In Progress";
                    try
                    {
                        to.Outcome = (TestOutcome)Enum.Parse(typeof(TestOutcome), from.Outcome);
                        to.ErrorMessage = from.Comment;

                        if (string.IsNullOrWhiteSpace(from.Attachments) == false)
                        {
                            from.Attachments.Trim().Split('\n').ForEach(line =>
                            {
                                var fileName = line.Trim();
                                if (string.IsNullOrWhiteSpace(fileName) == false)
                                {
                                    var file = System.IO.Path.Combine(imagesFolder, fileName);
                                    var attachment = to.CreateAttachment(file);
                                    to.Attachments.Add(attachment);
                                } // end if
                            });
                        }

                        if (from.DateStartedForSave.HasValue) { to.DateStarted = from.DateStartedForSave.Value; }
                        if (from.DateCompletedForSave.HasValue) { to.DateCompleted = from.DateCompletedForSave.Value; }

                        if (from.DateStartedForSave.HasValue && from.DateCompletedForSave.HasValue) {
                            to.Duration = to.DateCompleted - to.DateStarted;
                        } // end if

                        c.Iteration.Actions.Add(to);
                        from.ImportResult = "Saving";
                    }
                    catch
                    {
                        from.HasFormatError = true;
                        from.ImportResult = "Failed";
                    }

                });

                var start = (from s in steps
                             where s.StepResultForImport.DateStartedForSave.HasValue
                             orderby s.StepResultForImport.DateStartedForSave
                             select s.StepResultForImport.DateStartedForSave).FirstOrDefault();
                var completed = (from s in steps
                                 where s.StepResultForImport.DateCompletedForSave.HasValue
                                 orderby s.StepResultForImport.DateCompletedForSave descending
                                 select s.StepResultForImport.DateCompletedForSave).FirstOrDefault();
                {
                    var run = c.Run;
                    if (start.HasValue) { run.DateStarted = start.Value; }
                    if (completed.HasValue) { run.DateCompleted = completed.Value; }
                }

                var outcome = TestOutcome.None;
                if (steps.Count() == 0) {
                    outcome = TestOutcome.None;
                } else if ((from s in steps where s.StepResult.Outcome == TestOutcome.Passed select s).Count() == steps.Count()) {
                    outcome = TestOutcome.Passed;
                } else {
                    outcome = TestOutcome.Failed;
                } // end if

                {
                    var result = c.RunResult;
                    result.Owner = c.Point.AssignedTo;
                    result.RunBy = c.Point.AssignedTo;
                    result.State = TestResultState.Completed;
                    if (start.HasValue) { result.DateStarted = start.Value; }
                    if (completed.HasValue) { result.DateCompleted = completed.Value; }
                    if (start.HasValue && completed.HasValue) { result.Duration = completed.Value - start.Value; }
                    result.Outcome = outcome;
                }
                {
                    var iteration = c.Iteration;
                    if (start.HasValue) { iteration.DateStarted = start.Value; }
                    if (completed.HasValue) { iteration.DateCompleted = completed.Value; }
                    if (start.HasValue && completed.HasValue) { iteration.Duration = completed.Value - start.Value; }
                    iteration.Outcome = outcome;
                }
                try
                {
                    c.RunResult.Save(false);

                    steps.ForEach(s => {
                        var from = s.StepResultForImport;
                        if (from.HasImportError == false) { from.ImportResult = "Saved"; }
                    });
                }
                catch 
                {
                    steps.ForEach(s => {
                        var from = s.StepResultForImport;
                        from.HasImportError = true;
                        from.ImportResult = "Failed";
                    });
                } // end try


            });

        } // end sub

    } // end class
} // end namespace
