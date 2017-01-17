using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TeamReportPlus.ViewModels
{
    public class ServerProjectTreeItem
    {
        public virtual string Text { get; set; }

        public virtual IEnumerable<ServerProjectTreeItem> Children { get; set; }

        public string FontWeight { get; set; } = "Normal";

        public virtual bool IsChecked { get; set; }

        public virtual bool IsCheckBox { get; set; } = false;

        public virtual Visibility CheckBoxVisibility {
            get {
                if( this.IsCheckBox)
                {
                    return Visibility.Visible;
                }else
                {
                    return Visibility.Collapsed;
                } // end if
            }
        } // end property

        public virtual Visibility TextVisibility
        {
            get
            {
                if (this.IsCheckBox)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                } // end if
            }
        } // end property

    } // end class

} // end namespace
