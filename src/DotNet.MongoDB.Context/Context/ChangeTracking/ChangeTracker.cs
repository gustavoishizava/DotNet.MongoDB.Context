namespace DotNet.MongoDB.Context.Context.ChangeTracking
{
    public class ChangeTracker
    {
        private List<Entry> _entries { get; set; }

        public IReadOnlyList<Entry> Entries => _entries.AsReadOnly();

        public ChangeTracker()
        {
            _entries = new();
        }

        public void AddEntry(Entry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry), "Entry cannot be null.");

            _entries.Add(entry);
        }

        public void Clear()
        {
            _entries.Clear();
        }
    }
}