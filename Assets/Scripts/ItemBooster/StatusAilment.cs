using System;

[Serializable]
public class StatusAilmentData
{
    public enum StatusType
    {
        STUN,
        FREEZE,
        STONE,
        CONFUSE,
        POISON,
        BURN,
        INVINCIBLE,
        INVISIBLE
    }

    public StatusType statusType;
    public float duration;
    public float interval;

    public float durationGrowth;
    public float intervalGrowth;
}
