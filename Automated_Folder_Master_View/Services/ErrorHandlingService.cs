using System;
using System.Windows;

namespace Master_View.Services
{
    public static class ErrorHandlingService
    {
        public static void ExceptionHandler(Exception ex)
        {

        }
        internal static class PopupHandler
        {
            public static void Popup(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
            {
                MessageBox.Show(text, caption, button, icon);
            }
        }
    }
}
