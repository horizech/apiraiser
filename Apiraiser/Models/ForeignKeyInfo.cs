using System;
using System.Collections.Generic;

using Apiraiser.Services;
using Apiraiser.Interfaces;
using Apiraiser.Enums;

namespace Apiraiser.Models
{
    public class ForeignKeyInfo
    {
        public string ParentSchema { get; set; }
        public string ParentTable { get; set; }
        public string ParentColumn { get; set; }
        public string ChildSchema { get; set; }
        public string ChildTable { get; set; }
        public string ChildColumn { get; set; }
        public string ConnectionName { get; set; }

        public void Print()
        {
            ServiceManager.Instance.GetService<LogService>().Print(string.Format("ParentSchema: {0}, ParentTable: {1}, ParentColumn: {2}, ChildColumn: {3}, ConnectionName: {4}", ParentSchema, ParentTable, ParentColumn, ChildColumn, ConnectionName), LoggingLevel.All);
        }
    }

}