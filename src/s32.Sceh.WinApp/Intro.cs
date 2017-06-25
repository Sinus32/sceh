using s32.Sceh.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace s32.Sceh.WinApp
{
    public partial class Intro : Form
    {
        public Intro()
        {
            InitializeComponent();
        }

        //[DataContract(IsReference = true, Namespace = XmlSerialization.NS_SCEH)]
        //public class Person
        //{
        //    public Person(string firstName, string lastName, Person parent)
        //    {
        //        FirstName = firstName;
        //        LastName = lastName;
        //        Children = new List<Person>();
        //        Parent = parent;
        //    }

        //    [DataMember]
        //    public string FirstName { get; set; }
        //    [DataMember]
        //    public string LastName { get; set; }
        //    [DataMember]
        //    public List<Person> Children { get; set; }
        //    [DataMember]
        //    public Person Parent { get; set; }
        //}

        [XmlRoot("Person", Namespace = XmlSerialization.NS_SCEH)]
        public class Person
        {
            public Person()
            {

            }

            public Person(string firstName, string lastName, Person parent)
            {
                FirstName = firstName;
                LastName = lastName;
                Children = new List<Person>();
            }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public List<Person> Children { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Person kim = new Person("Kim", "Abercrombie", null);
            kim.Children.Add(new Person("Hazem", "Abolrous", kim));
            kim.Children.Add(new Person("Luka", "Abrus", kim));

            try
            {
                //DataContractSerializer dcs = new DataContractSerializer(typeof(Person));
                var serializer = new XmlSerializer(typeof(Person));

                using (var text = new StringWriter())
                using (var xml = XmlWriter.Create(text))
                {
                    //dcs.WriteStartObject(xml, kim);
                    //XmlSerialization.WriteNamespaceAttributes(xml);
                    //dcs.WriteObjectContent(xml, kim);
                    //dcs.WriteEndObject(xml);
                    serializer.Serialize(xml, kim);
                    xml.Flush();
                    var t = text.ToString();
                    MessageBox.Show(t, "Result");
                    Clipboard.SetData(DataFormats.UnicodeText, t);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
            }
        }
    }
}
