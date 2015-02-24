# .NET TimeMachine (WORK IN PROGRESS)

**TimeMachine** is a simple, universal property tracker for your objects. What is more important it's able to store all changes in form of serializable change events, restore object state from stored events and recreate it's state at the specific point in time.

### Quick start

```csharp
var entity = new MyEntity();

// create time machine for MyEngine types using in-memory event journal
var timeMachine = new TimeMachine<MyEntity>(new MemoryJournal());

// create wrapper around entity
var wrapper = timeMachine.Wrap(entity);
// change underlying value with event stored in journal
wrapper.MyProperty = "changed value";

// go back to entity state at the specific point in time
// returned value is a wrapper's deep copy 
var past = wrapper.TimeTravel(new DateTime(2015, 01, 01));
// unwrap entity
var oldEntity = past.Unwrapped;
```

### What TimeMachine is not?

- It's not Eventsourcing framework