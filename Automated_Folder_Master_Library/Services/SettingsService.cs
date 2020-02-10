﻿using Master_Library.Entities;
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

        public static void AddToStartup()
        {
            _regKey.SetValue(_appName, _appPath);
        }

        public static void RemoveFromStartup()
        {
            _regKey.DeleteValue(_appName, false);
        }

        public static void SetGlobalLifeTime()
        {
            _currentSettings.UpdateLifeSpans();
        }

        public static void SetData(SettingsInfo info, bool SetGlobal)
        {
            _currentSettings = info;
            switch (info.Autostart)
            {
                case true:
                    AddToStartup();
                    break;
                case false:
                    RemoveFromStartup();
                    break;
            }

            if (SetGlobal)
            {
                SetGlobalLifeTime();
            }
        }

        public static SettingsInfo ReadData()
        {
            var serializer = new XmlSerializer(typeof(SettingsInfo));
            var settings = new SettingsInfo();

            using var stream = new FileStream(string.Concat(_saveFilePath, _fileName), FileMode.Open, FileAccess.Read);
            var reader = new XmlTextReader(stream);
            settings = (SettingsInfo)serializer.Deserialize(reader);

            return settings;
        }

        public static void SaveData()
        {
            var serializer = new XmlSerializer(typeof(SettingsInfo));

            using var writer = new StreamWriter(string.Concat(_saveFilePath, _fileName));
            serializer.Serialize(writer, _currentSettings);
        }
        private static string GetExecutingConsoleDirectory()
        {
            var parentDir = Directory.GetParent(Directory.GetCurrentDirectory());
            var targetDirChildren = Directory.GetDirectories(parentDir.FullName);

            foreach (var folder in targetDirChildren)
            {
                if (folder.Contains("Console"))
                {
                    return folder;
                }
            }
            return string.Empty;
        }
    }
}
