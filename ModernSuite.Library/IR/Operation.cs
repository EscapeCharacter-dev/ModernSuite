using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ModernSuite.Library.IR
{
    public class Operation
    {
        [XmlAttribute("a")]
        public string ResultIdentifier;
        [XmlAttribute("b")]
        public string Operator;
        [XmlAttribute("c")]
        public string FirstOperand;
        [XmlAttribute("d")]
        public string SecondOperand;
    }
}
