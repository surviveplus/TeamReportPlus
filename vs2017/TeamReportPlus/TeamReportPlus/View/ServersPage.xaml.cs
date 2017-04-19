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
using TeamReport.VisualStudio;
using TeamReportPlus.Core;
using TeamReportPlus.Models;

namespace TeamReportPlus.View
{
    /// <summary>
    /// ServersPage.xaml の相互作用ロジック
    /// </summary>
    public partial class ServersPage : Page , IFramePage
    {
        public Frame MainFrame { get; set; }

        public ServersPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<TeamServer> servers = LastServers.GetServers();
            if (servers == null)
            {
                servers = TeamClient.GetServersWithPicker();
                LastServers.Save(servers);
            }//end if

            //servers.ForEach((s) => {
            //    Console.WriteLine(s.Name);
            //    s.Projects.ForEach((project) =>
            //    {
            //        Console.WriteLine(project.Name);
            //    });
            //});

            var viewModel = new {
                Servers =(from s in servers select ViewModels.ServerTreeItem.FromTeamServer(s)).ToArray()
            };
            this.DataContext = viewModel;

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var servers = LastServers.GetServers();
            var newServers = TeamClient.GetServersWithPicker();
            if (newServers.Count() == 0) return;

            newServers.ForEach((n) =>
            {
                servers = (from s in servers where s.Uri != n.Uri select s);
            });
            servers = servers.Union(newServers).OrderBy((s)=>s.Name).ToArray();

           LastServers.Save(servers);

            var viewModel = new
            {
                Servers = (from s in servers select ViewModels.ServerTreeItem.FromTeamServer(s)).ToArray()
            };
            this.DataContext = viewModel;

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new
            {
                Servers = new ViewModels.ServerTreeItem[] { }
            };
            LastServers.Save(null);
            this.DataContext = viewModel;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic data  = this.projectsTree.DataContext;
            var servers = data.Servers as IEnumerable<ViewModels.ServerTreeItem>;
            if (servers != null &&
                servers.Count() > 0)
            {
                var targets = 
                    (from s in servers
                     select new Models.TeamServer {
                        Name = s.Name,
                        Uri = s.Uri ,
                        Projects = ( from p in s.Projects where p.IsChecked select new Models.TeamProject { Name = p.Name }) } ).ToArray();

                
                if(targets.Count() >0 )
                {
                    var page = new PlansPage { MainFrame = this.MainFrame, Servers = targets };
                    this.MainFrame.Navigate(page);
                } // end if
            } // end if
        }

        private void HowToUseLink_Click(object sender, RoutedEventArgs e)
        {
            var url = "https://github.com/surviveplus/TeamReportPlus/blob/master/HowToUse/HowToUse.md";
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch {
                MessageBox.Show("See : " + url);
            }
        }
    }
}
