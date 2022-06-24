using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Security;
using System.Security.Permissions;

namespace Apiraiser.Experimental
{
    public class QueryProvider : IQueryProvider
    {
        private readonly IQueryContext queryContext;

        public QueryProvider(IQueryContext queryContext)
        {
            this.queryContext = queryContext;
        }

        public virtual IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type; //TypeSystem.GetElementType(expression.Type);
            try
            {
                return
                   (IQueryable)Activator.CreateInstance(typeof(Queryable<>).
                          MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
            // return null;
        }

        public virtual IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new Queryable<T>(this, expression);
            // return null;
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return queryContext.Execute(expression, false);
            // return null;
        }

        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)queryContext.Execute(expression,
                       (typeof(T).Name == "IEnumerable`1"));
        }
    }
}