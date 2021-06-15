using ModernSuite.Library.IR;
using System.Xml.Serialization;

namespace ModernSuite.Library.Xml
{
    /// <summary>
    /// A module.
    /// </summary>
    public sealed class Module
    {
        public string Name { get; init; }
        public string Version { get; init; }
        public Debug Debug { get; init; }
        [XmlArray("Code")]
        public Operation[] Code { get; init; }
        [XmlArray("Dependencies")]
        public Dependency[] Dependencies { get; init; }
    }
}
