using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatusAilments : MonoBehaviour
{
    private List<StatusAilmentData> statusEffects = new List<StatusAilmentData>();

    public void AddStatusAilment(StatusAilmentData effect)
    {
        statusEffects.Add(effect);
        StartCoroutine(StatusAilmentExpire(effect));
    }

    public void RemoveStatusAilment(StatusAilmentData effect)
    {
        statusEffects.Remove(effect);
    }

    public List<StatusAilmentData> GetStatusAilments()
    {
        return statusEffects;
    }

    private IEnumerator StatusAilmentExpire(StatusAilmentData effect)
    {
        yield return new WaitForSeconds(effect.duration);
        RemoveStatusAilment(effect);
    }
}
