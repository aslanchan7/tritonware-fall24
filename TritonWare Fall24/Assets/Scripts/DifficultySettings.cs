using System;

[Serializable]
public struct DifficultySetting
{
    public string Name;
    public float InitialDifficultyScaling;
    public float TimeToMaxDifficulty;
    public float MaxDifficultyScaling;

    public DifficultySetting(string name, float initialDifficultyScaling, float timeToMaxDifficulty, float maxDifficultyScaling)
    {
        Name = name;
        InitialDifficultyScaling = initialDifficultyScaling;
        TimeToMaxDifficulty = timeToMaxDifficulty;
        MaxDifficultyScaling = maxDifficultyScaling;
    }

    public static DifficultySetting[] Difficulties =
{
        new DifficultySetting("Easy", 0.7f, 900, 1.7f),
        new DifficultySetting("Normal", 1.0f, 900, 2.2f),
        new DifficultySetting("Hard", 1.3f, 900, 3.0f),
        new DifficultySetting("Impossible", 1.6f, 1800, 6f),
    };
}
