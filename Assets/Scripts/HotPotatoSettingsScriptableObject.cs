using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HotPotatoSettings", menuName = "ScriptableObjects/HotPotatoSettings", order = 1)]
public class HotPotatoSettingsScriptableObject : ScriptableObject
{
    public int lives = 3;
    public float maxBombDuration = 50f;
    public float minBombDuration = 15f;
}
