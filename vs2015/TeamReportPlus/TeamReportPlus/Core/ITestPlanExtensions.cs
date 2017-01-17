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
    public static class ITestPlanExtensions
    {

        /// <summary>
        /// テスト計画全体のテストスイートの一覧を取得
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <returns>
        /// Returns ITestSuiteEntry items.
        /// </returns>
        public static IEnumerable<ITestSuiteEntry> AllTestSuiteEntries(this ITestPlan me)
        {
            if (me == null) throw new ArgumentNullException("me");

            return
                from suite in me.RootSuite.Entries.ToEnumerable<ITestSuiteEntry>()
                from entry in suite.AllTestSuiteEntries()
                select entry;
        } // end function
    } // end class
} // end namespace