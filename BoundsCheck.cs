using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoundType
{
    none,
    right,
    left,
    up,
    down
}

public class BoundsCheck : MonoBehaviour
{
    public bool keepOnScreen = false;
    [SerializeField] private float distance = 1f;

    [HideInInspector] public BoundType type = BoundType.none;
    private float camWidth;
    private float camHeight;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        type = BoundType.none;

        if (pos.x > camWidth + distance)
        {
            pos.x = camWidth + distance;
            type = BoundType.right;
        }
        if (pos.x < -camWidth - distance)
        {
            pos.x = -camWidth - distance;
            type = BoundType.left;
        }
        if (pos.y > camHeight + distance)
        {
            pos.y = camHeight + distance;
            type = BoundType.up;
        }
        if (pos.y < -camHeight - distance)
        {
            pos.y = -camHeight - distance;
            type = BoundType.down;
        }

        if (keepOnScreen && type != BoundType.none)
        {
            transform.position = pos;
            type = BoundType.none;
        }
    }
}
