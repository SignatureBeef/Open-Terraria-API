using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OTA.DebugFramework
{
    public class Assert
    {

        /// <summary>
        /// Asserts the specified LINQ expression for a true condition and throws an assertion 
        /// failure exception indicating the compiled expression which failed, if the result of 
        /// the expression is true.
        /// </summary>
        /// <param name="assertionExpr">A LINQ-style expression which to evaluate</param>
        /// <example>
        /// Use a LINQ-style expression to evaluate an assertion expression.
        /// 
        /// Assert.Expression(() => objectRef.Property == null);
        /// </example>
        /// <remarks>
        /// This code will only run in debug mode.  Calls to this method are ignored when
        /// the DEBUG symbol is not defined.
        /// </remarks>
        [Conditional("DEBUG")]
        public static void Expression(Expression<Func<bool>> assertionExpr)
        {
            /*
             * Compiles the LINQ lambda expression tree into an actual executable
             * delegate.  This can be expensive, it is imperative it only runs in 
             * DEBUG mode where the performance hit can be expected for the testing
             * framework.
             * 
             * Expression will close over the `this` pointer and all references to
             * whatever the expression contains, so no problems should arise from
             * executing a function using fields from another method not in scope
             * of this function.
             */
            Func<bool> assertionDelegate = assertionExpr.Compile();

            if (assertionDelegate())
            {
                throw new AssertionException(assertionExpr.ToString());
            }
        }
    }

    public class AssertionException : Exception
    {
        public AssertionException(string message)
            : base($"Assertion \"{message}\" failed.")
        {
        }
    }
}
