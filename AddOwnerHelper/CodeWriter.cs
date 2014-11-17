namespace AddOwnerHelper
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;

    public static class CodeWriter
    {
        public static string Write(FieldInfo field, string newOwner)
        {
            var template = new AddOwnerTemplate();
            var dependencyProperty = (DependencyProperty)field.GetValue(null);
            var propType = AliasName(dependencyProperty.PropertyType.Name);
            var metaData = MetaData(dependencyProperty.DefaultMetadata, propType);

            return template.WriteCode(
                    field.DeclaringType.Name,
                    newOwner,
                    field.Name.Replace("Property", ""),
                    propType,
                    metaData);
        }


        private static string AliasName(string name)
        {
            switch (name)
            {
                case "Double":
                    return "double";
                case "Int":
                    return "int";
                case "String":
                    return "string";
                case "Object":
                    return "object";
            }
            return name;
        }

        public static string MetaData(PropertyMetadata propertyMetadata, string propType)
        {
            var frameworkPropertyMetadata = propertyMetadata as FrameworkPropertyMetadata;
            if (frameworkPropertyMetadata == null)
            {
                //new PropertyMetadata(default(object))
                return string.Format("new PropertyMetadata(default({0}))", propType);
            }
            else
            {
                //new FrameworkPropertyMetadata(
                //    null,
                //    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)
                //    {
                //        DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default
                //    };
                //new FrameworkPropertyMetadata
                //    {
                //        AffectsArrange = true,
                //        AffectsMeasure = true,
                //        AffectsParentArrange = true,
                //        AffectsParentMeasure = true,
                //        AffectsRender = true,
                //        DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default,
                //        BindsTwoWayByDefault = true,
                //        DefaultValue = default(object),
                //        Inherits = true,
                //        IsNotDataBindable = true,
                //        OverridesInheritanceBehavior = true,
                //        CoerceValueCallback = null,
                //        IsAnimationProhibited = true,
                //        Journal = true,
                //        PropertyChangedCallback = null,
                //        SubPropertiesDoNotAffectRender = true
                //    };

                var stringBuilder = new StringBuilder();
                var writer = new IndentedTextWriter(new StringWriter(stringBuilder));
                writer.WriteLine("new FrameworkPropertyMetadata(");
                writer.Indent += 3;
                writer.WriteCtorArgs(frameworkPropertyMetadata, propType);

                //writer.WriteLine("{");
                //writer.Indent++;



                //writer.Indent--;
                //writer.WriteLine("};");
                return stringBuilder.ToString();
            }
        }

        private static IndentedTextWriter WriteCtorArgs(
            this IndentedTextWriter writer,
            FrameworkPropertyMetadata frameworkPropertyMetadata, string propType)
        {
            writer.WriteLine("default({0}),", propType);

            var props = Enum.GetValues(typeof(FrameworkPropertyMetadataOptions))
                            .Cast<FrameworkPropertyMetadataOptions>()
                            .Select(x => GetPropAndValue(frameworkPropertyMetadata, x))
                            .ToArray();

            if (props.Any(x => x.Value))
            {
                var propAndValues = props.Where(x => x.Value).ToArray();
                for (var index = 0; index < propAndValues.Length; index++)
                {
                    var propAndValue = propAndValues[index];
                    writer.Write("FrameworkPropertyMetadataOptions.{0}", propAndValue.Option);
                    if (index < (propAndValues.Length - 1))
                    {
                        writer.Write(" | ");
                    }
                }
            }
            else
            {
                writer.WriteLine("FrameworkPropertyMetadataOptions.{0})", FrameworkPropertyMetadataOptions.None);
            }

            writer.Write(")");
            return writer;
        }

        private static IndentedTextWriter WriteObjectInitializer(
            this IndentedTextWriter writer,
            FrameworkPropertyMetadata frameworkPropertyMetadata)
        {
            if (!frameworkPropertyMetadata.IsAnimationProhibited &&
                 frameworkPropertyMetadata.DefaultUpdateSourceTrigger != UpdateSourceTrigger.Default &&
                !frameworkPropertyMetadata.BindsTwoWayByDefault&&
                !frameworkPropertyMetadata.IsAnimationProhibited)
            {
                return writer;
            }
            writer.WriteLine();

            writer.WriteLine("{");
            writer.Indent++;

            writer.Indent--;
            writer.WriteLine("{");
            return writer;
        }

        private static PropAndValue GetPropAndValue(this FrameworkPropertyMetadata metadata, FrameworkPropertyMetadataOptions option)
        {
            if (option == FrameworkPropertyMetadataOptions.None)
            {
                return new PropAndValue(null, false, option);
            }
            var propertyInfo = typeof(FrameworkPropertyMetadata).GetProperty(option.ToString()) ??
                               typeof(FrameworkPropertyMetadata).GetProperty("Is" + option.ToString());

            var value = (bool)propertyInfo.GetValue(metadata);
            return new PropAndValue(propertyInfo, value, option);
        }

        public class PropAndValue
        {
            public PropAndValue(PropertyInfo propertyInfo, bool value, FrameworkPropertyMetadataOptions option)
            {
                this.PropertyInfo = propertyInfo;
                this.Value = value;
                this.Option = option;
            }
            public FrameworkPropertyMetadataOptions Option { get; private set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public bool Value { get; private set; }
        }
    }
}
