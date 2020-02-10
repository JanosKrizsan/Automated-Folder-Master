using System;
using System.Windows;

namespace Master_View.Services
{
    public static class ErrorHandlingService
    {
        public static void ExceptionHandler(Exception ex)
        {
            var text = string.Empty;
            var caption = string.Empty;
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Exclamation;

            switch (ex.GetType().ToString())
            {
                case "System.UnauthorizedAccessException":
                    text = "You do not have the necessary privileges to open this folder!";
                    caption = "Access Denied";
                    break;
                case "System.InvalidOperationException":
                    text = "The path you are trying to add is already included.";
                    caption = "Path Already Existing";
                    icon = MessageBoxImage.Information;
                    break;
                case "System.IO.DirectoryNotFoundException":
                case "System.IO.FileNotFoundException":
                    text = "The save file could not be found, or is corrupted!";
                    caption = "Save File Error";
                    icon = MessageBoxImage.Warning;
                    break;
                case "System.Exception":
                    text = "An unknown error has occurred, please seek advice from the creator.";
                    caption = "Unknown Error";
                    break;
            }
            PopupHandler.ErrorPopup(text, caption, button, icon);
        }
        public static class PopupHandler
        {
            public static void ErrorPopup(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
            {
                MessageBox.Show(text, caption, button, icon);
            }

            public static void SuccessPopup(string text)
            {
                MessageBox.Show(text, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
