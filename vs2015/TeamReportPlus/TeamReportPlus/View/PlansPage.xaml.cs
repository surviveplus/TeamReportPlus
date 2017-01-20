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
using TeamReportPlus.Properties;

namespace TeamReportPlus.View
{
    /// <summary>
    /// PlansPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PlansPage : Page, IFramePage
    {
        public Frame MainFrame { get; set; }
        public IEnumerable<Models.TeamServer> Servers { get; set; }

        public PlansPage()
        {
            InitializeComponent();
            this.plans = new System.Collections.ObjectModel.ObservableCollection<ViewModels.TestPlanListItem>();
            BindingOperations.EnableCollectionSynchronization(this.plans, new object());

        }
        private System.Collections.ObjectModel.ObservableCollection<ViewModels.TestPlanListItem> plans;

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame?.GoBack();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.imagePrefix.Text = Settings.Default.DownloadAttachmentFileNamePrefix;

            if (this.ResultListView.ItemsSource == null)
            {
                this.Progress.Visibility = Visibility.Visible;
                this.ResultListView.ItemsSource = this.plans;

                if (this.plans.Count == 0){
                    await Task.Run(() =>
                    {
                        (from p in TeamClient.GetTestPlans(this.Servers)
                         select ViewModels.TestPlanListItem.FromITestPlan(p)
                        ).ForEach((p) => { this.plans.Add(p); });
                    });
                }

                this.Progress.Visibility = Visibility.Hidden;
            } // end if
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var targets = this.ResultListView.SelectedItems.ToEnumerable<ViewModels.TestPlanListItem>();
            if (targets != null && targets.Count() > 0)
            {
                Settings.Default.DownloadAttachmentFileNamePrefix = this.imagePrefix.Text;
                Settings.Default.Save();

                var p = new TestResultsPage {MainFrame = this.MainFrame, Plans = (from item in targets select item.Plan ) };
                this.MainFrame.Navigate(p);
            } // end if
        }

        private void ExportTestActionsButton_Click(object sender, RoutedEventArgs e)
        {
            var targets = this.ResultListView.SelectedItems.ToEnumerable<ViewModels.TestPlanListItem>();
            if (targets != null && targets.Count() > 0)
            {
                var p = new TestActionsPage { MainFrame = this.MainFrame, Plans = (from item in targets select item.Plan) };
                this.MainFrame.Navigate(p);

            } // end if

        }

        private void ImportTestResultsButton_Click(object sender, RoutedEventArgs e)
        {
            var targets = this.ResultListView.SelectedItems.ToEnumerable<ViewModels.TestPlanListItem>();
            if (targets != null && targets.Count() > 0)
            {
                var p = new ImportTestResultsPage { MainFrame = this.MainFrame, Plans = (from item in targets select item.Plan) };
                this.MainFrame.Navigate(p);

            } // end if
        }
    }
}
