using System.Text;
using System.Windows;
using Master_View.Views;

namespace Master_View.Services
{
    public static class InformationPopupService
    {
        private const string _usageGuideMain = "Follow differing buttons for their specific usage. The list on the bottom left provides information about the paths, while the selection box on the right shows you the directories.";
        private const string _usageGuideSecond = "Double clicking on the directory will select it and add it to the paths monitored.";
        private const string _usageGuideThird = "Clicking on an already selected path will bring up its window, where you can edit it's details.";
        private const string _usageGuideFourth = "Below that, you can select how many days should a file be able to exist before being atuomatically deleted.";
        private const string _usageGuideFifth = "You can toggle the three checkboxes in the top left, switching on/off specific deletion-features.";



        public static void UsageGuidePopup()
        {
            var concatenated = new StringBuilder();
            concatenated.Append(_usageGuideMain)
                .Append("\n\n")
                .Append(_usageGuideSecond)
                .Append("\n\n")
                .Append(_usageGuideThird)
                .Append("\n\n")
                .Append(_usageGuideFourth)
                .Append("\n\n")
                .Append(_usageGuideFifth);

            ErrorHandlingService.PopupHandler.Popup(concatenated.ToString(), "Usage Guide", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
