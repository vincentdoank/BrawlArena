using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributes : MonoBehaviour
{
    private CustomCharacterHorizontalMovement horizontalMovement;
    private const string SPEED = "SPEED";
    private float bonusSpeed = 0;
    private const float NORMAL_SPEED = 5f;
    private const float BOMB_BONUS_SPEED = 1f;

    private void Start()
    {
        horizontalMovement = GetComponent<CustomCharacterHorizontalMovement>();
    }

    public void OnBombGrabbed()
    {
        bonusSpeed += BOMB_BONUS_SPEED;
    }

    public void OnBombReleased()
    {
        bonusSpeed -= BOMB_BONUS_SPEED;
    }

    private void Update()
    {
        horizontalMovement.MovementSpeed = NORMAL_SPEED + bonusSpeed;
    }

    public void OnAttributeChanged(List<Buff> buffs)
    {
        foreach (Buff buff in buffs)
        {
            float baseAtr = 0f;
            switch (buff.data.targetAtrMultiplier)
            {
                case SPEED:
                    baseAtr = horizontalMovement.MovementSpeed;
                    break;
            }

            switch (buff.data.targetAtrMultiplier)
            {
                case SPEED:
                    bonusSpeed = GetAtrMultiplier(buff) * baseAtr + GetFlatAtr(buff);
                    break;
            }
            StartCoroutine(BuffExpire(buff));
        }
    }

    private float GetAtrMultiplier(Buff buff)
    {
        return buff.data.atrMultiplierGrowthLevelInterval == 0 ? 0 : buff.data.atrMultiplier + buff.data.atrMutiplierGrowth * Mathf.FloorToInt(buff.level / buff.data.atrMultiplierGrowthLevelInterval);
    }

    private float GetFlatAtr(Buff buff)
    {
        return buff.data.flatAtrGrowthLevelInterval == 0 ? 0 : buff.data.flatAtr + buff.data.flatAtrGrowth * Mathf.FloorToInt(buff.level / buff.data.flatAtrGrowthLevelInterval);
    }

    public void OnAttributeChangeEnded(List<Buff> buffs)
    {
        foreach (Buff buff in buffs)
        {
            switch (buff.data.targetAtrMultiplier)
            {
                case SPEED:
                    bonusSpeed = 0;
                    break;
            }
        }
    }

    private IEnumerator BuffExpire(Buff buff)
    {
        Debug.Log("cur speed : " + horizontalMovement.MovementSpeed);
        yield return new WaitForSeconds(buff.data.duration);
        OnAttributeChangeEnded(new List<Buff>() { buff });
        Debug.Log("cur speed : " + horizontalMovement.MovementSpeed);
    }
}
