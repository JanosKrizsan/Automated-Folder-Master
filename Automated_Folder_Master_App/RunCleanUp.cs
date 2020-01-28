using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Master_Console.Services;
using Master_Library.Entities;
using Master_Library.Services;
using Microsoft.VisualBasic.FileIO;

namespace Master_Console
{
    public static class RunCleanUp
    {
        private static SettingsInfo _settings;
        private static List<PathInfo> _folders;

        public static void DoCleanup()
        {
            _folders.ForEach((folder) => DetermineDeletion(folder));

            ShutdownErrorService();
        }

        public static void ManualSetup()
        {
            IOHandlingService.Introduction();

            _settings = new SettingsInfo();

            _settings.AutoStartPath = string.Empty;
            _settings.DeleteExes = true;
            _settings.Paths = new HashSet<PathInfo>();
            _settings.DeleteFolder = IOHandlingService.YesOrNoInput(IOHandlingService.FolderDelOrNot);
            _settings.SendToBin = IOHandlingService.YesOrNoInput(IOHandlingService.BinOrNot);

            _settings.GlobalLifeSpan = IOHandlingService.LifeSpanInput();

            var path = new PathInfo
            {
                Path = IOHandlingService.PathInput(),
                LifeSpan = _settings.GlobalLifeSpan
            };

            _settings.Paths.Add(path);

            _folders = _settings.Paths?.ToList();

            IOHandlingService.SuccessConfirmer();

        }

        public static void AutomaticSetup()
        {
            ReadSettings();
            var folders = ReadFolders();

            if (folders == null || folders.Count == 0)
            {
                ErrorHandlingService.ExceptionHandler(new InvalidDataException());
            }
            else if (folders.Count >= 1)
            {
                _folders = folders;
            }
        }

        private static void DetermineDeletion(PathInfo folder)
        {
            var lifeSpan = folder.LifeSpan;
            var directory = folder.Path;
            var folderCreationDate = Directory.GetCreationTime(directory);
            var files = Directory.GetFiles(directory);
            
            if (!_settings.DeleteExes && files.Contains(".exe"))
            {
                return;
            }

            //TODO, think about directory deletion, if needed or not, can be option too

            var timeToDelFolder = lifeSpan < DateTime.Now.Subtract(folderCreationDate);

            if (_settings.DeleteFolder && timeToDelFolder)
            {
                DeleteDirectory(directory, files);
                return;
            }

            foreach (var file in files)
            {
                var timeToDel = lifeSpan < DateTime.Now.Subtract(File.GetCreationTime(file));
                if (timeToDel)
                {
                    DeleteFile(file);
                }
            }
        }

        private static void DeleteFile(string path)
        {
            if (_settings.SendToBin)
            {
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                File.Delete(path);
            }
        }

        private static void DeleteDirectory(string path, string[] files)
        {
            if (_settings.SendToBin)
            {
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                for (var i = 0; i < files.Length; i++)
                {
                    DeleteFile(files[i]);
                }
                Directory.Delete(path);
            }
        }

        private static List<PathInfo> ReadFolders()
        {
            return _settings.Paths?.ToList();
        }

        private static void ReadSettings()
        {
            dynamic info = SettingsService.ReadData();

            if (info is Exception)
            {
                ErrorHandlingService.ExceptionHandler(info);
                return;
            }
            
            _settings = info;
        }

        private static void ShutdownErrorService()
        {
            ErrorHandlingService.Shutdown();
        }
    }
}
