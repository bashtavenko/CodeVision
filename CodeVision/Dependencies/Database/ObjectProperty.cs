namespace CodeVision.Dependencies.Database
{
    public abstract class ObjectProperty
    {
        public ObjectPropertyType PropertyType { get; }

        protected ObjectProperty(ObjectPropertyType propertyType)
        {
            PropertyType = propertyType;
        }

        public override bool Equals(object obj)
        {
            var anotherProperty = obj as ObjectProperty;
            return anotherProperty != null && anotherProperty.PropertyType == PropertyType;
        }

        public override int GetHashCode()
        {
            return PropertyType.GetHashCode();
        }
    }
}
