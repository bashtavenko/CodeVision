namespace CodeVision.Dependencies.Database
{
    public class CommentProperty : ObjectProperty
    {
        public string Text { get; set; }
        
        public CommentProperty(string text) :
            base (ObjectPropertyType.String)
        {
            Text = text;
        }
    }
}