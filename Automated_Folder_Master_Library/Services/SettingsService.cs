using Master_Library.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Master_Library.Services
{
    public static class SettingsService
    {
        
        private static SettingsInfo _currentSettings;
        private static readonly string _saveFilePath = @"C:\Users\Public\Documents\";
        private static readonly string _fileName = "settings.xml";
        private static readonly string _appName = "Automated Folder Master Console";
        private static string _appPath;

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
            _appPath = GetExecutingConsoleDirectory();
            var key = OpenKey();
            key.SetValue(_appName, _appPath);
            key.Dispose();
        }

        public static void RemoveFromStartup()
        {
            var key = OpenKey();
            key.SetValue(_appName, false);
            key.Dispose();
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

            using var stream = new FileStream(string.Concat(_saveFilePath, _fileName), FileMode.Open, FileAccess.Read);
            var reader = new XmlTextReader(stream);
            return (SettingsInfo)serializer.Deserialize(reader);
        }

        public static void SaveData()
        {
            var serializer = new XmlSerializer(typeof(SettingsInfo));

            using var writer = new StreamWriter(string.Concat(_saveFilePath, _fileName));
            serializer.Serialize(writer, _currentSettings);
        }

        private static RegistryKey OpenKey()
        {
            var keyLocation = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            return Registry.CurrentUser.CreateSubKey(keyLocation, true);
        }
        private static string GetExecutingConsoleDirectory()
        {
            var target = "";
            var current = Directory.GetCurrentDirectory();
            var parentDir = Directory.GetParent(current);
            var targetDirChildren = Directory.GetDirectories(parentDir.FullName).ToList();
                
            targetDirChildren.ForEach((dir) =>
            {
                if (dir.ToLower().Contains("console"))
                {
                    target = dir;
                }
            });

            var files = Directory.GetFiles(target).ToList();
            files.ForEach((file) =>
            {
                if (Path.GetExtension(file).Equals(".exe"))
                {
                    target = file;
                }
            });

            return target;
        }
    }
}
