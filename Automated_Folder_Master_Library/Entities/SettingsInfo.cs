using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Master_Library.Entities
{
    [XmlRoot("SettingsInfo")]
    public struct SettingsInfo
    {
        [XmlElement("AutoStartPath")]
        public bool Autostart { get; set; }
        [XmlElement("DeleteExes")]
        public bool DeleteExes { get; set; }
        [XmlElement("DeleteFolder")]
        public bool DeleteFolder { get; set; }
        [XmlElement("SendToBin")]
        public bool SendToBin { get; set; }
        [XmlElement("GlobalLifeSpan")]
        public TimeSpan GlobalLifeSpan { get; set; }
        [XmlArray("Paths"), XmlArrayItem("Path")]
        public HashSet<PathInfo> Paths { get; set; }
        public void AddPath(PathInfo info)
        {
            Paths.Add(info);
        }
        public void RemovePath(PathInfo info)
        {
            Paths.Remove(info);
        }
        public void UpdatePath(PathInfo persisted, PathInfo updated)
        {
            RemovePath(persisted);
            AddPath(updated);
        }
        public void UpdateLifeSpans()
        {
            var updatedPaths = new HashSet<PathInfo>();

            foreach (var path in Paths)
            {
                updatedPaths.Add(new PathInfo()
                {
                    Path = path.Path,
                    LifeSpan = GlobalLifeSpan
                });
            }
            Paths = updatedPaths;
        }
    }
}
