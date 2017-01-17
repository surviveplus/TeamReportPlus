using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamReportPlus.Models;

namespace TeamReportPlus.ViewModels
{
    public class ProjectTreeItem : ServerProjectTreeItem
    {
        public string Name { get; set; }

        public override string Text{
            get{
                return this.Name;
            }
            set{
                this.Name = value;
            }
        } // end property

        public static ProjectTreeItem FromTeamProject( TeamProject from)
        {
            return new ProjectTreeItem
                {
                    Name = from.Name
                };
        }

        public ProjectTreeItem()
        {
            this.IsCheckBox = true;
        }

    }
}
