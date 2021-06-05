using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ModernSuite.Library.Xml
{
    /// <summary>
    /// A module dependency.
    /// </summary>
    public sealed class Dependency
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; init; }

        [XmlElement(ElementName = "version")]
        public string Version { get; init; }
    }
}
