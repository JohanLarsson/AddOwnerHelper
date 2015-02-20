namespace AddOwnerHelper.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Shapes;

    using NUnit.Framework;

    public class DpMetaDataTests
    {
        [TestCase(typeof(Shape), "StretchProperty")]
        [TestCase(typeof(TextBoxBase), "IsReadOnlyProperty")]
        public void DumpMetadataType(Type type, string name)
        {
            var dependencyProperty = this.GetDp(type, name);
            Console.WriteLine("{0}.{1}: {2}", type.FullName, name, dependencyProperty.DefaultMetadata.GetType().Name);
        }

        private DependencyProperty GetDp(Type type, string name)
        {
            var fieldInfo = type.GetField(name, BindingFlags.Public | BindingFlags.Static);
            var dp = (DependencyProperty)fieldInfo.GetValue(null);
            return dp;
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
