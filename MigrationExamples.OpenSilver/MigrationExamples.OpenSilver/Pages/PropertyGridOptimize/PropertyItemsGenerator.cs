using System;
using System.Collections.Generic;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    internal static class PropertyItemsGenerator
    {
        public static List<PropertyItem> Generate()
        {
            List<PropertyItem> properties = new List<PropertyItem>()
            {
                new PropertyItem
                {
                    Name = "Background",
                    IsReadOnly = true,
                    GroupName = "Brush",
                    Value = "No brush",
                },
                new PropertyItem
                {
                    Name = "BorderBrush",
                    IsReadOnly = true,
                    GroupName = "Brush",
                    Value = "Green",
                },
                new PropertyItem
                {
                    Name = "Foreground",
                    IsReadOnly = true,
                    GroupName = "Brush",
                    Value = "Red",
                },
                new PropertyItem
                {
                    Name = "OpacityMask",
                    IsReadOnly = true,
                    GroupName = "Brush",
                    Value = "No brush",
                },

                new PropertyItem
                {
                    Name = "Opacity",
                    GroupName = "Appearance",
                    Value = "100",
                },
                new PropertyItem
                {
                    Name = "Visibility",
                    GroupName = "Appearance",
                    Value = "Visible",
                    Options = new List<string> { "Visible", "Collapsed" },
                },
                new PropertyItem
                {
                    Name = "BorderThickness",
                    GroupName = "Appearance",
                    Value = "0,0,0,0",
                },
                new PropertyItem
                {
                    Name = "Effect",
                    GroupName = "Appearance",
                    IsDialog = true,
                    Value = string.Empty,
                },

                new PropertyItem
                {
                    Name = "ToolTipService.ToolTip",
                    GroupName = "Common",
                    Value = string.Empty,
                },
                new PropertyItem
                {
                    Name = "Cursor",
                    GroupName = "Common",
                    Value = "Arrow",
                    Options = new List<string> { "Arrow", "Eraser", "Hand", "IBeam", "None", "SizeNESW", "SizeNS", "SizeNWSE", "SizeWE", "Stylus", "Wait" },
                },
                new PropertyItem
                {
                    Name = "DataContext",
                    GroupName = "Common",
                    IsDialog = true,
                    Value = string.Empty,
                },
                new PropertyItem
                {
                    Name = "AllowDrop",
                    GroupName = "Common",
                    IsBool = true,
                    Value = "0",
                },
                new PropertyItem
                {
                    Name = "IsEnabled",
                    GroupName = "Common",
                    IsBool = true,
                    Value = "1",
                },
                new PropertyItem
                {
                    Name = "IsHitTestVisible",
                    GroupName = "Common",
                    IsBool = true,
                    Value = "1",
                },
                new PropertyItem
                {
                    Name = "IsTabStop",
                    GroupName = "Common",
                    IsBool = true,
                    Value = "0",
                },
                new PropertyItem
                {
                    Name = "TabIndex",
                    GroupName = "Common",
                    Value = "2147483647",
                },
                new PropertyItem
                {
                    Name = "TabNavigation",
                    GroupName = "Common",
                    Value = "Local",
                    Options = new List<string> { "Local", "Cycle", "Once" },
                },
                new PropertyItem
                {
                    Name = "Tag",
                    GroupName = "Common",
                    Value = string.Empty,
                },

                new PropertyItem
                {
                    Name = "Width",
                    GroupName = "Layout",
                    Value = "400",
                },
                new PropertyItem
                {
                    Name = "Height",
                    GroupName = "Layout",
                    Value = "400",
                },
                new PropertyItem
                {
                    Name = "HorizontalAlignment",
                    GroupName = "Layout",
                    Value = "Stretch",
                    Options = new List<string> { "Left", "Center", "Right", "Stretch" },
                },
                new PropertyItem
                {
                    Name = "VerticalAlignment",
                    GroupName = "Layout",
                    Value = "Stretch",
                    Options = new List<string> { "Top", "Center", "Bottom", "Stretch" },
                },
                new PropertyItem
                {
                    Name = "Margin",
                    GroupName = "Layout",
                    Value = "0,0,0,0",
                },

                new PropertyItem
                {
                    Name = "CacheMode",
                    GroupName = "Miscellaneous",
                    IsDialog = true,
                },
                new PropertyItem
                {
                    Name = "CharacterSpacing",
                    GroupName = "Miscellaneous",
                    Value = "0",
                },
                new PropertyItem
                {
                    Name = "Clip",
                    GroupName = "Miscellaneous",
                    Value = string.Empty,
                },
                new PropertyItem
                {
                    Name = "Style",
                    GroupName = "Miscellaneous",
                    Value = string.Empty,
                },
                new PropertyItem
                {
                    Name = "Template",
                    GroupName = "Miscellaneous",
                    Value = string.Empty,
                },
            };

            return Shuffle(properties);
        }

        private static List<PropertyItem> Shuffle(List<PropertyItem> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        private static Random rng = new Random();
    }
}
