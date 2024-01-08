using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUseItem : MonoBehaviour
{
    private CharacterAttributes characterAtr;

    private void Start()
    {
        characterAtr = GetComponent<CharacterAttributes>();
    }

    public void UseItem(Booster booster)
    {
        List<Buff> buffs = booster.buffs;
        List<StatusAilment> statusList = booster.statusEffects;
        characterAtr.OnAttributeChanged(buffs);
    }
}
