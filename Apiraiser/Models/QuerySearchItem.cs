using System;
using System.Collections.Generic;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Interfaces;
using Apiraiser.Enums;

namespace Apiraiser.Models
{
    public class QuerySearchItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ColumnCondition Condition { get; set; }
        public bool CaseSensitive { get; set; }

        public void Print()
        {
            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Name: {0}, Value: {1}, Condition: {2}, CaseSensitive: {3}", Name, Value.ToString(), Condition.ToString(), CaseSensitive.ToString()), LoggingLevel.All);
        }
    }

}