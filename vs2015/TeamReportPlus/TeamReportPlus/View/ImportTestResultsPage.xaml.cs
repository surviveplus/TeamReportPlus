using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualBasic.FileIO;
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
using TeamReportPlus.Models;
using TeamReportPlus.Properties;
//using TeamReportPlus.Properties;

namespace TeamReportPlus.View
{
    /// <summary>
    /// ImportTestResultsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class ImportTestResultsPage : Page, IFramePage
    {
        public Frame MainFrame { get; set; }
        public IEnumerable<ITestPlan> Plans { get; set; }

        public ImportTestResultsPage()
        {
            InitializeComponent();
            this.results = new System.Collections.ObjectModel.ObservableCollection<Models.TestResultForImport>();
            BindingOperations.EnableCollectionSynchronization(this.results, new object());
        }

        private System.Collections.ObjectModel.ObservableCollection<Models.TestResultForImport> results;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ResultListView.DataContext == null)
            {
                this.Progress.Visibility = Visibility.Visible;
                this.ResultListView.ItemsSource = this.results;
                this.Progress.Visibility = Visibility.Hidden;
                this.imagesFolderBox.Text = Settings.Default.UploadAttachmentFileFolder;
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
                else if (e.Key == Key.V)
                {
                    // Ctrl + V
                    this.PasteItems();

                } // end if

            }else
            {
                if(e.Key == Key.Delete)
                {
                    // Delete
                    var targets = this.ResultListView.SelectedItems.ToEnumerable<TestResultForImport>().ToArray();
                    targets.ForEach(target => { this.results.Remove(target); });
                } // end if
            
            } // end if
        } // end sub

        private void PasteItems()
        {

            var text = Clipboard.GetText();
            using (var reader = new System.IO.StringReader(text))
            using(var paser = new TextFieldParser(reader))
            {
                paser.SetDelimiters( "\t" );
                while(paser.EndOfData == false)
                {
                    var values = paser.ReadFields();

                    var result = new TestResultForImport {
                        SuiteId = values[0],
                        TestCaseId = values[1],
                        TestCaseTitle = values[2],
                        StepNo = values[3],
                        Action = values[4],
                        ExpectedResult = values[5],
                        Outcome = values[6],
                        Comment = values[7],
                        Attachments = values[8],
                        DateStarted = values[9],
                        DateCompleted = values[10]
                    };

                    if(result.IsHeader == false)
                    {
                        result.CheckFormat();
                        this.results.Add(result);
                    }// end if

                } // end while
            } // end using( reader, paser )

        } // end sub

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            this.PasteItems();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var imagesFolder = this.imagesFolderBox.Text;
            this.Progress.Visibility = Visibility.Visible;
            await Task.Run(() =>
            {
                TeamClient.SaveTestResults(this.Plans, this.results, imagesFolder);
            });
            this.Progress.Visibility = Visibility.Hidden;
            Settings.Default.UploadAttachmentFileFolder = this.imagesFolderBox.Text;
            Settings.Default.Save();

        }
    }
}
