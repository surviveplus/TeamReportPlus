using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeamReportPlus.Core;

namespace TeamReportPlus.View
{
    /// <summary>
    /// TestResultsSummaryPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TestResultsSummaryPage : Page, IFramePage
    {
        public Frame MainFrame { get; set; }
        public IEnumerable<ITestPlan> Plans { get; set; }


        public TestResultsSummaryPage()
        {
            InitializeComponent();
            this.results = new System.Collections.ObjectModel.ObservableCollection<Models.TestCaseResult>();
            BindingOperations.EnableCollectionSynchronization(this.results, new object());
        }
        private System.Collections.ObjectModel.ObservableCollection<Models.TestCaseResult> results;

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ResultListView.DataContext == null)
            {
                this.Progress.Visibility = Visibility.Visible;
                this.ResultListView.ItemsSource = this.results;

                if (this.results.Count == 0)
                {
                    await Task.Run(() =>
                    {
                        TeamClient.GetTestCaseResults(this.Plans).ForEach((result) => { this.results.Add(result); });
                    });
                }

                this.Progress.Visibility = Visibility.Hidden;
            } // end if

        }

        private void ResultListView_KeyDown(object sender, KeyEventArgs e)
        {
            if( (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None){
                if(e.Key == Key.A)
                {
                    // Ctrl + A
                    this.ResultListView.SelectAll();
                }
                else if (e.Key == Key.C)
                    // Ctrl + C
                    CopyResults();
            } // end if
        } // end sub

        public void CopyResults()
        {

            var text = new StringBuilder();
            text.Append("Suite");
            text.Append("\t");
            text.Append("TestCaseId");
            text.Append("\t");
            text.Append("RequirementTitle");
            text.Append("\t");
            text.Append("TestCaseTitle");
            text.Append("\t");
            text.Append("Descriptions");
            text.Append("\t");
            text.Append("TestRunId");
            text.Append("\t");
            text.Append("Outcome");
            text.Append("\t");
            text.Append("RunByName");
            text.Append("\t");
            text.Append("ComputerName");
            text.Append("\t");
            text.Append("DateStarted");
            text.Append("\t");
            text.Append("Duration");
            text.Append("\t");
            text.Append("AnalysisOwnerName");
            text.Append("\t");
            text.Append("Comment");
            text.AppendLine("");

            Action<Models.TestCaseResult> collectItemForCopy = (item) =>
            {
                text.Append(item.SuiteId.ToString());
                text.Append("\t");
                text.Append(item.TestCaseId.ToString());
                text.Append("\t");
                text.Append(item.RequirementTitle);
                text.Append("\t");
                text.Append(item.TestCaseTitle);
                text.Append("\t");
                text.Append(item.Descriptions);
                text.Append("\t");
                text.Append(item.TestRunId?.ToString());
                text.Append("\t");
                text.Append(item.Outcome);
                text.Append("\t");
                text.Append(item.RunByName);
                text.Append("\t");
                text.Append(item.ComputerName);
                text.Append("\t");
                text.Append(item.DateStarted?.ToString());
                text.Append("\t");
                text.Append(item.Duration?.ToString());
                text.Append("\t");
                text.Append(item.AnalysisOwnerName);
                text.Append("\t");
                text.Append(item.Comment);
                text.AppendLine("");
            };

            if (this.ResultListView.SelectedItems.Count == 0)
            {
                this.ResultListView.Items.ToEnumerable<Models.TestCaseResult>().ForEach(item => collectItemForCopy(item));
            }
            else
            {
                this.ResultListView.SelectedItems.ToEnumerable<Models.TestCaseResult>().ForEach(item => collectItemForCopy(item));
            } // end if

            Clipboard.SetText(text.ToString());

            // end if
        } // end sub

    } // end class
} // end namespace
