using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Master_Library.Entities
{
    public struct SettingsInfo
    {
        public string AutoStartPath { get; set; }
        public bool DeleteExes { get; set; }
        public bool SendToBin { get; set; }
        public TimeSpan GlobalLifeSpan { get; set; }
        public HashSet<PathInfo> Paths { get; set; }
    }
}
