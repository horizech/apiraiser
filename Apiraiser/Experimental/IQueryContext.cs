using System.Linq.Expressions;

namespace Apiraiser.Experimental
{
    public interface IQueryContext
    {
        object Execute(Expression expression, bool isEnumerable);
    }
}
