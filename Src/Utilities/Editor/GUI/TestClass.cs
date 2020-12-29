using System.Xml.Serialization;

namespace Utilities.Editor.GUI {
    class TestClass {
        [XmlAttribute]
   //     [EditorString(notnull = true, description = "this is the project name", allowedCharacters = "A-Z,A-z,_")]
        public string ProjectName { get; set; }

        //[ValidationMethod(warning = false)]
        public string ValidateProjectName() {
            return string.Empty;
        }

     // return error message or null to accept


        // a set of innards
      //  [XmlArray("Nodes", IsNullable = false,)]
     //   public List<Innard> innards {get; set;}
    }

    [XmlRoot("Innard")]
    public class Innard {
        
    }
}
