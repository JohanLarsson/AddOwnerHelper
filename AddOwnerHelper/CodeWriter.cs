namespace AddOwnerHelper
{
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;

    public static class CodeWriter
    {
        public static IndentedTextWriter WriteAddOwnerField(this IndentedTextWriter writer, FieldInfo dp, string newOwner)
        {
            writer.WriteFieldComment(dp, newOwner);
            writer.WriteLine("public static readonly DependencyProperty {0} = {1}.{0}.AddOwner(", dp.Name, dp.DeclaringType.Name);
            writer.Indent++;
            writer.WriteLine("typeof({0}),", newOwner);
            writer.WritePropertyMetadata(dp);
            writer.Indent--;
            return writer;
        }

        public static IndentedTextWriter WriteAddOwnerProperty(this IndentedTextWriter writer, FieldInfo dpField, string newOwner)
        {
            var dp = (DependencyProperty)dpField.GetValue(null);
            writer.WritePropertyComment(dpField, newOwner);
            writer.WriteLine("public {0} {1}", AliasName(dp.PropertyType.Name), dp.Name);
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine(@"get {{ return ({0})GetValue({1}); }}", AliasName(dp.PropertyType.Name), dpField.Name);
            writer.WriteLine(@"set {{ SetValue({0}, value); }}", dpField.Name);
            writer.Indent--;
            writer.WriteLine("}");
            return writer;
        }

        private static IndentedTextWriter WritePropertyMetadata(this IndentedTextWriter writer, FieldInfo dpField)
        {
            var dpMetaData = new DpMetaData(dpField);
            if (dpMetaData.FrameworkPropertyMetadata != null)
            {
                writer.WriteLine("new {0}(", dpMetaData.FrameworkPropertyMetadata.GetType().Name);
            }
            else
            {
                writer.Write("new {0}(", dpMetaData.Metadata.GetType().Name);
            }
            writer.Indent++;
            writer.Write(AliasName(dpMetaData.DefaultValue));
            if (dpMetaData.HasMetadataOptions)
            {
                writer.WriteLine(",");
                writer.Write(dpMetaData.MetaDataOptions);
            }
            writer.Write("));");
            writer.Indent--;
            writer.WriteLine();
            return writer;
        }

        private static IndentedTextWriter WriteFieldComment(this IndentedTextWriter writer, FieldInfo dp, string newOwner)
        {
            var comment = Comment.CreateFieldComment(dp);
            return writer.WriteComment(comment, dp, newOwner);
        }

        private static IndentedTextWriter WritePropertyComment(this IndentedTextWriter writer, FieldInfo dp, string newOwner)
        {
            var comment = Comment.CreatePropertyComment(dp);
            return writer.WriteComment(comment, dp, newOwner);
        }

        private static IndentedTextWriter WriteComment(this IndentedTextWriter writer, Comment comment, FieldInfo dp, string newOwner)
        {
            writer.WriteLine(@"/// <summary>");
            foreach (var summary in comment.Summaries)
            {
                writer.WriteLine(@"/// " + summary.Replace(dp.DeclaringType.FullName, newOwner));
            }
            writer.WriteLine(@"/// </summary>");
            if (comment.Returns != null && comment.Returns.Any())
            {
                writer.WriteLine(@"/// <returns>");
                foreach (var @return in comment.Returns)
                {
                    writer.WriteLine(@"/// " + @return.Replace(dp.DeclaringType.FullName, newOwner));
                }
                writer.WriteLine(@"/// </returns>");
            }
            return writer;
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
                case "Boolean":
                    return "bool";
                case "True":
                    return "true";
                case "False":
                    return "false";
                case "Object":
                    return "object";
            }
            return name;
        }
    }
}
