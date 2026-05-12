using System;
using System.Collections.Generic;

namespace Filereader
{
    public class FormatConfig
    {
        public string DisplayName { get; set; }
        public List<string> RecordTypeNames { get; set; } = new List<string>();
        public string SelectorTypeName { get; set; }
        public string SelectorMethodName { get; set; }
        public List<string> CuratedFields { get; set; } = new List<string>();
        public List<ColumnConfig> MasterColumns { get; set; } = new List<ColumnConfig>();
    }

    public class ColumnConfig
    {
        public string Name { get; set; }
        public string DataType { get; set; } // e.g., "System.String"
        public bool IsPrimaryKey { get; set; }
    }

    public class FormatCollection
    {
        public List<FormatConfig> Formats { get; set; } = new List<FormatConfig>();
    }
}
