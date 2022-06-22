namespace MeuBolsoDigital.MongoDB.Context.Context.ChangeTracking
{
    public class Entry
    {
        public EntryState State { get; private set; }
        public object Value { get; private set; }

        public Entry(EntryState state, object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");

            State = state;
            Value = value;
        }
    }
}