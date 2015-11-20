namespace CodeVision.Dependencies.Database
{
    public class CommentProperty : ObjectProperty
    {
        public string Text { get; set; }

        public override ObjectPropertyType PropertyType => ObjectPropertyType.String;

        public CommentProperty(string text)
        {
            Text = text;
        }
    }
}