namespace CodeVision.Dependencies.Database
{
    public class CommentProperty : ObjectProperty
    {
        public string Text { get; private set; }
        
        public CommentProperty(string text)
        {
            Text = text;
            PropertyType = ObjectPropertyType.String;
        }
    }
}