using System.Collections.Generic;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public class PropertyItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsBool { get; set; }
        public bool IsReadOnly { get; set; }
        public string GroupName { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public List<bool> BoolOptions { get; } = new List<bool>() { true, false };
        public bool IsDialog { get; set; }
        public bool HasOptions => Options?.Count > 0;
    }
}
