using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Core
{
    /// <summary>
    /// Static class which is defined extension methods.
    /// </summary>
    public static class TfsTeamProjectCollectionExtensions
    {

        /// <summary>
        /// プロジェクトの一覧を取得
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <returns>
        /// Returns Test Project items.
        /// </returns>
        public static IEnumerable<ITestManagementTeamProject> AllTestManagementTeamProjects(this TfsTeamProjectCollection me)
        {
            if (me == null) throw new ArgumentNullException("me");

            var testService = me.GetService<ITestManagementService>();
            var store = me.GetService<WorkItemStore>();
            return
                (from p in store.Projects.ToEnumerable<Project>()
                 select testService.GetTeamProject(p));


        } // end function
    } // end class
} // end namespace