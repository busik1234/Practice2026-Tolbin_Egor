using task05;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;
        private string _privateField;
        public int Property { get; set; }

        public void Method() { }
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
        }
        [Fact]
        public void GetMethodParams_ReturnsEmpty_WhenMethodHasNoParams()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var parameters = analyzer.GetMethodParams("Method");

            Assert.Empty(parameters);
        }
        [Fact]
        public void GetMethodParams_ReturnsEmpty_IfMethodNotFound()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var parameters = analyzer.GetMethodParams("NonExistentMethod");

            Assert.Empty(parameters);
        }

        [Fact]
        public void GetProperties_ReturnsCorrectProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.Contains("Property", properties);
        }

        [Fact]
        public void HasAttribute_ReturnsTrue_WhenAttributeExists()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

            Assert.True(hasAttribute);
        }

        [Fact]
        public void HasAttribute_ReturnsFalse_WhenAttributeDoesNotExist()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

            Assert.False(hasAttribute);
        }
    }
}
