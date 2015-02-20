namespace AddOwnerHelper.Tests
{
    using System;
    using System.Linq;
    using System.Windows;

    using NUnit.Framework;

    public class DpMetaDataTests
    {

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
