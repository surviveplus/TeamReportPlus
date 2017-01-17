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
    /// TestActionsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TestActionsPage : Page, IFramePage
    {
        public Frame MainFrame { get; set; }
        public IEnumerable<ITestPlan> Plans { get; set; }

        public TestActionsPage()
        {
            InitializeComponent();
            this.actions = new System.Collections.ObjectModel.ObservableCollection<Models.TestAction>();
            BindingOperations.EnableCollectionSynchronization(this.actions, new object());
        }

        private System.Collections.ObjectModel.ObservableCollection<Models.TestAction> actions;

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ResultListView.DataContext == null)
            {
                this.Progress.Visibility = Visibility.Visible;
                this.ResultListView.ItemsSource = this.actions;

                if (this.actions.Count == 0)
                {
                    await Task.Run(() =>
                    {
                        TeamClient.GetTestActions(this.Plans).ForEach((action) => { this.actions.Add(action); });
                    });
                }

                this.Progress.Visibility = Visibility.Hidden;
            } // end if

        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame?.GoBack();
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
                {
                    // Ctrl + C
                    this.CopyItems();

                } // end if


            } // end if
        }

        private void CopyItems( bool forImport = false)
        {
            var text = new StringBuilder();
            text.Append("Suite");
            text.Append("\t");
            text.Append("TestCaseId");
            text.Append("\t");
            text.Append("TestCaseTitle");
            text.Append("\t");
            text.Append("StepNo");
            text.Append("\t");
            text.Append("Action");
            text.Append("\t");
            text.Append("ExpectedResult");

            if (forImport)
            {
                text.Append("\t");
                text.Append("Outcome");
                text.Append("\t");
                text.Append("Comment");
                text.Append("\t");
                text.Append("Attachments");
                text.Append("\t");
                text.Append("DateStarted");
                text.Append("\t");
                text.Append("DateCompleted");
            } // end if
            text.AppendLine("");

            var format = true; //this.FormatCheckBox.IsChecked.Value;
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
            Action<Models.TestAction> collectItemForCopy = (item) =>
            {
                text.Append(item.SuiteId.ToString());
                text.Append("\t");
                text.Append(item.TestCaseId.ToString());
                text.Append("\t");
                text.Append(item.TestCaseTitle);
                text.Append("\t");
                text.Append(item.StepNo);
                text.Append("\t");
                text.Append(formatNoHtml(format, item.Action).EscapeMultilineForTsv());
                text.Append("\t");
                text.Append(formatNoHtml(format, item.ExpectedResult).EscapeMultilineForTsv());

                if (forImport)
                {
                    text.Append("\t");
                    text.Append("None / Passed / Failed"); // Outcome
                    text.Append("\t");
                    text.Append(""); // Comment
                    text.Append("\t");
                    text.Append(""); // Attachments
                    text.Append("\t");
                    if (item.StepNo == 1)
                    {
                        text.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.FFF")); // DateStarted
                    }else
                    {
                        text.Append(""); // DateStarted
                    } // end if
                    text.Append("\t");
                    text.Append(""); // DateCompleted
                } // end if

                text.AppendLine("");
            };

            if (this.ResultListView.SelectedItems.Count == 0)
            {
                this.ResultListView.Items.ToEnumerable<Models.TestAction>().ForEach(item => collectItemForCopy(item));
            }
            else
            {
                this.ResultListView.SelectedItems.ToEnumerable<Models.TestAction>().ForEach(item => collectItemForCopy(item));
            } // end if

            Clipboard.SetText(text.ToString());
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            this.CopyItems();
        }

        private void CopyForImportButton_Click(object sender, RoutedEventArgs e)
        {
            this.CopyItems(true);
        }
    }
}
