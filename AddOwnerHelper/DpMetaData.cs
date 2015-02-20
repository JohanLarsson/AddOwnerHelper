namespace AddOwnerHelper
{
    using System;
    using System.CodeDom.Compiler;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    public class DpMetaData
    {
        private readonly FieldInfo _dpField;

        private DependencyProperty _dependencyProperty;

        public DpMetaData(FieldInfo dpField)
        {
            _dpField = dpField;
            _dependencyProperty = (DependencyProperty)dpField.GetValue(null);
            FrameworkPropertyMetadata = _dependencyProperty.DefaultMetadata as FrameworkPropertyMetadata;
            Metadata = _dependencyProperty.DefaultMetadata;
        }

        public FrameworkPropertyMetadata FrameworkPropertyMetadata { get; private set; }

        public PropertyMetadata Metadata { get; private set; }

        public string DefaultValue
        {
            get
            {
                if (Metadata.DefaultValue == null)
                {
                    return string.Format("default({0})", _dependencyProperty.PropertyType.Name);
                }
                var type = Metadata.DefaultValue.GetType();
                if (type.IsEnum)
                {
                    return type.Name + "." + Metadata.DefaultValue;
                }
                if (type == typeof(double))
                {
                    return ((double)Metadata.DefaultValue).ToString("F1", CultureInfo.InvariantCulture);
                }
                return Metadata.DefaultValue.ToString();
            }
        }

        public bool HasMetadataOptions
        {
            get
            {
                return FrameworkPropertyMetadata != null;
            }
        }

        public bool HasPropertyChangedCallback
        {
            get
            {
                return Metadata.PropertyChangedCallback != null;
            }
        }

        public bool HasCoerceValueCallback
        {
            get
            {
                return Metadata.CoerceValueCallback != null;
            }
        }

        public string MetaDataOptions
        {
            get
            {
                if (FrameworkPropertyMetadata == null)
                {
                    return "";
                }
                var props = Enum.GetValues(typeof(FrameworkPropertyMetadataOptions))
                                .Cast<FrameworkPropertyMetadataOptions>()
                                .Select(x => GetPropAndValue(FrameworkPropertyMetadata, x))
                                .ToArray();

                if (props.Any(x => x.Value))
                {
                    var propAndValues = props.Where(x => x.Value)
                                             .ToArray();
                    return string.Join(
                        " | ",
                        propAndValues.Select(x => string.Format("FrameworkPropertyMetadataOptions.{0}", x.Option)));
                }
                else
                {
                    return string.Format("FrameworkPropertyMetadataOptions.{0}", FrameworkPropertyMetadataOptions.None);
                }
            }
        }


        private IndentedTextWriter WriteCtorArgs(IndentedTextWriter writer, FrameworkPropertyMetadata frameworkPropertyMetadata, string propType)
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

        private IndentedTextWriter WriteObjectInitializer(IndentedTextWriter writer, FrameworkPropertyMetadata frameworkPropertyMetadata)
        {
            if (!frameworkPropertyMetadata.IsAnimationProhibited &&
                 frameworkPropertyMetadata.DefaultUpdateSourceTrigger != UpdateSourceTrigger.Default &&
                !frameworkPropertyMetadata.BindsTwoWayByDefault &&
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


        private static PropAndValue GetPropAndValue(FrameworkPropertyMetadata metadata, FrameworkPropertyMetadataOptions option)
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
                PropertyInfo = propertyInfo;
                Value = value;
                Option = option;
            }
            public FrameworkPropertyMetadataOptions Option { get; private set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public bool Value { get; private set; }
        }
    }
}