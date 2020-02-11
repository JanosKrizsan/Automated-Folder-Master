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
        private const string _usageGuideSeventh = "Please place the compiled Settings and Console folders within the same directory.";


        public static void UsageGuidePopup()
        {
            var newLine = "\n\n";
            var concatenated = new StringBuilder();
            concatenated.Append(_usageGuideMain)
                .Append(newLine)
                .Append(_usageGuideSecond)
                .Append(newLine)
                .Append(_usageGuideThird)
                .Append(newLine)
                .Append(_usageGuideFourth)
                .Append(newLine)
                .Append(_usageGuideFifth)
                .Append(newLine)
                .Append(_usageGuideSixth)
                .Append(newLine)
                .Append(_usageGuideSeventh);

            Popup(concatenated.ToString(), "Usage Guide", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
