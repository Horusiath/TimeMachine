using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TimeMachine.Journals
{
    public class MemoryJournal : IEventJournal
    {
        private readonly ConcurrentDictionary<object, ISet<UpdateEvent>> memoryMap;

        public MemoryJournal()
        {
            memoryMap = new ConcurrentDictionary<object, ISet<UpdateEvent>>();
        }

        public void WriteEvent(UpdateEvent e)
        {
            AddEvent(e.Identifier, e);
        }

        public IEnumerable<UpdateEvent> GetEvents(object id, DateTime? from = null, DateTime? to = null)
        {
            ISet<UpdateEvent> events;
            if (memoryMap.TryGetValue(id, out events))
            {
                var start = from ?? DateTime.MinValue;
                var end = to ?? DateTime.MaxValue;
                return events.Where(e => e.Timestamp <= end && e.Timestamp >= start);
            }

            return Enumerable.Empty<UpdateEvent>();
        }

        private void AddEvent(object key, UpdateEvent updateEvent)
        {
            var events = memoryMap.GetOrAdd(key, new SortedSet<UpdateEvent>());
            events.Add(updateEvent);
        }
    }
}