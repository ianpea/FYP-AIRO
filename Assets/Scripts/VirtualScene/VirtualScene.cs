using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A virtual scene used by virtual scene manager to determine which scene to go to.
/// </summary>
public class VirtualScene : MonoBehaviour
{
    public int sceneIndex;
    public string sceneName;
    public Vector3 initialSpawn;
    public int totalCoins, totalMail, totalGmail;
    public List<GameObject> activeGameObjects;
    public GameObject WorldSpaceUI;

    private void Start()
    {
        totalCoins = gameObject.GetComponentsInChildren<Coin>().Length;
        totalMail = gameObject.GetComponentsInChildren<MailBasic>().Length;
        totalGmail = gameObject.GetComponentsInChildren<MailGoogle>().Length;
        DeactivateScene();
    }

    public void ActivateScene()
    {
        /// Move world space UI to current level position.
        WorldSpaceUI.transform.position = new Vector3(transform.position.x, WorldSpaceUI.transform.position.y, transform.position.z);

        ///// Set current level spawn points.
        //RespawnManager.Instance.spawnPoints = spawnPoints;

        foreach (GameObject g in activeGameObjects)
            g.SetActive(true);

    }

    public void DeactivateScene()
    {
        foreach (GameObject g in activeGameObjects)
            g.SetActive(false);
    }
}
