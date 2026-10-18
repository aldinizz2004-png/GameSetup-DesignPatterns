namespace GameSetup;

public sealed class GameSettings
{
    private static readonly Lazy<GameSettings> _instance = new Lazy<GameSettings>(() => new GameSettings());

    public static GameSettings Instance => _instance.Value;
    private int _soundVolume=50;

    private GameSettings()
    {
        Console.WriteLine("[GameSettings] Instance created (this line should appear only once).");
    }

    public string Difficulty { get; set; } = "Normal";

    public int SoundVolume
    {
        get => _soundVolume;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Volume must be between 0 and 100.");
            _soundVolume = value;
        }
    }
    public override string ToString()
    {
        return $"Difficulty: {Difficulty}, Sound Volume: {SoundVolume}";
    }

}
