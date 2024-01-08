using System;

[Serializable]
public class BuffData
{
    public enum EffectType
    {
        MOVE_SPEED = 1,
        ATTACK = 2,
    }
    public float duration;
    public float interval;
    public float atrMultiplier;
    public string targetAtrMultiplier;
    public float flatAtr;

    public float durationGrowth;
    public int durationGrowthLevelInterval;

    public float intervalGrowth;

    public float atrMutiplierGrowth;
    public int atrMultiplierGrowthLevelInterval;
    
    public float flatAtrGrowth;
    public int flatAtrGrowthLevelInterval;
}
