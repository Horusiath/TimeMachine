using System;

namespace TimeMachine
{
    [Serializable]
    public class UpdateEvent : IComparable<UpdateEvent>
    {
        public UpdateEvent()
        {
        }

        public UpdateEvent(string propertyName, object value, DateTime timestamp)
        {
            PropertyName = propertyName;
            Value = value;
            Timestamp = timestamp;
        }

        public DateTime Timestamp { get; set; }
        public object Identifier { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public int CompareTo(UpdateEvent other)
        {
            return Timestamp.CompareTo(other);
        }
    }
}