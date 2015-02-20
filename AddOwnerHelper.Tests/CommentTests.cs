namespace AddOwnerHelper.Tests
{
    using System.Linq;
    using System.Reflection;
    using System.Windows.Shapes;

    using NUnit.Framework;

    public class CommentTests
    {

        [Test]
        public void StretchFieldComment()
        {
            var fieldInfo = typeof(Shape).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Single(x => x.Name == "StretchProperty");
            var fieldComment = Comment.CreateFieldComment(fieldInfo);
            CollectionAssert.AreEqual(new[] { @"Identifies the <see cref=""P:System.Windows.Shapes.Shape.Stretch"" /> dependency property. " }, fieldComment.Summaries);
            CollectionAssert.AreEqual(new[] { @"The identifier for the <see cref=""P:System.Windows.Shapes.Shape.Stretch"" /> dependency property." }, fieldComment.Summaries);
        }
    }
}
