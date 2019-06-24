//------------------------------------------------------------------------------
// <copyright file="TeamReportPlusWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace TeamReportPlus
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TeamReportPlusWindowControl.
    /// </summary>
    public partial class TeamReportPlusWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamReportPlusWindowControl"/> class.
        /// </summary>
        public TeamReportPlusWindowControl()
        {
            this.InitializeComponent();
            serversPage.MainFrame = this.mainFrame;
            System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        } // end constructor


        private View.ServersPage serversPage = new View.ServersPage();


        private void MyToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.mainFrame.Content == null)
            {
                this.mainFrame.Navigate(this.serversPage);
            } // end if
        } // end sub

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, System.ResolveEventArgs args)
        {

            var name = new System.Reflection.AssemblyName(args.Name);
            var path = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer";
            //var path = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer";

            if (name.Name.EndsWith(".resources"))
            {
                path = System.IO.Path.Combine(path, "ja");

            } // end if

            var file = System.IO.Path.Combine(path, name.Name + ".dll");
            var a = System.Reflection.Assembly.LoadFrom(file);
            return a;

        } // end function

    } // end class
} // end namespace