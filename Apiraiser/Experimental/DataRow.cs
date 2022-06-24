
#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace Apiraiser.Experimental
{
    [DefaultMember("Item")]
    public class DataRow : Dictionary<string, object>
    {
        public DataRow() : base() { }

        public DataRow(IEnumerable<KeyValuePair<string, object>> pairs) : base(pairs) { }

        public Dictionary<string, object> AsDictionary()
        {
            return (Dictionary<string, object>)this;
        }
    }

}