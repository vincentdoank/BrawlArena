using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombTimer : MonoBehaviour
{
    public float explodeTime;
    [Range(0, 1)]
    public float warningTimePct = 0.2f;

    private float elapsedTime;
    private bool scaleUp = true;
    private bool isWarning = false;

    private float tickingTime = 1f;
    private float elapsedTickingTime;
    private float tickSpeed = 1f;
    private float warningTickSpeed = 4f;
    public bool isStarted;

    public Transform target;
    public Vector3 offset;
    public Camera uiCamera;
    public Image targetBlinkImage;

    public static BombTimer Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        Reset();
    }

    public void Reset()
    {
        isWarning = false;
        targetBlinkImage.color = Color.white;
        explodeTime = ((HotPotatoSettings)GameSettings.Instance).GetBombDuration();
        isStarted = false;
        target = null;
        elapsedTime = 0f;
        elapsedTickingTime = 0f;
        tickSpeed = 1f;
        scaleUp = true;
    }

    private void Update()
    {
        if (isStarted && target)
        {
            elapsedTime += Time.deltaTime;
            elapsedTickingTime += Time.deltaTime;

            float remainingTime = explodeTime - elapsedTime;
            tickSpeed = 1 + 4 * elapsedTime / explodeTime;

            float warningTime = explodeTime * warningTimePct;
            if (elapsedTime >= explodeTime - warningTime && elapsedTime < explodeTime)
            {
                isWarning = true;
                //tickSpeed = warningTickSpeed;
            }
            if (elapsedTime >= explodeTime)
            {
                Explode();
            }

            Ticking();
            if (elapsedTickingTime >= tickingTime / tickSpeed)
            {
                scaleUp = !scaleUp;
                elapsedTickingTime = 0f;
            }
        }
    }

    private void LateUpdate()
    {
        if (target)
        {
            RectTransform rectTransform = (RectTransform)transform;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform, screenPos, uiCamera, out Vector3 worldPos);
            rectTransform.position = new Vector3(worldPos.x, worldPos.y, rectTransform.position.z) + offset;
        }
    }

    private void Ticking()
    {
        if (scaleUp)
        {
            ScaleUp();
        }
        else
        {
            ScaleDown();
        }
    }

    private void ScaleUp()
    {
        transform.localScale += Vector3.one * Time.deltaTime * 0.5f * tickSpeed;
        if (isWarning)
        {
            targetBlinkImage.color =  new Color(1, (elapsedTickingTime / (tickingTime / tickSpeed)), (elapsedTickingTime / (tickingTime / tickSpeed)), 1);
        }
    }

    private void ScaleDown()
    {
        transform.localScale -= Vector3.one * Time.deltaTime * 0.5f * tickSpeed;
        if (isWarning)
        {
            targetBlinkImage.color = new Color(1, (elapsedTickingTime / (tickingTime / tickSpeed)), (elapsedTickingTime / (tickingTime / tickSpeed)), 1);
        }
    }

    private void Explode()
    {
        Debug.LogWarning("Exploded");
        Character character = target.GetComponent<Character>();
        ((HotPotatoSettings)GameSettings.Instance).Explode(character);
        Reset();
        transform.localScale = Vector3.zero;
    }

    public void SetTarget(Transform target)
    {
        transform.localScale = Vector3.one;
        scaleUp = true;
        if (target)
        {
            isStarted = true;
            this.target = target;
        }
        else
        {
            isStarted = false;
        }
        Debug.Log("set Target : " + target?.name);
    }

    public void SetTarget(Transform player1, Transform player2)
    {
        if (target == player1)
        {
            target = player2;
            player1.GetComponent<CharacterAttributes>().OnBombReleased();
            player2.GetComponent<CharacterAttributes>().OnBombGrabbed();
        }
        else if(target == player2)
        {
            target = player1;
            player2.GetComponent<CharacterAttributes>().OnBombReleased();
            player1.GetComponent<CharacterAttributes>().OnBombGrabbed();
        }
        Debug.Log("new target : " + target);
    }
}
