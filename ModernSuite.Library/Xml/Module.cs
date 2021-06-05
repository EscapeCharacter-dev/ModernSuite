using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ModernSuite.Library.Xml
{
    /// <summary>
    /// A module.
    /// </summary>
    public sealed class Module
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; init; }
        [XmlElement(ElementName = "version")]
        public string Version { get; init; }
        public Debug Debug { get; init; }
        public Code Code { get; init; }
        [XmlArray("Dependencies")]
        public Dependency[] Dependencies { get; init; }
    }
}
