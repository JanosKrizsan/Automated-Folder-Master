
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

        private ObservableCollection<PathInfo> _paths;
        private SettingsInfo _defaultSettings = SettingsService.Default;

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
            set { SetProperty(ref _setAllGlobal, value); }
        }
        public bool Autostart
        {
            get => _settings.Autostart;
            set { OnPropertyChanged(); _settings.Autostart = value; }
        }
        public bool DeleteExes
        {
            get => _settings.DeleteExes;
            set { OnPropertyChanged(); _settings.DeleteExes = value; }
        }
        public bool DeleteFolder
        {
            get => _settings.DeleteFolder;
            set { OnPropertyChanged(); _settings.DeleteFolder = value; }
        }
        public bool SendToBin
        {
            get => _settings.SendToBin;
            set { OnPropertyChanged(); _settings.SendToBin = value; }
        }

        public double SelectedLifeSpan 
        {
            get => _settings.GlobalLifeSpan.TotalDays;
            set { OnPropertyChanged(); _settings.GlobalLifeSpan = TimeSpan.FromDays(value); }
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


        private void ToggleAll_Execute(string filler)
        {
            //if (ToggleAll != null)
            //{
            //    var toggle = (bool)ToggleAll;
            //    Autostart = toggle;
            //    DeleteExes = toggle;
            //    DeleteFolder = toggle;
            //    SendToBin = toggle;
            //    SetAllLifeToGlobal = toggle;
            //}
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

            UpdateValues();
        }

        private void UpdateValues()
        {
            Autostart = Autostart;
            DeleteExes = DeleteExes;
            DeleteFolder = DeleteFolder;
            SendToBin = SendToBin;
            SelectedLifeSpan = SelectedLifeSpan;
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
