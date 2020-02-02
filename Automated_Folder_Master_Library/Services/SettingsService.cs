using Master_Library.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Master_Library.Services
{
    public static class SettingsService
    {
        private static RegistryKey _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static SettingsInfo _currentSettings;
        private static readonly string _saveFilePath = @"C:\Users\Public\Documents\";
        private static readonly string _fileName = "settings.xml";
        private static readonly string _appName = "Automated_Folder_Master_Console";
        private static readonly string _appPath = GetExecutingConsoleDirectory();

        public static SettingsInfo Default { get; } = new SettingsInfo()
        {
            Autostart = true,
            DeleteExes = true,
            DeleteFolder = false,
            SendToBin = false,
            GlobalLifeSpan = TimeSpan.FromDays(30),
            Paths = new HashSet<PathInfo>()
        };

        public static SettingsInfo CurrentSettings
        {
            get => _currentSettings;
            set => _currentSettings = value;
        }

        public static void AddToStartup()
        {
            _regKey.SetValue(_appName, _appPath);
        }

        public static void RemoveFromStartup()
        {
            _regKey.DeleteValue(_appName, false);
        }

        public static void SetGlobalLifeTime(bool SetGlobal)
        {
            if (SetGlobal)
            {
                var updatedPaths = new HashSet<PathInfo>();

                foreach (var path in CurrentSettings.Paths)
                {
                    updatedPaths.Add(new PathInfo()
                    {
                        Path = path.Path,
                        LifeSpan = CurrentSettings.GlobalLifeSpan
                    });
                }
                _currentSettings.Paths = updatedPaths;
            }
        }

        public static void SetData(SettingsInfo info, bool SetGlobal)
        {
            CurrentSettings = info;
            switch (info.Autostart)
            {
                case true:
                    AddToStartup();
                    break;
                case false:
                    RemoveFromStartup();
                    break;
            }

            SetGlobalLifeTime(SetGlobal);
        }

        public static dynamic ReadData()
        {
            var serializer = new XmlSerializer(typeof(SettingsInfo));
            dynamic settings = new SettingsInfo();
            try
            {
                using var stream = new FileStream(string.Concat(_saveFilePath, _fileName), FileMode.Open, FileAccess.Read);
                var reader = new XmlTextReader(stream);
                settings = (SettingsInfo)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                return e;
            }
            return settings;
        }

        public static dynamic SaveData()
        {

            var serializer = new XmlSerializer(typeof(SettingsInfo));
            try
            {
                using (var writer = new StreamWriter(string.Concat(_saveFilePath, _fileName)))
                {
                    serializer.Serialize(writer, CurrentSettings);
                }
            }
            catch(IOException e)
            {
                return e;
            }
            return true;
        }
        private static string GetExecutingConsoleDirectory()
        {
            //rework this upon release
            var parentDir = Directory.GetParent(Directory.GetCurrentDirectory());
            var targetDirChildren = Directory.GetDirectories(parentDir.FullName);

            foreach (var folder in targetDirChildren)
            {
                if (folder.Contains("Automated_Folder_Master_Console"))
                {
                    return folder;
                }
            }
            return string.Empty;
        }
    }
}
