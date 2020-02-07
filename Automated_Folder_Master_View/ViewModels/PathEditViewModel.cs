using Master_Library.Entities;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Master_View.ViewModels
{
    public class PathEditViewModel : Base
    {
        private PathInfo _info;
        private PathInfo _persisted;
        private SettingsInfo _settings;
        private Window _window;
        private ObservableCollection<PathInfo> _obsPaths;
        public string Path
        {
            get => _info.Path;
            set { OnPropertyChanged(); _info.Path = value; }
        }
        public double LifeSpan 
        {
            get => _info.LifeSpan.TotalDays;
            set { OnPropertyChanged(); _info.LifeSpan = TimeSpan.FromDays(value); }
        }
        public PathEditViewModel(PathInfo info, Window window, SettingsInfo settings, ObservableCollection<PathInfo> obs)
        {
            _info = info;
            _persisted = info;
            _settings = settings;
            _window = window;
            _obsPaths = obs;
            SetCommands();
        }

        public DelegateCommand<string> SaveCommand { get; private set; }
        public DelegateCommand<string> RemoveCommand { get; private set; }
        public DelegateCommand<string> CancelCommand { get; private set; }

        private void SetCommands()
        {
            SaveCommand = new DelegateCommand<string>(Save_Execute);
            RemoveCommand = new DelegateCommand<string>(Remove_Execute);
            CancelCommand = new DelegateCommand<string>(Cancel_Execute);
        }

        private void Cancel_Execute(string filler)
        {
            _window.Close();
        }

        private void Remove_Execute(string filler)
        {
            _settings.RemovePath(_persisted);
            _obsPaths.Remove(_persisted);
            Cancel_Execute(string.Empty);
        }

        private void Save_Execute(string filler)
        {
            var contains = false;
            foreach (var info in _settings.Paths)
            {
                if (info.Path.Equals(_info.Path) && info.LifeSpan.Equals(_info.LifeSpan))
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                _settings.UpdatePath(_persisted, _info);

                var index = _obsPaths.IndexOf(_persisted);
                _obsPaths.Remove(_persisted);
                _obsPaths.Insert(index, _info);

                _persisted = _info;
            }
        }
    }
}
