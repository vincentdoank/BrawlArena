using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollideDetection : MonoBehaviour
{
    public LayerMask layerMask;
    public UnityEvent<Transform, Transform> onCollide;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collide : " + CompareLayer(other.gameObject.layer));
        if (CompareLayer(other.gameObject.layer))
        {
            GameSettings.Instance.Collide(transform, other.transform);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        GameSettings.Instance.CheckRepeatCollide(transform, other.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameSettings.Instance.RemoveCollide(transform, other.transform);
    }

    private bool CompareLayer(int layer)
    {
        if ((layerMask & 1 << layer) == 1 << layer)
        {
            return true;            
        }
        return false;
    }
}
