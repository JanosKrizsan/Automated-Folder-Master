using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;

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

        //public override bool Equals(object info)
        //{
        //    var other = (SettingsInfo)info;
        //    return (Autostart.Equals(other.Autostart.ToString())
        //        && DeleteExes.Equals(other.DeleteExes)
        //        && DeleteFolder.Equals(other.DeleteFolder)
        //        && SendToBin.Equals(other.SendToBin)
        //        && GlobalLifeSpan.Equals(other.GlobalLifeSpan)
        //        && Paths.Equals(other.Paths));
        //}
    }
}
