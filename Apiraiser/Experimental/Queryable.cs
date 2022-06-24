using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections;

namespace Apiraiser.Experimental
{
    public class Queryable<T> : IOrderedQueryable<T>
    {
        public Queryable(IQueryContext queryContext)
        {
            Initialize(new QueryProvider(queryContext), null);
        }

        public Queryable(IQueryProvider provider)
        {
            Initialize(provider, null);
        }

        internal Queryable(IQueryProvider provider, Expression expression)
        {
            Initialize(provider, expression);
        }

        private void Initialize(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (expression != null && !typeof(IQueryable<T>).
                   IsAssignableFrom(expression.Type))
                throw new ArgumentException(
                     String.Format("Not assignable from {0}", expression.Type), "expression");

            Provider = provider;
            Expression = expression ?? Expression.Constant(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression { get; private set; }
        public IQueryProvider Provider { get; private set; }
    }
}
