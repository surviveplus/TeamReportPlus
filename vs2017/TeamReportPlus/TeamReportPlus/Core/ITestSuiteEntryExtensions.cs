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
    public static class ITestSuiteEntryExtensions
    {

        /// <summary>
        /// テストスイート全体のテストスイートの一覧を取得
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <returns>
        /// Returns ITestSuiteEntry items.
        /// </returns>
        public static IEnumerable<ITestSuiteEntry> AllTestSuiteEntries(this ITestSuiteEntry me)
        {
            if (me == null) throw new ArgumentNullException("me");

            // 再帰呼び出し
            Func<ITestSuiteEntry, IEnumerable<ITestSuiteEntry>> getSuite = null;
            getSuite = (suite) =>
            {
                var m = suite.TestSuite as IStaticTestSuite;
                if (m != null)
                {
                    Func<IEnumerable<ITestSuiteEntry>> other = () =>
                    {
                        var staticSuite = suite.TestSuite as IStaticTestSuite;
                        return
                            from entry in staticSuite.Entries.ToEnumerable<ITestSuiteEntry>()
                            from entrySuite in getSuite(entry)
                            select entrySuite;
                    };

                    if (suite.TestCase == null)
                    {
                        return suite.Singleton().Union(other());
                    }
                    else
                    {
                        return other();
                    } // end if

                }
                else
                {
                    //return suite.TestSuite.TestSuiteEntry.Singleton();
                    return suite.Singleton();
                } // end if
            };

            return getSuite(me);
        } // end function

        /// <summary>
        /// 対象のテストスイートで実行された対象のテストケースのテスト結果を取得 
        /// </summary>
        /// <param name="me"></param>
        /// <param name="testcase"></param>
        /// <returns></returns>
        public static IEnumerable<ITestPoint> SelectTestPoints(this ITestSuiteEntry me, ITestCase testcase)
        {
            if(me.TestSuite != null) {
                var plan = me.TestSuite.Plan;
                return plan.QueryTestPoints(string.Format("SELECT * FROM TestPoint Where SuiteId = {0} AND TestCaseId = {1}", me.Id, testcase.Id)).ToEnumerable<ITestPoint>();
            }else
            {
                return new ITestPoint[] { };
            } // end if
        }

        /// <summary>
        /// 対象のテストスイートで実行された対象のテストの結果を取得
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static IEnumerable<ITestPoint> AllTestPoints(this ITestSuiteEntry me)
        {
            if (me.TestSuite == null)
            {
                return
                    from ca in me.TestCase.Singleton()
                    from c in me.SelectTestPoints(ca)
                    select c;
            }
            else
            {
                return
                    from t in me.TestSuite?.TestCases?.ToEnumerable<ITestSuiteEntry>()
                    from c in me.SelectTestPoints(t.TestCase)
                    select c;
            } // end if

        }

    } // end class
} // end namespace