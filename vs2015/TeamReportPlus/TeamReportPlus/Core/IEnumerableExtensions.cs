using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Core
{
    /// <summary>
    /// Object 型に対する拡張メソッドを定義する静的クラスです。
    /// </summary>
    public static partial class ObjectExtensions {

		/// <summary>
		/// foreach は可能だが IEnumerable&lt;T&gt; は実装していない クラシック コレクション を反復処理する列挙子を返します。
		/// </summary>
		/// <typeparam name="T">要素の型</typeparam>
		/// <param name="me">拡張メソッドを追加する元の型のオブジェクトです。null 参照 (Visual Basic では Nothing) を指定した時は、空の列挙子を返します。</param>
		/// <returns>
		/// 指定された T型 を反復する列挙子を返します。
		/// </returns>
		public static IEnumerable<T> ToEnumerable<T>( this Object me ) {
			if (me != null) {
				dynamic list = me;

				foreach (T item in list) {
					yield return item;
				} // next item
			} // end if

		} // end function
	} // end class

	/// <summary>
	/// Object 型に対する拡張メソッドを定義する静的クラスです。
	/// </summary>
	public static partial class ObjectExtensions
	{
		/// <summary>
		/// インスタンス自身を要素として1つだけ返す列挙子を返します。
		/// </summary>
		/// <typeparam name="T">要素の型</typeparam>
		/// <param name="me">拡張メソッドを追加する元の型のオブジェクトです。null 参照 (Visual Basic では Nothing) を指定した時は、空の列挙子を返します。</param>
		/// <returns></returns>
		/// <remarks>
		/// <para>次のサンプルコードを実行すると、list は IEnumerable&lt;int&gt; となります。</para>
		/// <code>
		/// int a = 0;
		/// var list = a.Singleton();
		/// </code>
		/// </remarks>
		public static IEnumerable<T> Singleton<T>( this T me ) {

			if (me != null) {
				yield return me;
			} // end if

		} // end function
	} // end class

	/// <summary>
	/// IEnumerable&lt;T&gt; に対する拡張メソッドを定義する静的クラスです。
	/// </summary>
	public static partial class IEnumerableExtensions
	{
		/// <summary>
		/// 反復処理を実行します。
		/// 列挙子をそのまま返すので、メソッドチェーンで処理を継続出来ます。
		/// </summary>
		/// <param name="me">拡張メソッドを追加する元の型のオブジェクトです。</param>
		/// <param name="action">要素に対して実行する反復処理を指定します。</param>
		/// <returns>
		/// 列挙子をそのまま返します。
		/// </returns>
		/// <exception cref="ArgumentNullException">null 参照 (Visual Basic では Nothing) を指定した時</exception>
		public static IEnumerable<T> ForEach<T>( this IEnumerable<T> me, Action<T> action ) {
			if (me == null) throw new ArgumentNullException( "me" );
			if (action == null) throw new ArgumentNullException( "action" );

			if (action != null) {
				foreach (var item in me) {
					action( item );
				}// next item
			} // end if

			return me;
		} // end function
	} // end class

} // end namespace
