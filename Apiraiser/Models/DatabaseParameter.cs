using System;
using System.Collections.Generic;

namespace Apiraiser.Models
{

    public class DatabaseParameter
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public DatabaseParameter(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

}