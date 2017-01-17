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

namespace TeamReportPlus.View
{
    /// <summary>
    /// TestResultsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TestResultsPage : Page, IFramePage
    {
        public Frame MainFrame { get; set; }
        public IEnumerable<ITestPlan> Plans { get; set;}

        public TestResultsPage()
        {
            InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame?.GoBack();
        }

        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(this.summaryFrame, 2);
            this.summaryFrame.Visibility = Visibility.Visible;

            this.detailFrame.Visibility = Visibility.Hidden;
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            this.summaryFrame.Visibility = Visibility.Hidden;

            Grid.SetColumn(this.detailFrame, 0);
            Grid.SetColumnSpan(this.detailFrame, 2);
            this.detailFrame.Visibility = Visibility.Visible;
        }

        private void LeftRightButton_Click(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(this.summaryFrame, 1);
            this.summaryFrame.Visibility = Visibility.Visible;

            Grid.SetColumn(this.detailFrame, 1);
            Grid.SetColumnSpan(this.detailFrame, 1);
            this.detailFrame.Visibility = Visibility.Visible;

        }

        private TestResultsSummaryPage summaryPage;
        private TestResultsDetailPage detailPage;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.summaryPage = new TestResultsSummaryPage { MainFrame = this.summaryFrame, Plans = this.Plans };
            this.summaryFrame.Navigate(this.summaryPage);

            this.detailPage = new TestResultsDetailPage { MainFrame = this.detailFrame, Plans = this.Plans };
            this.detailFrame.Navigate(this.detailPage);
        }

        private void CopySummaryButton_Click(object sender, RoutedEventArgs e)
        {
            this.summaryPage?.CopyResults();
        } // end sub

        private void CopyDetailyButton_Click(object sender, RoutedEventArgs e)
        {
            this.detailPage?.CopyResults();
        } // end sub

    } // end class
} // end namespace
