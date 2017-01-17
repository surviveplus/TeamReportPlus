using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamReportPlus.Models;

namespace TeamReportPlus.ViewModels
{
    public class ServerTreeItem : ServerProjectTreeItem
    {

        public string Name { get; set; }
        public Uri Uri { get; set; }

        public IEnumerable<ProjectTreeItem> Projects { get; set; }

        public override string Text
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        } // end property

        public override IEnumerable<ServerProjectTreeItem> Children
        {
            get
            {
                return this.Projects;
            }

            set
            {
                this.Projects = (value as IEnumerable<ProjectTreeItem>);
            }
        }

        public static ServerTreeItem FromTeamServer( TeamServer server)
        {
            return new ServerTreeItem {
                Name = server.Name,
                Uri = server.Uri,
                Projects = (from p in server.Projects select ProjectTreeItem.FromTeamProject(p)).ToArray()
            };
        }

    }
}
