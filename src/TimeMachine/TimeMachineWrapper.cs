using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TimeMachine
{
    public class TimeMachineWrapper<T>: DynamicObject, ICloneable where T: new()
    {
        private readonly IDictionary<string, PropertyObserver> trackers;
        private readonly Func<T, object> identify;

        private readonly T entity;
        private readonly IEventJournal eventJournal;
        private readonly ISnapshotStore snapshotStore;

        /// <summary>
        /// Gets an underlying value.
        /// </summary>
        public T Unwrapped { get { return entity; } }

        protected internal TimeMachineWrapper(T entity, IEventJournal eventJournal, ISnapshotStore snapshotStore, IDictionary<string, PropertyObserver> trackers, Func<T, object> identify)
        {
            this.entity = entity;
            this.eventJournal = eventJournal;
            this.snapshotStore = snapshotStore;
            this.trackers = trackers;
            this.identify = identify;
        }

        public void Update(params UpdateEvent[] events)
        {
            // sort them in ascending order and apply 
            Array.Sort(events);
            foreach (var updateEvent in events)
            {
                TryUpdate(updateEvent.PropertyName, updateEvent.Value);
            }
        }

        public void TakeSnapshot()
        {
            
        }

        public TimeMachineWrapper<T> TimeTravel(DateTime pointInTime)
        {
            return TimeTravel(pointInTime, new T());
        }

        public TimeMachineWrapper<T> TimeTravel(DateTime pointInTime, T initialState)
        {
            var entityIdentifier = identify(initialState);
            var events = eventJournal.GetEvents(entityIdentifier, to: pointInTime);

            var copy = Copy(initialState);
            copy.Update(events.ToArray());

            return copy;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TryUpdate(binder.Name, value, emitEvent: true);
        }

        private bool TryUpdate(string propertyName, object value, bool emitEvent = false)
        {
            PropertyObserver observer;
            if (trackers.TryGetValue(propertyName, out observer))
            {
                var e = observer.Set(entity, value);
                if (emitEvent && e != null)
                {
                    e.Identifier = identify(entity);
                    eventJournal.WriteEvent(e);
                }

                return true;
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyObserver observer;
            if (trackers.TryGetValue(binder.Name, out observer))
            {
                result = observer.Get(entity);

                return true;
            }

            result = null;
            return false;
        }

        public object Clone()
        {
            return Copy(entity);
        }

        private TimeMachineWrapper<T> Copy(T state)
        {
            return new TimeMachineWrapper<T>(state, eventJournal, snapshotStore, trackers, identify);
        }
    }
}