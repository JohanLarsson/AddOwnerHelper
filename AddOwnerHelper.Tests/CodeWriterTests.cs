namespace AddOwnerHelper.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Shapes;

    using NUnit.Framework;

    [TestFixture]
    public class CodeWriterTests
    {
        private StringBuilder _stringBuilder;
        private IndentedTextWriter _writer;

        [SetUp]
        public void SetUp()
        {
            _stringBuilder = new StringBuilder();
            var textWriter = new StringWriter(_stringBuilder);
            var tabString = "    ";
            _writer = new IndentedTextWriter(textWriter, tabString) { Indent = 2 };
            _writer.Write(tabString);
            _writer.Write(tabString);
        }

        [Test]
        public void Write()
        {
            var fieldInfo = typeof(TextBoxBase).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Single(x => x.Name == "IsReadOnlyProperty");
            _writer.WriteAddOwnerField(fieldInfo, "NewType");
            _writer.WriteLine();
            _writer.WriteAddOwnerProperty(fieldInfo, "NewType");
            var code = _stringBuilder.ToString();
            Console.Write(code);
        }

        [Test]
        public void WriteAddOwnerTextBoxBaseIsReadOnlyField()
        {
            var fieldInfo = typeof(TextBoxBase).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Single(x => x.Name == "IsReadOnlyProperty");
            _writer.WriteAddOwnerField(fieldInfo, "MyControl");
            var code = _stringBuilder.ToString();
            Console.Write(code);
            var expected = Properties.Resources.AddOwnerTextBoxBaseIsReadOnlyField + Environment.NewLine;
            Assert.AreEqual(expected, code);
        }

        [Test(Description = "Dunno why I get PropertyMetadata and not FrameworkPropertyMetadata here")]
        public void WriteAddOwnerShapeStretchField()
        {
            var fieldInfo = typeof(Shape).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Single(x => x.Name == "StretchProperty");
            _writer.WriteAddOwnerField(fieldInfo, "MyControl");
            var code = _stringBuilder.ToString();
            Console.Write(code);
            Assert.AreEqual(Properties.Resources.AddOwnerShapeStretchField, code);
        }

        [Test]
        public void WriteAddOwnerShapeStretchProperty()
        {
            var fieldInfo = typeof(Shape).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Single(x => x.Name == "StretchProperty");
            _writer.WriteAddOwnerProperty(fieldInfo, "MyControl");
            var code = _stringBuilder.ToString();
            Console.Write(code);
            var expected = Properties.Resources.AddOwnerShapeStretchProperty + Environment.NewLine;
            Assert.AreEqual(expected, code);
        }
    }
}
