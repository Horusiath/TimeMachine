using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TimeMachine
{
    public delegate DateTime Clock();

    public class TimeMachine<TEntity> where TEntity : new()
    {
        protected readonly IEventJournal eventJournal;
        protected readonly ISnapshotStore snapshotStore;
        protected readonly Clock clock;
        protected readonly Func<TEntity, object> identifier;
        private IDictionary<string, PropertyObserver> observers;

        public TimeMachine(IEventJournal eventJournal, ISnapshotStore snapshotStore = null, Clock clock = null)
        {
            this.eventJournal = eventJournal;
            this.snapshotStore = snapshotStore;
            this.clock = clock ?? (() => DateTime.Now);
            this.observers = ConstructObservers();
            this.identifier = ConstructIdentifier();
        }

        public TimeMachineWrapper<TEntity> Wrap(TEntity entity)
        {
            return new TimeMachineWrapper<TEntity>(entity, eventJournal, snapshotStore, observers, identifier);
        }

        private Func<TEntity, object> ConstructIdentifier()
        {
            var identifierInfo = typeof(TEntity).GetMembers()
                .FirstOrDefault(info => info.GetCustomAttributes(typeof(IdentifierAttribute)).Any());

            if (identifierInfo != null)
            {
                if (identifierInfo is PropertyInfo) return ConstructIdentifierFromProperty(identifierInfo as PropertyInfo);
                if (identifierInfo is FieldInfo) return ConstructIdentifierFromField(identifierInfo as FieldInfo);

                throw new InvalidOperationException("IdentifierAttribute can be assigned only to fields or properties");
            }
            else
            {
                return ConstructIdentfierFromHashMethod();
            }
        }

        private Func<TEntity, object> ConstructIdentfierFromHashMethod()
        {
            var methodInfo = typeof(TEntity).GetMethod("GetHashCode");
            var fn = (Func<object, int>)methodInfo.CreateDelegate(typeof(Func<object, int>));
            return entity => fn(entity);
        }

        private Func<TEntity, object> ConstructIdentifierFromField(FieldInfo info)
        {
            Func<object, object> fn = info.GetValue;
            return entity => fn(entity);
        }

        private Func<TEntity, object> ConstructIdentifierFromProperty(PropertyInfo info)
        {
            Func<object, object> fn = info.GetValue;
            return entity => fn(entity);
        }

        private static IDictionary<string, PropertyObserver> ConstructObservers()
        {
            var observers = new Dictionary<string, PropertyObserver>();

            foreach (var property in typeof(TEntity).GetProperties())
            {
                var tracker = new PropertyObserver(property);
                observers.Add(property.Name, tracker);
            }

            return observers;
        }
    }
}
