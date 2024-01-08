using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    public SortingGroup faceSortingGroup;
    public SortingGroup backSortingGroup;
    public float flipDuration = 0.2f;
    public float floatDistance = 0.1f;
    public float floatDuration = 0.5f;

    public SpriteRenderer itemRenderer;

    private Tweener tween;
    private Tweener floatTween;

    public enum FaceState
    {
        FRONT,
        BACK
    }
    public FaceState faceState;

    public FaceState defaultFaceState;

    private void Start()
    {
        defaultFaceState = faceState;
    }

    public void SetFaceImageItem(string spriteName)
    {
        //USE ADDRESSABLE LATER
    }

    public void Reset()
    {
        if (defaultFaceState == FaceState.BACK)
        {
            FlipToBack();
        }
        else
        {
            FlipToFace();
        }
    }

    public void Flip()
    {
        if (faceState == FaceState.FRONT)
        {
            FlipToBack();
        }
        else
        {
            FlipToFace();
        }
    }

    public void FlipToFace()
    {
        faceSortingGroup.sortingOrder = 0;
        backSortingGroup.sortingOrder = 1;
        FlipTo90(() =>
        {
            faceSortingGroup.sortingOrder = 1;
            backSortingGroup.sortingOrder = 0;
            FlipTo180(() => transform.localEulerAngles = new Vector3(0, 0, 0));
            faceState = FaceState.FRONT;
        });
    }

    public void FlipToBack()
    {
        faceSortingGroup.sortingOrder = 1;
        backSortingGroup.sortingOrder = 0;
        FlipTo90(() =>
        {
            faceSortingGroup.sortingOrder = 0;
            backSortingGroup.sortingOrder = 1;
            FlipTo180(() => transform.localEulerAngles = new Vector3(0, 180, 0));
            faceState = FaceState.BACK;
        });
    }

    private void FlipTo90(Action onComplete = null)
    {
        tween = transform.DOLocalRotate(new Vector3(0, 90, 0), flipDuration, RotateMode.LocalAxisAdd);
        tween.SetEase(Ease.Linear);
        tween.SetLoops(1, LoopType.Incremental);
        if (onComplete != null)
        {
            tween.OnComplete(onComplete.Invoke);
        }
        tween.Play();
    }

    private void FlipTo180(Action onComplete = null)
    {
        tween = transform.DOLocalRotate(new Vector3(0, 90, 0), flipDuration, RotateMode.LocalAxisAdd);
        tween.SetEase(Ease.Linear);
        tween.SetLoops(1, LoopType.Incremental);
        if (onComplete != null)
        {
            tween.OnComplete(onComplete.Invoke);
        }
        tween.Play();
    }

    public void Float(Vector3 position)
    {
        transform.position = position;
        Vector3 targetPos = position + new Vector3(0, floatDistance, 0);
        floatTween = transform.DOLocalMoveY(targetPos.y, floatDuration);
        floatTween.SetEase(Ease.InOutQuad);
        floatTween.SetLoops(-1, LoopType.Yoyo);
        floatTween.Play();
    }

    public void StopFloat()
    {
        floatTween.Pause();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flip();
        }
    }
}
