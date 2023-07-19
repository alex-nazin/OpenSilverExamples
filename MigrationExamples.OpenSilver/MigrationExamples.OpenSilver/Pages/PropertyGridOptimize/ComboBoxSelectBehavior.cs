using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public class ComboBoxSelectBehavior : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += new KeyEventHandler(AssociatedObject_KeyDown);
        }

        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Count() == 1)
            {
                ComboBox comboBox = ((ComboBox)sender);
                List<string> selectedItems = ((List<string>)comboBox.ItemsSource).Where(s => !string.IsNullOrEmpty(s)).Where(s => s.ToLower().First() == e.Key.ToString().ToLower().First()).ToList();
                if (selectedItems.Count > 0)
                    comboBox.SelectedItem = selectedItems[0];

            }
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }
    }
}
