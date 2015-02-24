using NSubstitute;
using Xunit;
using Xunit.Should;

namespace TimeMachine.Tests
{
    internal class TestEntity
    {
        [Identifier]
        public int Id { get; set; }
        public string Name { get; set; }

        public TestEntity()
        {
        }

        public TestEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class DynamicTests
    {
        private IEventJournal eventJournal;
        private TestEntity entity;
        private dynamic wrapper;

        public DynamicTests()
        {
            eventJournal = Substitute.For<IEventJournal>();
            var timeMachine = new TimeMachine<TestEntity>(eventJournal);

            entity = new TestEntity(11, "John Doe");
            wrapper = timeMachine.Wrap(entity);
        }

        [Fact]
        public void TimeMachineWrapper_should_generate_an_event_after_wrapped_field_has_been_changed()
        {
            const string name = "John Smith";
            wrapper.Name = name;

            eventJournal.Received(1).WriteEvent(Arg.Is<UpdateEvent>(e => e.PropertyName == "Name" && e.Value.ToString() == name && (int)e.Identifier == entity.Id));
        }

        [Fact]
        public void TimeMachineWrapper_should_change_related_property_value_of_underlying_entity()
        {
            const string name = "Dan Allen";
            wrapper.Name = name;

            entity.Name.ShouldBe(name);
        }
    }
}