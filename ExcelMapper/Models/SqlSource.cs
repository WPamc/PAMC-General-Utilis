using System.Collections.Generic;

namespace ExcelMapper.Models
{
    public class SqlSource
    {
        public string Name { get; set; }
        public List<string> Columns { get; set; } = new List<string>();

        public SqlSource(string name, IEnumerable<string> columns)
        {
            Name = name;
            Columns = new List<string>(columns);
        }

        public override string ToString() => Name;
    }
}
