using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move up and down in X direction (according to prefab).
/// </summary>
public class DirectionHint : MonoBehaviour
{
    float newX, newY, newZ;
    public float speed = 2.0f;
    public float height = 0.15f;

    // Update is called once per frame
    void Update()
    {
        transform.position += (Mathf.Sin(Time.time * speed) * height) * transform.right;
    }
}
