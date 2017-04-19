using Microsoft.TeamFoundation.TestManagement.Client;
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
    public static class ITestManagementTeamProjectExtensions
    {

        /// <summary>
        /// テスト計画の一覧を取得
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <returns>
        /// Returns TestPlan items.
        /// </returns>
        public static IEnumerable<ITestPlan> AllTestPlans(this ITestManagementTeamProject me)
        {
            if (me == null) throw new ArgumentNullException("me");
            return me.TestPlans.Query("Select * From TestPlan").ToEnumerable<ITestPlan>();
        } // end function
    } // end class
} // end namespace