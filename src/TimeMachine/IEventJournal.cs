using System;
using System.Collections.Generic;

namespace TimeMachine
{
    public interface IEventJournal
    {
        void WriteEvent(UpdateEvent e);

        IEnumerable<UpdateEvent> GetEvents(object id, DateTime? from = null, DateTime? to = null);
    }
}