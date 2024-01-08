using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WTI.SpinningWheel;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class SpinningWheel : MonoBehaviour
{
    public Camera uiCamera;
    public GameObject sliceItemPrefab;
    public Transform sliceItemParent;
    public GameObject linePrefab;
    public Transform lineParent;

    public List<Color> colorList;
    public AnimationCurve throwCurve;
    public float curveMultiplier = 1f;
    public float speed = 1f;

    public RectTransform throwableBomb;
    public RectTransform root;

    private Action onThrowCompleted;

    private Transform wheel;
    private Transform target;
    private float curveElapsedTime = 0;
    private const int CIRCLE_ANGLE = 360;
    private const float BORDER_SIZE = 0.015f;

    private List<GameObject> sliceItemPool = new List<GameObject>();
    private List<GameObject> linePool = new List<GameObject>();
    private TweenScale wheelTweenScale;

    public static SpinningWheel Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        wheelTweenScale = GetComponent<TweenScale>();
        wheel = transform.GetChild(0);
        curveElapsedTime = 0f;
    }

    public void Spawn(List<PlayerData> playerDataList)
    {
        foreach (GameObject go in sliceItemPool)
        {
            go.SetActive(false);
        }
        //foreach (GameObject go in linePool)
        //{
        //    go.SetActive(false);
        //}
        transform.localScale = Vector3.one;
        throwableBomb.gameObject.SetActive(false);
        throwableBomb.sizeDelta = new Vector2(500, 500);
        int playerCount = playerDataList.Where(x => x.score > 0).Count();
        //float initialAngle = 90;
        //float partialAngle = CIRCLE_ANGLE / playerCount;
        //float perpendAngle = partialAngle - initialAngle;

        int index = 0;
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].score <= 0)
            {
                continue;
            }
            string id = playerDataList[i].playerId;
            GameObject go = GetSliceItem();
            go.transform.position = Vector3.zero;

            PartItem item = go.GetComponent<PartItem>();
            item.Init(id, colorList[i], 1f / playerCount - BORDER_SIZE, GameManager.Instance.GetAvatarSprite(id));
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = new Vector3(0, 0, CIRCLE_ANGLE / playerCount * index);

            //GameObject line = GetLine();
            //line.transform.localPosition = Vector3.zero;
            //line.transform.localEulerAngles = new Vector3(0, 0, CIRCLE_ANGLE / playerCount * index);
            //lineParent.transform.localEulerAngles = new Vector3(0, 0, perpendAngle);
            //Debug.LogWarning("index : " + index + " " + playerDataList[i].playerId);
            index += 1;
        }
        wheelTweenScale.Play();
    }

    private GameObject GetSliceItem()
    {
        for (int i = 0; i < sliceItemPool.Count; i++)
        {
            if (!sliceItemPool[i].activeInHierarchy)
            {
                sliceItemPool[i].SetActive(true);
                return sliceItemPool[i];
            }
        }

        GameObject go = Instantiate(sliceItemPrefab, sliceItemParent, false);
        sliceItemPool.Add(go);
        return go;
    }

    private GameObject GetLine()
    {
        for (int i = 0; i < linePool.Count; i++)
        {
            if (!linePool[i].activeInHierarchy)
            {
                linePool[i].SetActive(true);
                return linePool[i];
            }
        }

        GameObject go = Instantiate(linePrefab, lineParent, false);
        linePool.Add(go);
        return go;
    }

    public void Spin(List<PlayerData> playerDataList, int index, Action onComplete)
    {
        wheel.localEulerAngles = new Vector3(0, 0, 180);
        int playerCount = playerDataList.Where(x => x.score > 0).Count();
        int loop = Random.Range(5, 8);
        float size = CIRCLE_ANGLE / playerCount;
        wheel.DORotate(new Vector3(0, 0, CIRCLE_ANGLE * loop - index * size + 45), 3f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => onComplete?.Invoke())
            .SetLoops(1, LoopType.Incremental)
            .Play();
    }

    public void Throw(Transform target, Action onCompleted)
    {
        throwableBomb.anchoredPosition = Vector2.zero;
        throwableBomb.gameObject.SetActive(true);
        this.target = target;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(root, screenPos, uiCamera, out Vector3 worldPos);
        throwableBomb.DOMove(worldPos + BombTimer.Instance.offset, 1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                onCompleted?.Invoke();
                throwableBomb.gameObject.SetActive(false);
            })
            .Play();
        throwableBomb.DOSizeDelta(new Vector2(40, 40), 0.5f).Play();
        //onThrowCompleted = onCompleted;
        transform.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
    //    if (target)
    //    {
    //        if (curveElapsedTime < 1f)
    //        {
    //            transform.localScale = Vector3.zero;
    //            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
    //            RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform, screenPos, uiCamera, out Vector3 worldPos);
    //            //throwableBomb.position = new Vector3(throwableBomb.position.x + worldPos.x * curveElapsedTime, throwableBomb.position.y + worldPos.y * throwCurve.Evaluate(curveElapsedTime) * curveMultiplier, worldPos.z);

    //            curveElapsedTime += Time.deltaTime * speed;
    //        }
    //        else
    //        {
    //            onThrowCompleted?.Invoke();
    //        }
    //    }
    }
}
