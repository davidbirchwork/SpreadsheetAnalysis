using Utilities.Editor.Editors;

namespace Utilities_Test.Editor {
    
    public class TestClass {
        [StringEditor("Test Field", "A test field", true, AllowedCharacters = "a-z{}")]
        public string Name { get; set; }
    }
}
