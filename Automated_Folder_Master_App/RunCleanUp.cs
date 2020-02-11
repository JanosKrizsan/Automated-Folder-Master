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
    public class RunCleanUp
    {
        private SettingsInfo _settings;
        private List<PathInfo> _folders;
        private IOHandlingService _IOService = new IOHandlingService();

        public void DoCleanup()
        {
            _folders.ForEach((folder) => DetermineDeletion(folder));

            ShutdownErrorService();
        }

        public void ManualSetup()
        {
            _IOService.Introduction();

            _settings = new SettingsInfo();

            _settings.DeleteExes = true;
            _settings.Paths = new HashSet<PathInfo>();
            _settings.DeleteFolder = _IOService.YesOrNoInput(IOHandlingService.FolderDelOrNot);
            _settings.SendToBin = _IOService.YesOrNoInput(IOHandlingService.BinOrNot);

            _settings.GlobalLifeSpan = _IOService.LifeSpanInput();

            var path = new PathInfo
            {
                Path = _IOService.PathInput(),
                LifeSpan = _settings.GlobalLifeSpan
            };

            _settings.Paths.Add(path);

            _folders = _settings.Paths?.ToList();

            _IOService.SuccessConfirmer();

        }

        public void AutomaticSetup()
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

        private void DetermineDeletion(PathInfo folder)
        {
            var lifeSpan = folder.LifeSpan;
            var directory = folder.Path;
            var folderCreationDate = Directory.GetCreationTime(directory);
            var files = Directory.GetFiles(directory);
            
            if (!_settings.DeleteExes && files.Contains(".exe"))
            {
                return;
            }


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

        private void DeleteFile(string path)
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

        private void DeleteDirectory(string path, string[] files)
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

        private List<PathInfo> ReadFolders()
        {
            return _settings.Paths?.ToList();
        }

        private void ReadSettings()
        {
            var info = new SettingsInfo();

            try
            {
                info = SettingsService.ReadData();
            }
            catch(Exception e)
            {
                ErrorHandlingService.ExceptionHandler(e);
            }

            _settings = info;
        }

        private void ShutdownErrorService()
        {
            ErrorHandlingService.Shutdown();
        }
    }
}
