using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Buff
{
    public int level;
    public BuffData data;
}

[Serializable]
public class StatusAilment
{
    public int level;
    public StatusAilmentData data;
}

[Serializable]
public class Booster
{
    public string boosterId;
    public List<Buff> buffs;
    public List<StatusAilment> statusEffects;
}

public class ItemBooster : MonoBehaviour
{
    public string boosterId;

    private Booster booster;
    private Card card;
    private Tweener ascendTween;
    private Tweener scaleTween;

    private bool isUsed = false;
    private ItemSpawner spawner;

    private void Start()
    {
        card = GetComponent<Card>();
        isUsed = false;
    }

    public void Reset()
    {
        card.Reset();
    }

    public void Spawn(ItemSpawner spawner, Vector3 position)
    {
        this.spawner = spawner;
        transform.position = position;
        card.Float(transform.position);
        Reset();
    }

    public Booster UseItem()
    {
        card.StopFloat();
        card.Flip();
        AscendEffect();
        if (spawner)
        {
            spawner.Release();
        }
        if (boosterId == "RANDOM")
        {
            return ItemBoosterManager.Instance.GetRandomBooster();
        }
        else
        {
            return ItemBoosterManager.Instance.GetBooster(boosterId);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isUsed)
        {
            isUsed = true;
            Debug.Log("Get Item : " + gameObject.name, this);
            booster = UseItem();
            CharacterUseItem characterUseItem = other.GetComponent<CharacterUseItem>();
            characterUseItem.UseItem(booster);
        }
    }

    private void AscendEffect()
    {
        Vector3 pos = transform.localPosition;
        pos.y += 2f;
        ascendTween = transform.DOLocalMoveY(pos.y, 1f);
        ascendTween.SetEase(Ease.OutBack);
        ascendTween.Play();

        scaleTween = transform.DOScale(Vector3.one * 3f, 1f);
        scaleTween.OnComplete(ScaleDownEffect);
        scaleTween.Play();
    }

    private void ScaleDownEffect()
    {
        scaleTween = transform.DOScale(Vector3.zero, 0.2f);
        scaleTween.SetDelay(1f);
        scaleTween.SetEase(Ease.InBack);
        scaleTween.Play();
    }
}
