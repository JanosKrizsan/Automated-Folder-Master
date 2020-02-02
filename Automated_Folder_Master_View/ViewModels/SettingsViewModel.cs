
using Master_Library.Entities;
using Master_Library.Services;
using Master_View.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Master_View.ViewModels
{
    public class SettingsViewModel : Base
    {
        private SettingsInfo _settings = new SettingsInfo();

        private ObservableCollection<PathInfo> _paths = new ObservableCollection<PathInfo>();
        private SettingsInfo _defaultSettings = SettingsService.Default;

        private bool? _toggleAll = false;
        private bool[] _switches = new bool[] { true, false, false, true, true };
        private double _selectedLifeSpan;
        public ObservableCollection<PathInfo> ObsPaths
        {
            get => _paths;
            private set
            {
                SetProperty(ref _paths, value);
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
        public bool Autostart
        {
            get => _switches[0];
            set { SetProperty(ref _switches[0], value); }
        }
        public bool SetAllLifeToGlobal
        {
            get => _switches[1];
            set { SetProperty(ref _switches[1], value); }
        }
        public bool DeleteExes
        {
            get => _switches[2];
            set { SetProperty(ref _switches[2], value); }
        }
        public bool DeleteFolder
        {
            get => _switches[3];
            set { SetProperty(ref _switches[3], value); }
        }
        public bool SendToBin
        {
            get => _switches[4];
            set { SetProperty(ref _switches[4], value); }
        }

        public double SelectedLifeSpan 
        {
            get => _selectedLifeSpan;
            set { SetProperty(ref _selectedLifeSpan, value); }
        }

        public DelegateCommand<string> ToggleAllCommand { get; private set; }

        public DelegateCommand<string> SaveSettingsCommand { get; private set; }
        public DelegateCommand<string> ResetDefaultsCommand { get; private set; }
        public DelegateCommand<string> ClearPathsCommand { get; private set; }
        public DelegateCommand<string> LoadSettingsCommand { get; private set; }
        public DelegateCommand<string> UsageGuideCommand { get; private set; }


        public SettingsViewModel()
        {
            //delete this in release
            ExperimentalSetup();
            SetupControllers();
            SetCommands();
        }


        private void SetCommands()
        {
            SaveSettingsCommand = new DelegateCommand<string>(SaveSettings_Execute, LoadSettings_CanExecute);
            ResetDefaultsCommand = new DelegateCommand<string>(ResetDefaults_Execute, ResetDefaults_CanExecute);
            ClearPathsCommand = new DelegateCommand<string>(ClearPaths_Execute, ClearPaths_CanExecute);
            LoadSettingsCommand = new DelegateCommand<string>(LoadSettings_Execute, LoadSettings_CanExecute);
            UsageGuideCommand = new DelegateCommand<string>(UsageGuide_Execute);

            ToggleAllCommand = new DelegateCommand<string>(ToggleAll_Execute);
        }

        private TimeSpan SetSelectedLifeSpan()
        {
            return TimeSpan.FromDays(SelectedLifeSpan);
        }
        private void ToggleAll_Checker()
        {

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
            SetupControllers();
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
            SelectedLifeSpan = _defaultSettings.GlobalLifeSpan.TotalDays;
        }

        private void SaveSettings_Execute(string filler)
        {
            FinalizeConfig();
            SettingsService.SaveData(Settings);
        }

        private void FinalizeConfig()
        {
            _settings.Autostart = Autostart;
            _settings.DeleteExes = DeleteExes;
            _settings.DeleteFolder = DeleteFolder;
            _settings.GlobalLifeSpan = SetSelectedLifeSpan();
            _settings.SendToBin = SendToBin;

            if (SetAllLifeToGlobal)
            {
                SettingsService.SetGlobalLifeTime();
            }
            foreach (var folder in ObsPaths)
            {
                if (!_settings.Paths.Contains(folder))
                {
                    _settings.Paths.Add(folder);
                }
            }
        }

        private void SetupControllers()
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
            SelectedLifeSpan = Settings.GlobalLifeSpan.TotalDays;
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
