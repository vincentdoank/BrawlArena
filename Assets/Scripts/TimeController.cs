using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeController : MonoBehaviour
{
    public float decideTime = 2f;
    public float nextRoundDelay = 3f;

    public static TimeController Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void Decide(Action onComplete)
    {
        StartCoroutine(Wait(decideTime, onComplete));
        onComplete?.Invoke();
    }

    public void NextRound(Action onComplete)
    {
        StartCoroutine(Wait(nextRoundDelay, onComplete));
    }

    public IEnumerator Wait(float time, Action onComplete)
    {
        yield return new WaitForSeconds(time);
        onComplete?.Invoke();
    }
}
