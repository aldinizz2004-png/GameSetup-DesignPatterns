using System.Text;
using GameSetup;

Console.OutputEncoding = Encoding.UTF8;

const int Width = 60;
int passed = 0, total = 0;

PrintBanner();

// =====================================================================
// 1. SINGLETON PATTERN
// =====================================================================
Section(1, "SINGLETON PATTERN  ·  GameSettings");

Code("var settingsA = GameSettings.Instance;");
var settingsA = GameSettings.Instance;          // first access -> instance is created here (lazy)
Code("var settingsB = GameSettings.Instance;");
var settingsB = GameSettings.Instance;          // second access -> same instance returned
Console.WriteLine();

Card("DEFAULT SETTINGS", new[]
{
    ("Difficulty",   settingsA.Difficulty),
    ("Sound Volume", VolumeBar(settingsA.SoundVolume))
});

Code("settingsA.Difficulty  = \"Hard\";");
Code("settingsA.SoundVolume = 80;");
settingsA.Difficulty = "Hard";
settingsA.SoundVolume = 80;
Console.WriteLine();

Card("READ THROUGH settingsB (never modified directly)", new[]
{
    ("Difficulty",   settingsB.Difficulty),
    ("Sound Volume", VolumeBar(settingsB.SoundVolume))
});

Card("MEMORY IDENTITY", new[]
{
    ("settingsA hash", settingsA.GetHashCode().ToString()),
    ("settingsB hash", settingsB.GetHashCode().ToString())
});

// Thread-safety: many threads request the instance at the same moment.
const int ThreadCount = 100;
var fromThreads = new GameSettings[ThreadCount];
Parallel.For(0, ThreadCount, i => fromThreads[i] = GameSettings.Instance);
int distinctInstances = fromThreads.Distinct().Count();

Check("ReferenceEquals(settingsA, settingsB) == true", ReferenceEquals(settingsA, settingsB));
Check("Change made via A is visible via B", settingsB.Difficulty == "Hard" && settingsB.SoundVolume == 80);
Check($"{ThreadCount} parallel threads -> {distinctInstances} instance", distinctInstances == 1);
Check("No public constructor (new GameSettings() blocked)", typeof(GameSettings).GetConstructors().Length == 0);

// =====================================================================
// 2. BUILDER PATTERN - DEFAULT BASIC PLAYER
// =====================================================================
Section(2, "BUILDER PATTERN  ·  Default Basic Player");

Code("var defaultPlayer = new PlayerProfile.Builder().Build();");
var defaultPlayer = new PlayerProfile.Builder().Build();
Console.WriteLine();

PlayerCard("DEFAULT BASIC PLAYER", defaultPlayer);

// =====================================================================
// 3. BUILDER PATTERN - FULLY CUSTOM PLAYER
// =====================================================================
Section(3, "BUILDER PATTERN  ·  Fully Custom Player");

Code("var customPlayer = new PlayerProfile.Builder()");
Code("    .WithUsername(\"Izzaldeen\")");
Code("    .WithClassType(\"Mage\")");
Code("    .WithLevel(25)");
Code("    .AddItem(\"Staff of Fire\")");
Code("    .AddItems(\"Health Potion\", \"Mana Potion\", \"Magic Ring\")");
Code("    .Build();");

var customPlayer = new PlayerProfile.Builder()
    .WithUsername("Izzaldeen")
    .WithClassType("Mage")
    .WithLevel(25)
    .AddItem("Staff of Fire")
    .AddItems("Health Potion", "Mana Potion", "Magic Ring")
    .Build();
Console.WriteLine();

PlayerCard("FULLY CUSTOM PLAYER", customPlayer);

Check("No public constructor (must use the Builder)", typeof(PlayerProfile).GetConstructors().Length == 0);
Check("Inventory is read-only from outside",
      customPlayer.Inventory is ICollection<string> collection && collection.IsReadOnly);
Check("Default player unaffected by custom build",
      defaultPlayer.Username == "Guest" && defaultPlayer.Inventory.Count == 0);

PrintSummary();

// =====================================================================
// 4. INTERACTIVE MODE - change the shared settings and build players live
// =====================================================================
RunInteractiveMenu(new List<PlayerProfile> { defaultPlayer, customPlayer });

// =====================================================================
// Console formatting helpers (presentation only)
// =====================================================================

void PrintBanner()
{
    int inner = Width - 2;
    WriteColored("╔" + new string('═', inner) + "╗", ConsoleColor.Cyan);
    WriteColored("║" + Center("", inner) + "║", ConsoleColor.Cyan);
    WriteColored("║" + Center("GAME SETTINGS  &  PLAYER SETUP", inner) + "║", ConsoleColor.Cyan);
    WriteColored("║" + Center("Singleton + Builder Design Patterns  ·  C#", inner) + "║", ConsoleColor.Cyan);
    WriteColored("║" + Center("", inner) + "║", ConsoleColor.Cyan);
    WriteColored("╚" + new string('═', inner) + "╝", ConsoleColor.Cyan);
}

void PrintSummary()
{
    bool allPassed = passed == total;
    var color = allPassed ? ConsoleColor.Green : ConsoleColor.Red;
    string message = allPassed
        ? $"ALL CHECKS PASSED  ({passed}/{total})"
        : $"SOME CHECKS FAILED  ({passed}/{total})";

    int inner = Width - 2;
    Console.WriteLine();
    WriteColored("╔" + new string('═', inner) + "╗", color);
    WriteColored("║" + Center(message, inner) + "║", color);
    WriteColored("╚" + new string('═', inner) + "╝", color);
    Console.WriteLine();
}

void Section(int number, string title)
{
    Console.WriteLine();
    WriteColored($" [{number}] {title}", ConsoleColor.Cyan);
    WriteColored(" " + new string('─', Width - 1), ConsoleColor.DarkGray);
}

void Code(string line) => WriteColored("   > " + line, ConsoleColor.DarkGray);

void Check(string label, bool ok)
{
    total++;
    if (ok) passed++;

    Console.Write("   ");
    Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write(ok ? "[PASS]" : "[FAIL]");
    Console.ResetColor();
    Console.WriteLine(" " + label);
}

void PlayerCard(string title, PlayerProfile player)
{
    var rows = new List<(string Label, string Value)>
    {
        ("Username",  player.Username),
        ("Class",     player.ClassType),
        ("Level",     player.Level.ToString()),
        ("Inventory", player.Inventory.Count == 0 ? "(empty)" : $"{player.Inventory.Count} item(s)")
    };
    rows.AddRange(player.Inventory.Select(item => ("", "• " + item)));
    Card(title, rows);
}

void Card(string title, IEnumerable<(string Label, string Value)> rows)
{
    int inner = Width - 6;
    WriteColored("   ┌" + new string('─', inner + 2) + "┐", ConsoleColor.DarkCyan);
    BoxLine(title, inner, ConsoleColor.Yellow);
    WriteColored("   ├" + new string('─', inner + 2) + "┤", ConsoleColor.DarkCyan);
    foreach (var (label, value) in rows)
    {
        string text = label.Length == 0 ? "                  " + value : $"{label,-16}: {value}";
        BoxLine(text, inner, ConsoleColor.White);
    }
    WriteColored("   └" + new string('─', inner + 2) + "┘", ConsoleColor.DarkCyan);
    Console.WriteLine();
}

void BoxLine(string text, int inner, ConsoleColor color)
{
    if (text.Length > inner) text = text[..inner];
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.Write("   │ ");
    Console.ForegroundColor = color;
    Console.Write(text.PadRight(inner));
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine(" │");
    Console.ResetColor();
}

string VolumeBar(int volume)
{
    int filled = volume / 10;
    return new string('█', filled) + new string('░', 10 - filled) + $"  {volume}%";
}

string Center(string text, int width) =>
    text.PadLeft((width + text.Length) / 2).PadRight(width);

void WriteColored(string text, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ResetColor();
}

// =====================================================================
// Interactive mode
// =====================================================================

void RunInteractiveMenu(List<PlayerProfile> players)
{
    Section(4, "INTERACTIVE MODE  ·  Live Settings & Players");

    while (true)
    {
        // A fresh reference every loop - it is still the same single instance.
        var current = GameSettings.Instance;
        Card("CURRENT SETTINGS", new[]
        {
            ("Difficulty",   current.Difficulty),
            ("Sound Volume", VolumeBar(current.SoundVolume)),
            ("Players",      players.Count.ToString())
        });

        WriteColored("   [1] Change difficulty", ConsoleColor.White);
        WriteColored("   [2] Change sound volume", ConsoleColor.White);
        WriteColored("   [3] Prove Singleton (get a new reference)", ConsoleColor.White);
        WriteColored("   [4] Create a new player (Builder)", ConsoleColor.White);
        WriteColored("   [5] List all players", ConsoleColor.White);
        WriteColored("   [0] Exit", ConsoleColor.White);
        Console.WriteLine();

        string? choice = Prompt("Choose an option");
        Console.WriteLine();

        switch (choice)
        {
            case null:
            case "0":
                WriteColored("   Goodbye! Settings saved in the single GameSettings instance.", ConsoleColor.Green);
                Console.WriteLine();
                return;
            case "1": ChangeDifficulty(); break;
            case "2": ChangeVolume(); break;
            case "3": ProveSingletonAgain(); break;
            case "4": CreatePlayer(players); break;
            case "5": ListPlayers(players); break;
            default: Error("Invalid option, choose a number from the menu."); break;
        }
        Console.WriteLine();
    }
}

void ChangeDifficulty()
{
    string[] levels = { "Easy", "Normal", "Hard", "Nightmare" };
    for (int i = 0; i < levels.Length; i++)
        WriteColored($"   [{i + 1}] {levels[i]}", ConsoleColor.White);

    string? input = Prompt("Select difficulty");
    if (int.TryParse(input, out int index) && index >= 1 && index <= levels.Length)
    {
        GameSettings.Instance.Difficulty = levels[index - 1];
        Success($"Difficulty set to {levels[index - 1]}.");
    }
    else
    {
        Error("Invalid choice, difficulty not changed.");
    }
}

void ChangeVolume()
{
    string? input = Prompt("Enter sound volume (0-100)");
    if (!int.TryParse(input, out int volume))
    {
        Error("Please enter a whole number.");
        return;
    }

    try
    {
        // The validation lives inside GameSettings, not here.
        GameSettings.Instance.SoundVolume = volume;
        Success($"Sound volume set to {volume}%.");
    }
    catch (ArgumentOutOfRangeException)
    {
        Error("Volume must be between 0 and 100.");
    }
}

void ProveSingletonAgain()
{
    Code("var newReference = GameSettings.Instance;");
    var newReference = GameSettings.Instance;

    Card("NEW REFERENCE vs ORIGINAL settingsA", new[]
    {
        ("newReference",  $"{newReference.Difficulty}, {newReference.SoundVolume}%  (hash {newReference.GetHashCode()})"),
        ("settingsA",     $"{settingsA.Difficulty}, {settingsA.SoundVolume}%  (hash {settingsA.GetHashCode()})")
    });

    if (ReferenceEquals(newReference, settingsA))
        Success("Same object in memory - every reference sees your latest changes.");
    else
        Error("Different instances - Singleton is broken!");
}

void CreatePlayer(List<PlayerProfile> players)
{
    WriteColored("   Press Enter to keep the default value.", ConsoleColor.DarkGray);
    var builder = new PlayerProfile.Builder();

    try
    {
        string? username = Prompt("Username   (default: Guest)");
        if (!string.IsNullOrWhiteSpace(username))
            builder.WithUsername(username);

        string? classType = Prompt("Class      (default: Warrior)");
        if (!string.IsNullOrWhiteSpace(classType))
            builder.WithClassType(classType);

        string? levelText = Prompt("Level      (default: 1)");
        if (!string.IsNullOrWhiteSpace(levelText))
        {
            if (!int.TryParse(levelText, out int level))
            {
                Error("Level must be a whole number. Player not created.");
                return;
            }
            builder.WithLevel(level);
        }

        string? itemsText = Prompt("Items      (comma separated, optional)");
        if (!string.IsNullOrWhiteSpace(itemsText))
            builder.AddItems(itemsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        var player = builder.Build();
        players.Add(player);

        Console.WriteLine();
        PlayerCard("NEW PLAYER CREATED", player);
        Success($"{player.Username} was built step by step with the Builder.");
    }
    catch (ArgumentException ex)
    {
        // Validation errors thrown by the Builder (empty name, level < 1, ...).
        Error(ex.Message + " Player not created.");
    }
}

void ListPlayers(List<PlayerProfile> players)
{
    for (int i = 0; i < players.Count; i++)
        PlayerCard($"PLAYER #{i + 1}", players[i]);
}

string? Prompt(string label)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write($"   {label}: ");
    Console.ResetColor();
    return Console.ReadLine()?.Trim();
}

void Success(string message) => WriteColored("   [OK] " + message, ConsoleColor.Green);

void Error(string message) => WriteColored("   [ERROR] " + message, ConsoleColor.Red);