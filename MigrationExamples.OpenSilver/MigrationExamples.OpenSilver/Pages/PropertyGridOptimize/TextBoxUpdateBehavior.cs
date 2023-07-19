using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public class TextBoxUpdateBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyUp += new KeyEventHandler(AssociatedObject_KeyDown);
        }

        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab && e.Key != Key.Ctrl && e.Key != Key.Shift && !(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C))
                AssociatedObject.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyUp -= AssociatedObject_KeyDown;
        }
    }
}
