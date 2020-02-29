using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    /// <summary>
    /// Singleton.
    /// </summary>
    private static RespawnManager _instance;
    public static RespawnManager Instance { get { return _instance; } }

    /// <summary>
    /// Respawn delegate.
    /// </summary>
    public delegate void OnRespawn();
    public OnRespawn onRespawn;

    // Properties
    public SpawnPoint[] spawnPoints;
    public GameObject player;
    public Vector3 pos;
    public const float RESPAWN_TIME = 3.0f;

    // Coroutine
    private Coroutine respawnCo;
    public bool isRespawning = false;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            if(_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        Instance.onRespawn += Instance.Respawn;
    }

    public void Respawn()
    {
        List<float> dist = new List<float>(spawnPoints.Length);
        float min = float.MaxValue;

        foreach(SpawnPoint sp in spawnPoints)
        {
            if(Vector3.Distance(sp.position, player.transform.position) < min)
            {
                min = Vector3.Distance(sp.position, player.transform.position);
                pos = sp.position;
            }
        }

        respawnCo = StartCoroutine(RespawnCo(pos));
    }

    public IEnumerator RespawnCo(Vector3 targetPos)
    {
        Vector3 startPos = player.transform.position;
        player.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().isKinematic = true;

        isRespawning = true;
        for(float t = 0; t < RESPAWN_TIME; t += Time.fixedDeltaTime)
        {
            player.transform.position = Vector3.Lerp(startPos, targetPos, t/RESPAWN_TIME);
            yield return null;
        }
        isRespawning = false;

        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Rigidbody>().isKinematic = false;

        yield return null;
    }

    public void StopRespawn()
    {
        StopCoroutine(respawnCo);
    }
}
