using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FoundryReports.View.Utils
{
    internal class DecoratedTextBox : TextBox
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(string), typeof(DecoratedTextBox), new PropertyMetadata(default(string)));

        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty GhostTextProperty = DependencyProperty.Register(
            "GhostText", typeof(string), typeof(DecoratedTextBox), new PropertyMetadata(default(string)));

        public string GhostText
        {
            get => (string) GetValue(GhostTextProperty);
            set => SetValue(GhostTextProperty, value);
        }

        public DecoratedTextBox()
        {
            GotFocus += OnGotFocus;
            PreviewMouseDown += OnPreviewMouseDown;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedText))
                SelectAll();
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsFocused)
                return;

            if (!string.IsNullOrWhiteSpace(SelectedText))
                return;

            Focus();
            SelectAll();
            e.Handled = true;
        }
    }
}
