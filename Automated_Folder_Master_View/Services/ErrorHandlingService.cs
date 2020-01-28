using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Master_View.Services
{
    public static class ErrorHandlingService
    {
        public static void ExceptionHandler(Exception ex)
        {

        }
        private static class PopupHandler
        {
            public static void Popup(Window window, string text, string caption, MessageBoxButton button, MessageBoxImage icon)
            {
                MessageBox.Show(window, text, caption, button, icon);
            }
        }
    }
}
