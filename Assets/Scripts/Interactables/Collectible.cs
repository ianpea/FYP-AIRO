using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    /// <summary>
    /// Angle spinned over time.
    /// </summary>
    private float spinRate;

    /// <summary>
    /// Name of this collectible. Set in editor.
    /// </summary>
    public string name;

    /// <summary>
    /// Which scene/level this collectible belongs to, set in virtual scene.
    /// </summary>
    public int sceneIndex;

    /// <summary>
    /// Angle spinned over 1 second.
    /// </summary>
    public float spinAnglePerSec;

    /// <summary>
    /// Axis of spin.
    /// </summary>
    public Vector3 spinAxis;

    /// <summary>
    /// Score given when collected.
    /// </summary>
    public int score;

    // Start is called before the first frame update
    public virtual void Start()
    {
        spinRate = spinAnglePerSec * Time.deltaTime;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Spin();
    }

    public virtual void Spin()
    {
        transform.Rotate(spinAxis, spinRate);
    }
}
