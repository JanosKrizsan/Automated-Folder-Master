using System.Text;
using System.Windows;
using static Master_View.Services.ErrorHandlingService.PopupHandler;

namespace Master_View.Services
{
    public static class InformationPopupService
    {
        private const string _usageGuideMain = "Follow differing buttons for their specific usage. The list on the bottom left provides information about the paths, while the selection box on the right shows you the directories.";
        private const string _usageGuideSecond = "Clicking on the '+' next to a directory will select it and add it to the paths monitored. Clicking on the directory path will open it.";
        private const string _usageGuideThird = "Clicking on an already selected path (bottom left) will bring up its window, where you can edit its details.";
        private const string _usageGuideFourth = "Above that, you can select how many days should a file exist for, before being automatically deleted.";
        private const string _usageGuideFifth = "You can toggle the checkboxes in the top left, switching on/off specific deletion-features.";
        private const string _usageGuideSixth = "Except the toggle for using global lifetime, all other settings will be saved and persisted.";

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
                .Append(_usageGuideFifth)
                .Append("\n\n")
                .Append(_usageGuideSixth);

            Popup(concatenated.ToString(), "Usage Guide", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
