﻿using Master_Library.Entities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Master_Library.Services
{
    public static class SettingsService
    {
        private static RegistryKey _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static SettingsInfo _currentSettings = ReadData();
        private static readonly string _fileName = "/settings.xml";
        private static readonly string _appName = "Automated_Folder_Master_Console";
        private static readonly string _appPath = CurrentSettings.AutoStartPath;

        public static SettingsInfo CurrentSettings
        {
            get => _currentSettings;
            set => _currentSettings = value;
        }

        public static TimeSpan LifeSpan { get; set; } = CurrentSettings.GlobalLifeSpan;

        public static bool DeleteExes { get; set; } = CurrentSettings.DeleteExes;

        public static void AddToStartup()
        {
            _regKey.SetValue(_appName, _appPath);
        }

        public static void RemoveFromStartup()
        {
            _regKey.DeleteValue(_appName, false);
        }

        public static dynamic AddPath(string path, TimeSpan lifespan) 
        {
            var newPath = new PathInfo
            {
                LifeSpan = lifespan,
                Path = path
            };

            var contained = CurrentSettings.Paths.Add(newPath);

            if (contained)
            {
                return new InvalidOperationException();
            }
            return true;
        }

        public static void SetGlobalLifeTime()
        {
            foreach (var path in CurrentSettings.Paths)
            {
                var info = new PathInfo();
                var found = CurrentSettings.Paths.TryGetValue(path, out info);

                if (found)
                {
                    info.LifeSpan = LifeSpan;
                }
            }
        }

        public static void SetSettings(SettingsInfo newInfo)
        {
            _currentSettings = newInfo;
        }

        public static dynamic ReadData()
        {
            var serializer = new XmlSerializer(typeof(SettingsInfo));
            dynamic settings = new SettingsInfo();
            try
            {
                using var stream = new FileStream(_appPath + _fileName, FileMode.Open, FileAccess.Read);
                var reader = new XmlTextReader(stream);
                settings = (SettingsInfo)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                return e;
            }
            return settings;
        }

        public static dynamic SaveData(SettingsInfo info)
        {
            SetSettings(info);
            var infoToSave = CurrentSettings;

            var serializer = new XmlSerializer(typeof(SettingsInfo));
            try
            {
                using var stream = new FileStream(_appPath + _fileName, FileMode.Open, FileAccess.Write);
                var writer = new StreamWriter(_appPath + _fileName);
                serializer.Serialize(writer, infoToSave);
            }
            catch(IOException e)
            {
                return e;
            }
            return true;
        }
    }
}
