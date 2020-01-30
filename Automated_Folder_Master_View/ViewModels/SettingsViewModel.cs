
using Master_Library.Entities;
using Master_Library.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Master_View.ViewModels
{
    public class SettingsViewModel
    {
        private SettingsInfo _settings = new SettingsInfo();
        public ObservableCollection<PathInfo> Paths { get; } = new ObservableCollection<PathInfo>();
        public bool DeleteExes { get; set; }
        public bool DeleteFolder { get; set; }
        public bool SendtoBin { get; set; }
        public string AutoStartPath { get; set; }
        public TimeSpan GlobalLifeSpan { get; set; }

        public SettingsViewModel()
        {
            //delete this in release
            ExperimentalSetup();
            SetupElements();
        }

        private void SetupElements()
        {
            _settings = SettingsService.ReadData();
            foreach (var folder in _settings.Paths)
            {
                Paths.Add(folder);
            }


        }
        private void ExperimentalSetup()
        {
            var paths = new HashSet<PathInfo>();
            paths.Add(new PathInfo()
            {
                Path = @"C:\Users\John\Postman",
                LifeSpan = TimeSpan.FromDays(14)
            });
            try
            {
                var data = SettingsService.SaveData(new SettingsInfo()
                {
                    AutoStartPath = "",
                    DeleteExes = true,
                    DeleteFolder = true,
                    SendToBin = false,
                    GlobalLifeSpan = TimeSpans.OneWeek,
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
