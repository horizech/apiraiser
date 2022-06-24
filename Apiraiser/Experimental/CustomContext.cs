using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Apiraiser.Experimental
{
    public class CustomContext : IQueryContext
    {
        /*
        /// Testing
        var ctx = new Queryable<Dictionary<string, object>>(new CustomContext());
        var query = ctx.Where(
            x => x.Contains(
                        new KeyValuePair<string, object>("Username", "Zeeshan1"))
            ).ToList();

        foreach (var q in query)
        {
            Console.WriteLine(q);
        }
        */

        private static KeyValuePair<Type, object>[] ResolveArgs<T>(Expression<Func<T, object>> expression)
        {
            var body = (System.Linq.Expressions.MethodCallExpression)expression.Body;
            var values = new List<KeyValuePair<Type, object>>();

            foreach (var argument in body.Arguments)
            {
                var exp = ResolveMemberExpression(argument);
                var type = argument.Type;

                var value = GetValue(exp);

                values.Add(new KeyValuePair<Type, object>(type, value));
            }

            return values.ToArray();
        }

        public static MemberExpression ResolveMemberExpression(Expression expression)
        {

            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                // if casting is involved, Expression is not x => x.FieldName but x => Convert(x.Fieldname)
                return (MemberExpression)((UnaryExpression)expression).Operand;
            }
            else
            {
                throw new NotSupportedException(expression.ToString());
            }
        }

        private static object GetValue(MemberExpression exp)
        {
            // expression is ConstantExpression or FieldExpression
            if (exp.Expression is ConstantExpression)
            {
                return (((ConstantExpression)exp.Expression).Value)
                        .GetType()
                        .GetField(exp.Member.Name)
                        .GetValue(((ConstantExpression)exp.Expression).Value);
            }
            else if (exp.Expression is MemberExpression)
            {
                return GetValue((MemberExpression)exp.Expression);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void recursiveCheckExpression(Expression expression)
        {
            if (expression is MethodCallExpression)
            {
                ReadOnlyCollection<Expression> collections = ((MethodCallExpression)expression).Arguments;
                foreach (Expression exp in collections)
                {
                    Console.WriteLine(exp);
                    recursiveCheckExpression(exp);
                }
            }
            else if (expression is ParameterExpression)
            {
                Object obj = ((ParameterExpression)expression).Name;
                Console.WriteLine(obj);
            }
            else if (expression is ConstantExpression)
            {
                Object obj = ((ConstantExpression)expression).Value;
                Console.WriteLine(obj);
            }
            else if (expression is UnaryExpression)
            {
                Expression exp = ((UnaryExpression)expression).Operand;
                Console.WriteLine(exp);
                recursiveCheckExpression(exp);
            }
            else if (expression is LambdaExpression)
            {
                Expression exp = ((LambdaExpression)expression).Body;
                Console.WriteLine(exp);
                recursiveCheckExpression(exp);
            }
            else if (expression is NewExpression)
            {
                ReadOnlyCollection<Expression> collections = ((NewExpression)expression).Arguments;
                foreach (Expression exp in collections)
                {
                    Console.WriteLine(exp);
                    recursiveCheckExpression(exp);
                }
            }
        }

        public object Execute(Expression expression, bool isEnumerable)
        {
            // Expression l = Expression.Lambda<Dictionary<string, object>>(expression, false);

            recursiveCheckExpression(expression);

            // IEnumerable<List<Dictionary<string, object>>> list = 
            List<Dictionary<string, object>> result =
            (new List<Dictionary<string, object>>(){
                new Dictionary<string, object>
                        {
                            { "Username", "Zeeshan1" }
                        }

            });


            IEnumerable<Dictionary<string, object>> list = result.AsEnumerable();

            return list;

            // return new List<string>() { "1", "2", "3" };
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }
}