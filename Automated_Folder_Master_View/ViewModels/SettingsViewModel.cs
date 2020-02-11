
using Master_Library.Entities;
using Master_Library.Services;
using Master_View.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using static Master_View.Services.InformationPopupService;
using static Master_View.Services.ErrorHandlingService;

namespace Master_View.ViewModels
{
    public class SettingsViewModel : Base
    {
        private SettingsInfo _settings = new SettingsInfo();
        private SettingsInfo _defaultSettings = SettingsService.Default;
        private SimplePath _currentTempFolder;
        private ObservableCollection<PathInfo> _paths;
        private ObservableCollection<SimplePath> _viewedPaths;
        private List<int> Switches { get; set; } = new List<int>() { 0, 0, 0, 0, 0 };
        private bool? _toggleAll = false;
        private bool _setAllGlobal = false;
        public ObservableCollection<PathInfo> ObsPaths
        {
            get => _paths;
            private set
            {
                SetProperty(ref _paths, value);
            }
        }
        public ObservableCollection<SimplePath> PathsCurrentlyViewed
        {
            get => _viewedPaths;
            private set
            {
                SetProperty(ref _viewedPaths, value);
            }
        }
        public SettingsInfo Settings
        {
            get => _settings;
            private set
            {
                SetProperty(ref _settings, value);
            }
        }


        public double[] LifeSpans { get; } = new double[] { 7, 14, 30, 90, 180, 365 };
        public bool? ToggleAll
        {
            get => _toggleAll;
            set { SetProperty(ref _toggleAll, value); }
        }
        public bool SetAllLifeToGlobal
        {
            get => _setAllGlobal;
            set {_setAllGlobal = value; 
                Switches[0] = Converter(value);
                OnPropertyChanged();
                ToggleAll_Check(); }
        }
        public bool Autostart
        {
            get => _settings.Autostart;
            set {_settings.Autostart = value; 
                Switches[1] = Converter(value);
                OnPropertyChanged();
                ToggleAll_Check(); }
        }
        public bool DeleteExes
        {
            get => _settings.DeleteExes;
            set {_settings.DeleteExes = value; 
                Switches[2] = Converter(value);
                OnPropertyChanged();
                ToggleAll_Check(); }
        }
        public bool DeleteFolder
        {
            get => _settings.DeleteFolder;
            set {_settings.DeleteFolder = value;
                Switches[3] = Converter(value);
                OnPropertyChanged();
                ToggleAll_Check(); }
        }
        public bool SendToBin
        {
            get => _settings.SendToBin;
            set {_settings.SendToBin = value; 
                Switches[4] = Converter(value);
                OnPropertyChanged();
                ToggleAll_Check(); }
        }

        public double SelectedLifeSpan 
        {
            get => _settings.GlobalLifeSpan.TotalDays;
            set { OnPropertyChanged(); _settings.GlobalLifeSpan = TimeSpan.FromDays(value); }
        }


        public DelegateCommand<Hyperlink> ListClickCommand { get; private set; }
        public DelegateCommand<string> ToggleAllCommand { get; private set; }
        public DelegateCommand<string> SaveSettingsCommand { get; private set; }
        public DelegateCommand<string> ResetDefaultsCommand { get; private set; }
        public DelegateCommand<string> ClearPathsCommand { get; private set; }
        public DelegateCommand<string> LoadSettingsCommand { get; private set; }
        public DelegateCommand<string> UsageGuideCommand { get; private set; }
        public DelegateCommand<StackPanel> AddCurrentPathCommand { get; private set; }
        public DelegateCommand<Hyperlink> OpenPathCommand { get; private set; }
        public DelegateCommand<string> FolderMoveBackCommand { get; private set; }



        public SettingsViewModel()
        {
            SetupControllers();
            SetCommands();
        }


        private void SetCommands()
        {
            SaveSettingsCommand = new DelegateCommand<string>(SaveSettings_Execute);
            ResetDefaultsCommand = new DelegateCommand<string>(ResetDefaults_Execute, ResetDefaults_CanExecute);
            ClearPathsCommand = new DelegateCommand<string>(ClearPaths_Execute, ClearPaths_CanExecute);
            LoadSettingsCommand = new DelegateCommand<string>(LoadSettings_Execute);
            UsageGuideCommand = new DelegateCommand<string>(UsageGuide_Execute);

            ToggleAllCommand = new DelegateCommand<string>(ToggleAll_Execute);
            ListClickCommand = new DelegateCommand<Hyperlink>(ListClick_Execute);

            FolderMoveBackCommand = new DelegateCommand<string>(FolderMoveBack_Execute);
            AddCurrentPathCommand = new DelegateCommand<StackPanel>(AddPath_Execute);
            OpenPathCommand = new DelegateCommand<Hyperlink>(OpenPath_Execute);

        }

        private void OpenPath_Execute(Hyperlink pathToOpen)
        {
            try
            {
                _currentTempFolder = (SimplePath)pathToOpen.DataContext;

                var updatedPathList = new ObservableCollection<SimplePath>();
                var innerFolders = Directory.GetDirectories(_currentTempFolder.FullPath).ToList();

                AddItemsToCollection(updatedPathList, innerFolders);

                PathsCurrentlyViewed = updatedPathList;
            }
            catch (Exception e)
            {
                ExceptionHandler(e);
            }
        }

        private void AddPath_Execute(StackPanel pathInfo)
        {
            var folder = (SimplePath)pathInfo.DataContext;
            var currentPaths = _settings.Paths.ToList();

            var pathToAdd = ReplaceBackSlash(folder.FullPath);
            var isExisting = currentPaths.Any(path => ReplaceBackSlash(path.Path) == pathToAdd);

            var folderName = Path.GetFileName(folder.FullPath);
            var isDrive = GetAllDrives().Any(drive => ReplaceBackSlash(drive).Contains(pathToAdd));
            var isSpecialFolder = GetAllSpecialFolders().Any(spec => ReplaceBackSlash(spec).Contains(folderName));
            
            if (isDrive || isSpecialFolder)
            {
                ExceptionHandler(new UnauthorizedAccessException());
                return;
            }
            if (!isExisting)
            {
                var newPath = new PathInfo() { Path = folder.FullPath, LifeSpan = Settings.GlobalLifeSpan };
                ObsPaths.Add(newPath);
                Settings.Paths.Add(newPath);
                PopupHandler.SuccessPopup($"New path: '{newPath.Path}' successfully added.");
                RaiseCanExecuteEvents();
            }
            else
            {
                ExceptionHandler(new InvalidOperationException());
            }
        }

        private void FolderMoveBack_Execute(string filler)
        {
            var current = _currentTempFolder.FullPath;
            if (!string.IsNullOrEmpty(current))
            {
                var drives = GetAllDrives();

                var isParentMyComp = drives.Any(drive => drive.Equals(current));


                if (isParentMyComp)
                {
                    UpdateDrives(drives);
                }
                else
                {
                    var parentDir = Directory.GetParent(current);

                    if (parentDir != null)
                    {
                        var resetFolders = new ObservableCollection<SimplePath>();
                        var parentSubs = Directory.GetDirectories(parentDir.FullName).ToList();

                        AddItemsToCollection(resetFolders, parentSubs);

                        PathsCurrentlyViewed = resetFolders;

                        _currentTempFolder = SimplePathCreator(parentDir.FullName, FileNameGetter(parentDir.FullName));
                    }
                }
            }
        }

        private void ListClick_Execute(Hyperlink pathToEdit)
        {
            var path = (PathInfo)pathToEdit.DataContext;
            var pathWindow = new PathEditWindow();
            pathWindow.DataContext = new PathEditViewModel(path, pathWindow, Settings, ObsPaths);
            pathWindow.Show();

        }

        private void RaiseCanExecuteEvents()
        {
            ResetDefaultsCommand.RaiseCanExecuteChanged();
            ClearPathsCommand.RaiseCanExecuteChanged();
        }
        private void ToggleAll_Check()
        {
            var trueFalse = 0;
            Switches.ForEach(num => trueFalse += num);

            switch (trueFalse)
            {
                case 0:
                    ToggleAll = false;
                    break;
                case 5:
                    ToggleAll = true;
                    break;
                default:
                    ToggleAll = null;
                    break;
            }
        }

        private void ToggleAll_Execute(string filler)
        {
            if (ToggleAll != null)
            {
                var toggle = (bool)ToggleAll;
                Autostart = toggle;
                DeleteExes = toggle;
                DeleteFolder = toggle;
                SendToBin = toggle;
                SetAllLifeToGlobal = toggle;
            }
        }

        private void UsageGuide_Execute(string filler)
        {
            UsageGuidePopup();
        }

        private void ClearPaths_Execute(string set)
        {
            ObsPaths = new ObservableCollection<PathInfo>();
            _settings.Paths = new HashSet<PathInfo>();

            if (!(set == "reset"))
            {
                RaiseCanExecuteEvents();
                PopupHandler.SuccessPopup("All paths have been cleared.");
            }
        }

        private bool ClearPaths_CanExecute(string filler)
        {
            return ObsPaths.Count != 0 && Settings.Paths.Count != 0;
        }

        private void LoadSettings_Execute(string filler)
        {
            var successfulLoad = SetupControllers(true);
            RaiseCanExecuteEvents();

            if (successfulLoad)
            {
                PopupHandler.SuccessPopup("Successfully loaded your settings.");
            }
        }
        private bool ResetDefaults_CanExecute(string filler)
        {
            return !Settings.Equals(_defaultSettings);
        }

        private void ResetDefaults_Execute(string filler)
        {
            Settings = _defaultSettings;
            ClearPaths_Execute("reset");
            UpdateValues();
            RaiseCanExecuteEvents();

            PopupHandler.SuccessPopup("Successfully reset your settings.");
        }

        private void SaveSettings_Execute(string filler)
        {
            try
            {
                SettingsService.SetData(Settings, SetAllLifeToGlobal);
                SettingsService.SaveData();
                PopupHandler.SuccessPopup("Successfully saved your settings.");
                SetupControllers(true);
            }
            catch(Exception e)
            {
                ExceptionHandler(e);
            }
        }

        private bool SetupControllers(bool reload = false)
        {
            _settings = SettingsService.Default;
            ObsPaths = new ObservableCollection<PathInfo>();
            try
            {
                _settings = SettingsService.ReadData();
            }
            catch (Exception e)
            {
                ExceptionHandler(e);
            }

            Settings.Paths.ToList().ForEach(folder => ObsPaths.Add(folder));

            if (!reload)
            {
                UpdateDrives();
            }

            UpdateValues();
            return true;
        }
        private void UpdateDrives(List<string> driveInfos = null)
        {
            PathsCurrentlyViewed = new ObservableCollection<SimplePath>();
            driveInfos ??= GetAllDrives();

            driveInfos.ForEach(drive => PathsCurrentlyViewed.Add(SimplePathCreator(drive, drive)));
        }

        private void UpdateValues()
        {
            Autostart = Autostart;
            DeleteExes = DeleteExes;
            DeleteFolder = DeleteFolder;
            SendToBin = SendToBin;
            SelectedLifeSpan = SelectedLifeSpan;

            UpdateSwitches();
        }

        private void UpdateSwitches()
        {
                Switches = new List<int>() {
                Converter(SetAllLifeToGlobal),
                Converter(Autostart),
                Converter(DeleteExes),
                Converter(DeleteFolder),
                Converter(SendToBin)};
        }

        private int Converter(bool value)
        {
            return Convert.ToInt32(value);
        }

        private SimplePath SimplePathCreator(string path, string name)
        {
            return new SimplePath()
            {
                FullPath = path,
                Name = name
            };
        }

        private void AddItemsToCollection(ObservableCollection<SimplePath> collection, List<string> fromList)
        {
            fromList.ForEach(folder => collection.Add(SimplePathCreator(folder, FileNameGetter(folder))));
        }

        private string FileNameGetter(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }

        private string ReplaceBackSlash(string toEdit)
        {
            return toEdit.Replace("\\", "");
        }

        private List<string> GetAllDrives()
        {
            return (from drive in DriveInfo.GetDrives().ToList()
                    select drive.Name).ToList();
        }

        private List<string> GetAllSpecialFolders()
        {
            var specials = new List<string>();

            foreach (Environment.SpecialFolder folder in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                specials.Add(Environment.GetFolderPath(folder));
            }

            return specials;
        }
    }
}
