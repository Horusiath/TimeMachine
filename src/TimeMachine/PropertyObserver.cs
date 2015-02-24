using System;
using System.Linq;
using System.Reflection;

namespace TimeMachine
{
    public class PropertyObserver
    {
        private Func<object, object> getter;
        private Action<object, object> setter;
        private string name;
        private bool isTracked;

        public PropertyObserver(PropertyInfo property)
        {
            isTracked = property.CustomAttributes.All(attr => attr.AttributeType != typeof (DoNotTrackAttribute));
            name = property.Name;
            getter = property.GetValue;
            setter = property.SetValue;
        }

        public UpdateEvent Set(object entity, object value)
        {
            setter(entity, value);
            return isTracked ? new UpdateEvent(name, value, DateTime.Now) : null;
        }

        public object Get(object entity)
        {
            return getter(entity);
        }
    }
}