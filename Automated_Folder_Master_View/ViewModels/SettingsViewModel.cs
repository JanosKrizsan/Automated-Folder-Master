
using Master_Library.Entities;
using Master_Library.Services;
using Master_View.Services;
using Master_View.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Master_View.ViewModels
{
    public class SettingsViewModel : Base
    {
        private SettingsInfo _settings = new SettingsInfo();

        private ObservableCollection<PathInfo> _paths;
        private ObservableCollection<SimplePath> _viewedPaths;
        private SettingsInfo _defaultSettings = SettingsService.Default;

        private bool? _toggleAll = false;
        private SimplePath _currentTempFolder;
        private bool _setAllGlobal = false;
        private List<int> Switches { get; set; } = new List<int>() { 0, 0, 0, 0, 0 };
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
                LoadSettingsCommand.RaiseCanExecuteChanged();
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
            //delete this in release
            ExperimentalSetup();
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
            ListClickCommand = new DelegateCommand<Hyperlink>(ListDoubleClick_Execute);

            FolderMoveBackCommand = new DelegateCommand<string>(FolderMoveBack_Execute);
            AddCurrentPathCommand = new DelegateCommand<StackPanel>(AddPath_Execute);
            OpenPathCommand = new DelegateCommand<Hyperlink>(OpenPath_Execute);

        }

        private void OpenPath_Execute(Hyperlink pathToOpen)
        {
            _currentTempFolder = (SimplePath)pathToOpen.DataContext;

            var updatedPathList = new ObservableCollection<SimplePath>();
            var innerFolders = Directory.GetDirectories(_currentTempFolder.FullPath);

            foreach(var folder in innerFolders)
            {
                var dirName = Path.GetFileName(folder);
                updatedPathList.Add(new SimplePath() { FullPath = folder, Name = dirName });
            }

            PathsCurrentlyViewed = updatedPathList;
        }

        private void AddPath_Execute(StackPanel pathInfo)
        {
            var textBlock = (TextBlock)pathInfo.Children[1];
            var info = textBlock.Inlines.Count;

            var folder = (SimplePath)pathInfo.DataContext;
            ObsPaths.Add(new PathInfo() { Path = folder.FullPath, LifeSpan = Settings.GlobalLifeSpan });
        }

        private void FolderMoveBack_Execute(string filler)
        {
            var current = _currentTempFolder.FullPath;
            if (!string.IsNullOrEmpty(current))
            {
                var resetFolders = new ObservableCollection<SimplePath>();
                var parentDir = Directory.GetParent(current);
                var isDrive = false;

                if (parentDir != null)
                {
                    var drives = DriveInfo.GetDrives().ToList();
                    foreach (var drive in drives)
                    {
                        if (drive.Name.Equals(parentDir.Name))
                        {
                            isDrive = true;
                            break;
                        }
                    }
                }

                if (parentDir != null && !isDrive)
                {
                    var parentSubs = Directory.GetDirectories(parentDir.FullName);

                    foreach (var dir in parentSubs)
                    {
                        var dirName = Path.GetFileName(dir);
                        resetFolders.Add(new SimplePath() { FullPath = dir, Name = dirName });
                    }

                    PathsCurrentlyViewed = resetFolders;
                }
                else
                {
                    UpdateDrives();
                }
            }
        }

        private void ListDoubleClick_Execute(Hyperlink pathToEdit)
        {
            var path = (PathInfo)pathToEdit.DataContext;
            var pathWindow = new PathEditWindow();
            pathWindow.DataContext = new PathEditViewModel(path, pathWindow, Settings, ObsPaths);
            pathWindow.Show();

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
            InformationPopupService.UsageGuidePopup();
        }

        private void ClearPaths_Execute(string filler)
        {
            ObsPaths = new ObservableCollection<PathInfo>();
            _settings.Paths = new HashSet<PathInfo>();
        }

        private bool ClearPaths_CanExecute(string filler)
        {
            return ObsPaths.Count != 0 && Settings.Paths.Count != 0;
        }

        private void LoadSettings_Execute(string filler)
        {
            SetupControllers();
        }
        private bool ResetDefaults_CanExecute(string filler)
        {
            return !Settings.Equals(_defaultSettings);
        }

        private void ResetDefaults_Execute(string filler)
        {
            ObsPaths = new ObservableCollection<PathInfo>();
            Settings = _defaultSettings;
            UpdateValues();
        }

        private void SaveSettings_Execute(string filler)
        {
            SettingsService.SetData(Settings, SetAllLifeToGlobal);
            SettingsService.SaveData();
        }

        private void SetupControllers()
        {
            _settings = SettingsService.ReadData();
            ObsPaths = new ObservableCollection<PathInfo>();

            foreach (var folder in Settings.Paths)
            {
                ObsPaths.Add(folder);
            }

            UpdateDrives();
            UpdateValues();
        }
        private void UpdateDrives()
        {
            PathsCurrentlyViewed = new ObservableCollection<SimplePath>();
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                PathsCurrentlyViewed.Add(new SimplePath() { FullPath = drive.Name, Name = drive.Name });
            }
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

        private void ExperimentalSetup()
        {
            var paths = new HashSet<PathInfo>
            {
                new PathInfo()
                {
                    Path = @"C:\Users\John\Postman",
                    LifeSpan = TimeSpan.FromDays(14)
                },
                new PathInfo()
                {
                    Path = @"C:\Users\John\",
                    LifeSpan = TimeSpan.FromDays(7)
                }
            };
            try
            {
                SettingsService.SetData((new SettingsInfo()
                {
                    Autostart = false,
                    DeleteExes = true,
                    DeleteFolder = true,
                    SendToBin = false,
                    GlobalLifeSpan = TimeSpan.FromDays(7),
                    Paths = paths
                }), false);
                var data = SettingsService.SaveData();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
