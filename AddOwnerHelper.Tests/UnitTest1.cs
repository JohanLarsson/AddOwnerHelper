namespace AddOwnerHelper.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    using NUnit.Framework;

    [TestFixture]
    public class CodeWriterTests
    {
        [Test]
        public void Write()
        {
            // TextBox.IsReadOnlyProperty
            var fieldInfo = typeof(TextBoxBase).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Single(x => x.Name == "IsReadOnlyProperty");
            var code = CodeWriter.Write(fieldInfo, "NewType");
            Console.Write(code);
        }

        [Test]
        public void DumpFrameworkPropertyMetadataOptions()
        {
            var strings = Enum.GetNames(typeof(FrameworkPropertyMetadataOptions));
            foreach (var s in strings)
            {
                Console.WriteLine(s);
            }
        }

        [Test]
        public void DumpFrameworkPropertyMetadataProperties()
        {
            var props = typeof(FrameworkPropertyMetadata).GetProperties();
            var names = Enum.GetNames(typeof(FrameworkPropertyMetadataOptions));
            Console.WriteLine("All properties:");
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Name);
            }
            Console.WriteLine();
            Console.WriteLine("Filtered properties:");
            foreach (var p in props.Where(x => !names.Any(n => x.Name.Contains(n))))
            {
                Console.WriteLine(p.Name);
            }
        }
    }
}
