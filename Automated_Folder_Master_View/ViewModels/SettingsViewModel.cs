
using Master_Library.Entities;
using Master_Library.Services;
using Master_View.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Master_View.ViewModels
{
    public class SettingsViewModel : Base
    {
        private SettingsInfo _settings = new SettingsInfo();

        private ObservableCollection<PathInfo> _paths = new ObservableCollection<PathInfo>();
        private SettingsInfo _defaultSettings = new SettingsInfo()
        {
            Autostart = true,
            DeleteExes = true,
            DeleteFolder = false,
            SendToBin = false,
            GlobalLifeSpan = TimeSpan.FromDays(30),
            Paths = new HashSet<PathInfo>()
        };

        private bool _deleteExes = true;
        private bool _deleteFolder = false;
        private bool _sendToBin = false;
        private bool _autostart = true;
        private TimeSpan _globalLifeSpan = TimeSpan.FromDays(30);
        public ObservableCollection<PathInfo> ObsPaths
        {
            get => _paths;
            private set
            {
                SetProperty(ref _paths, value);
                CheckCanExecutors();
            }
        }
        public SettingsInfo Settings
        {
            get => _settings;
            private set
            {
                SetProperty(ref _settings, value);
                CheckCanExecutors();
            }
        }


        public int[] LifeSpans { get; } = new int[] { 7, 14, 30, 90, 180, 365 };
        public bool DeleteExes { get => _deleteExes; 
            set { SetProperty(ref _deleteExes, value); } }
        public bool DeleteFolder { get => _deleteFolder; 
            set { SetProperty(ref _deleteFolder, value); } }
        public bool SendToBin { get => _sendToBin; 
            set { SetProperty(ref _sendToBin, value); } }
        public bool Autostart { get => _autostart; 
            set { SetProperty(ref _autostart, value); } }
        public TimeSpan GlobalLifeSpan { get => _globalLifeSpan; 
            set { SetProperty(ref _globalLifeSpan, value); } }

        public DelegateCommand<string> SaveSettingsCommand { get; private set; }
        public DelegateCommand<string> ResetDefaultsCommand { get; private set; }
        public DelegateCommand<string> ClearPathsCommand { get; private set; }
        public DelegateCommand<string> LoadSettingsCommand { get; private set; }
        public DelegateCommand<string> UsageGuideCommand { get; private set; }


        public SettingsViewModel()
        {
            //delete this in release
            ExperimentalSetup();
            SetupElements();
            SetCommands();
        }


        private void SetCommands()
        {
            SaveSettingsCommand = new DelegateCommand<string>(SaveSettings_Execute, LoadSettings_CanExecute);
            ResetDefaultsCommand = new DelegateCommand<string>(ResetDefaults_Execute, ResetDefaults_CanExecute);
            ClearPathsCommand = new DelegateCommand<string>(ClearPaths_Execute, ClearPaths_CanExecute);
            LoadSettingsCommand = new DelegateCommand<string>(LoadSettings_Execute, LoadSettings_CanExecute);
            UsageGuideCommand = new DelegateCommand<string>(UsageGuide_Execute);
        }

        private void CheckCanExecutors()
        {
            SaveSettingsCommand.RaiseCanExecuteChanged();
            ResetDefaultsCommand.RaiseCanExecuteChanged();
            ClearPathsCommand.RaiseCanExecuteChanged();
            LoadSettingsCommand.RaiseCanExecuteChanged();
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
            SetupElements();
        }
        private bool LoadSettings_CanExecute(string filler)
        {
            return !(Settings.Equals(SettingsService.ReadData()));
        }

        private bool ResetDefaults_CanExecute(string filler)
        {
            return !Settings.Equals(_defaultSettings);
        }

        private void ResetDefaults_Execute(string filler)
        {
            ObsPaths = new ObservableCollection<PathInfo>();
            Settings = _defaultSettings;
            DeleteExes = _defaultSettings.DeleteExes;
            DeleteFolder = _defaultSettings.DeleteFolder;
            SendToBin = _defaultSettings.SendToBin;
            Autostart = _defaultSettings.Autostart;
            GlobalLifeSpan = _defaultSettings.GlobalLifeSpan;
        }

        private void SaveSettings_Execute(string filler)
        {
            SettingsService.SaveData(Settings);
        }

        private void SetupElements()
        {
            _settings = SettingsService.ReadData();

            foreach (var folder in Settings.Paths)
            {
                if(!ObsPaths.Contains(folder))
                ObsPaths.Add(folder);
            }

            DeleteExes = Settings.DeleteExes;
            DeleteFolder = Settings.DeleteFolder;
            SendToBin = Settings.SendToBin;
            Autostart = Settings.Autostart;
            GlobalLifeSpan = Settings.GlobalLifeSpan;
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
                var data = SettingsService.SaveData(new SettingsInfo()
                {
                    Autostart = false,
                    DeleteExes = true,
                    DeleteFolder = true,
                    SendToBin = false,
                    GlobalLifeSpan = TimeSpan.FromDays(7),
                    Paths = paths
                });
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        
    }
}
