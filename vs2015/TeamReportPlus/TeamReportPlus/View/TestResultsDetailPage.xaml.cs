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
    /// TestResultsDetailPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TestResultsDetailPage : Page, IFramePage
    {
        public Frame MainFrame { get; set; }
        public IEnumerable<ITestPlan> Plans { get; set; }


        public TestResultsDetailPage()
        {
            InitializeComponent();
            this.results = new System.Collections.ObjectModel.ObservableCollection<Models.TestStepResult>();
            BindingOperations.EnableCollectionSynchronization(this.results, new object());
        }
        private System.Collections.ObjectModel.ObservableCollection<Models.TestStepResult> results;

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
                        TeamClient.GetTestStepResutls(this.Plans).ForEach((result) => { this.results.Add(result); });
                    });
                }

                this.Progress.Visibility = Visibility.Hidden;
            } // end if

        }

        private void ResultListView_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                if (e.Key == Key.A)
                {
                    // Ctrl + A
                    this.ResultListView.SelectAll();
                }
                else if (e.Key == Key.C)
                    // Ctrl + C
                    CopyResults();

            } // end if
        }

        public void CopyResults()
        {
            var format = true; //this.FormatCheckBox.IsChecked.Value;

            var text = new StringBuilder();
            text.Append("TestCaseId");
            text.Append("\t");
            text.Append("TestCaseTitle");
            text.Append("\t");
            text.Append("TestRunId");
            text.Append("\t");
            text.Append("StepNo");
            text.Append("\t");
            text.Append("Outcome");
            text.Append("\t");
            text.Append("Action");
            text.Append("\t");
            text.Append("ExpectedResult");
            text.Append("\t");
            text.Append("Comment");
            text.Append("\t");
            text.Append("Attachments");
            text.AppendLine("");

            Func<bool, string, string> formatNoHtml = (doFormat, targetText) =>
            {
                if (doFormat)
                {
                    return targetText.ConvertHtmlToText().Trim();
                }
                else
                {
                    return targetText;
                }
            };

            Action<Models.TestStepResult> collectItemForCopy = (item) =>
            {
                text.Append(item.TestCaseId.ToString());
                text.Append("\t");
                text.Append(item.TestCaseTitle);
                text.Append("\t");
                text.Append(item.TestRunId.ToString());
                text.Append("\t");
                text.Append(item.StepNo.ToString());
                text.Append("\t");
                text.Append(item.Outcome);
                text.Append("\t");
                text.Append(formatNoHtml(format, item.Action).EscapeMultilineForTsv());
                text.Append("\t");
                text.Append(formatNoHtml(format, item.ExpectedResult).EscapeMultilineForTsv());
                text.Append("\t");
                text.Append(item.Comment.EscapeMultilineForTsv());
                text.Append("\t");
                text.Append(item.Attachments.EscapeMultilineForTsv());
                text.AppendLine("");
            };

            if (this.ResultListView.SelectedItems.Count == 0)
            {
                this.ResultListView.Items.ToEnumerable<Models.TestStepResult>().ForEach(item => collectItemForCopy(item));
            }
            else
            {
                this.ResultListView.SelectedItems.ToEnumerable<Models.TestStepResult>().ForEach(item => collectItemForCopy(item));
            } // end if

            Clipboard.SetText(text.ToString());

            // end if
        } // end sub

    } // end class
} // end namespace
