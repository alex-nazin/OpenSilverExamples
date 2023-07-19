using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace MigrationExamples.OpenSilver.Pages.HideSomeItems
{
    internal static class ItemInfoGenerator
    {
        public static ItemInfo[] GenerateItems()
        {
            return GetColors()
                .Select(c => new ItemInfo
                {
                    MainText = c,
                    AdditionalText = $"Information about {c}",
                    DetailText = $"Details about {c}",
                })
                .ToArray();
        }

        private static IEnumerable<string> GetColors()
        {
            return typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(c => c.Name);
        }
    }
}
