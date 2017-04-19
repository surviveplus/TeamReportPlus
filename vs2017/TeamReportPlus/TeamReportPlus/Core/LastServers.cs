using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamReportPlus.Core;
using TeamReportPlus.Models;
using TeamReportPlus.Properties;

namespace TeamReport.VisualStudio
{
    public class LastServers
    {
        public static IEnumerable<TeamServer> GetServers()
        {
            var servers = new List<TeamServer>();

            var text = Settings.Default.LastServers;
            try
            {
                var s = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<TeamServer>>(text);
                servers.AddRange(s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (servers.Count == 0)
            {
                Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory.Collections.ForEach(c =>
                {
                    var projects =
                        (from p in c.GetService<Microsoft.TeamFoundation.Server.ICommonStructureService>().ListProjects()
                         orderby p.Name
                         select new TeamProject { Name = p.Name }).ToList();

                    servers.Add(new TeamServer { Name = c.DisplayName, Uri = c.Uri, Projects = projects });
                });
            } // end if

            if (servers.Count == 0 ){
                return null;
            }else{
                return servers;
            } // end if
        } // end function

        public static void Save(IEnumerable<TeamServer> servers)
        {
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(servers);
            Settings.Default.LastServers = text;
            Settings.Default.Save();
        } // end function

    } // end class
} // end namespace
