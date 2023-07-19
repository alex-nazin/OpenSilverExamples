using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public class SingleTextBoxFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            ((TextBox)AssociatedObject).GotFocus += new RoutedEventHandler(TextBox_GotFocus);
        }

        void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            ((TextBox)AssociatedObject).GotFocus -= TextBox_GotFocus;
        }

    }
}
