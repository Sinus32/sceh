using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace s32.Sceh.Classes
{
    public static class XmlSerialization
    {
        public const string NS_XML = "http://www.w3.org/XML/1998/namespace";
        public const string NS_XS = "http://www.w3.org/2001/XMLSchema";
        public const string NS_XSI = "http://www.w3.org/2001/XMLSchema-instance";
        public const string NS_S = "http://schemas.microsoft.com/2003/10/Serialization/";
        public const string NS_ARR = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        public const string NS_SCEH = "http://schema.s32.prv.pl/2017/04/Sceh";

        public static void WriteNamespaceAttributes(XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("xmlns", "xml", null, NS_XML);
            xmlWriter.WriteAttributeString("xmlns", "xs", null, NS_XS);
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, NS_XSI);
            xmlWriter.WriteAttributeString("xmlns", "s", null, NS_S);
            xmlWriter.WriteAttributeString("xmlns", "arr", null, NS_ARR);
        }
    }
}
