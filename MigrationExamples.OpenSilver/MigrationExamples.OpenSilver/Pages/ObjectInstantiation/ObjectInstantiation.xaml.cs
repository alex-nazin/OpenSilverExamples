using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MigrationExamples.OpenSilver.Pages.ObjectInstantiation
{
    public partial class ObjectInstantiation : Page
    {
        private static readonly int Count = 100_000;

        private static TestObjectModel objectModel;

        public ObjectInstantiation()
        {
            this.InitializeComponent();
        }

        private void UseActivator_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            CreateObjectsUsingActivator(Count);
            sw.Stop();

            UseActivatorResult.Text = $"Created {Count} instances in {sw.ElapsedMilliseconds} ms";
        }

        private void UseExpression_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            CreateObjectsUsingExpressions(Count);
            sw.Stop();

            UseExpressionResult.Text = $"Created {Count} instances in {sw.ElapsedMilliseconds} ms";
        }

        private void CreateObjectsUsingActivator(int count)
        {
            for (int i = 0;  i < count; i++)
            {
                var obj = Activator.CreateInstance(typeof(TestObjectModel));
                objectModel = obj as TestObjectModel;
            }

            objectModel = null;
        }

        private void CreateObjectsUsingExpressions(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = ClassBuilder.CreateInstance<TestObjectModel>();
                objectModel = obj;
            }

            objectModel = null;
        }
    }
}
