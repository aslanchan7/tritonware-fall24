public struct DifficultySetting
{
    public int Level;
    public float InitialDifficultyScaling;
    public float TimeToMaxDifficulty;
    public float MaxDifficultyScaling;

    public DifficultySetting(int level, float initialDifficultyScaling, float timeToMaxDifficulty, float maxDifficultyScaling)
    {
        Level = level;
        InitialDifficultyScaling = initialDifficultyScaling;
        TimeToMaxDifficulty = timeToMaxDifficulty;
        MaxDifficultyScaling = maxDifficultyScaling;
    }

    public static DifficultySetting[] Difficulties =
{
        new DifficultySetting(0, 0.7f, 900, 1.5f),
        new DifficultySetting(1, 1.0f, 900, 2.0f),
        new DifficultySetting(2, 1.3f, 900, 2.5f),
        new DifficultySetting(3, 1.5f, 1800, 5f),
    };
}
