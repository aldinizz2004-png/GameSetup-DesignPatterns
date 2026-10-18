namespace GameSetup;
public sealed class PlayerProfile
{
    public string Username { get; }
    public string ClassType { get; }
    public int Level { get; }
    public IReadOnlyList<string> Inventory { get; }

    // Private constructor: only the nested Builder can create a PlayerProfile.
    private PlayerProfile(string username, string classType, int level, List<string> inventory)
    {
        Username = username;
        ClassType = classType;
        Level = level;
        Inventory = inventory.AsReadOnly();
    }

    public override string ToString()
    {
        var items = Inventory.Count > 0 ? string.Join(", ", Inventory) : "(empty)";
        return $"Username: {Username}\n" +
               $"Class:    {ClassType}\n" +
               $"Level:    {Level}\n" +
               $"Inventory: {items}";
    }

    public sealed class Builder
    {
        // Default values -> calling Build() directly gives a basic player.
        private string _username = "Guest";
        private string _classType = "Warrior";
        private int _level = 1;
        private readonly List<string> _inventory = new();

        public Builder WithUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.", nameof(username));
            _username = username;
            return this;
        }

        public Builder WithClassType(string classType)
        {
            if (string.IsNullOrWhiteSpace(classType))
                throw new ArgumentException("Class type cannot be empty.", nameof(classType));
            _classType = classType;
            return this;
        }

        public Builder WithLevel(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
            _level = level;
            return this;
        }

        public Builder AddItem(string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
                _inventory.Add(item);
            return this;
        }

        public Builder AddItems(params string[] items)
        {
            foreach (var item in items)
                AddItem(item);
            return this;
        }
        public PlayerProfile Build() =>
            new(_username, _classType, _level, new List<string>(_inventory));
    }
}