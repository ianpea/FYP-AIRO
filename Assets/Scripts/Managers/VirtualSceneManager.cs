using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualSceneManager : MonoBehaviour
{
    private static VirtualSceneManager _instance;
    public static VirtualSceneManager Instance { get { return _instance; } }

    [SerializeField] public int currentSceneIndex;
    [SerializeField] public List<VirtualScene> sceneList;
    [SerializeField] public int defaultSceneIndex;
    [SerializeField] public PlayerModel player;

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
        currentSceneIndex = defaultSceneIndex;
        player = PlayerView.Instance.Model;
        currentSceneIndex = defaultSceneIndex;
        for (int i = 0; i < sceneList.Count; i++)
        {
            sceneList[i].gameObject.SetActive(false);
        }
        // Always go to menu scene/level at start.
        SetScene(currentSceneIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            NextScene();
        }
    }

    /// <summary>
    /// Play next scene.
    /// </summary>
    public void NextScene()
    {
        sceneList[currentSceneIndex].DeactivateScene();
        RecycleScene(currentSceneIndex);
        if (currentSceneIndex == sceneList.Count - 1)
        {
            Debug.Log("Last level completed. going back to tutorial level.");
            currentSceneIndex = 0;
        }
        else
        {
            currentSceneIndex++;
            Debug.Log("wrong level");
        }
        SetScene(currentSceneIndex);
    }

    /// <summary>
    /// To choose level.
    /// </summary>
    public void SetScene(int sceneIndex)
    {
        RecycleScene(sceneIndex);
        sceneList[sceneIndex].ActivateScene();
        InitScene(sceneIndex);
        AudioManager.Instance.StopAllAudios();
        switch (currentSceneIndex)
        {
            case 0: // Menu == 0
                AudioManager.Instance.SFXBGMMenu();
                GameplayUIManager.Instance.Clock.gameObject.SetActive(false);
                break;
            case 1: // Tutorial
                AudioManager.Instance.SFXBGMTutorial();
                GameplayUIManager.Instance.Clock.gameObject.SetActive(true);
                AudioManager.Instance.SFXGameStart();
                break;
            case 2: // Level 1
                AudioManager.Instance.SFXBGMLevel1();
                GameplayUIManager.Instance.Clock.gameObject.SetActive(true);
                AudioManager.Instance.SFXGameStart();
                break;
            case 3: // Level 2
                AudioManager.Instance.SFXBGMLevel2();
                GameplayUIManager.Instance.Clock.gameObject.SetActive(true);
                AudioManager.Instance.SFXGameStart();
                break;

        }
        GameplayUIManager.Instance.ResetClock();
    }

    /// <summary>
    /// Get current scene.
    /// </summary>
    /// <returns></returns>
    public VirtualScene GetCurrentScene()
    {
        return sceneList[currentSceneIndex];
    }

    public void ReturnToMenu()
    {
        sceneList[currentSceneIndex].DeactivateScene();
        RecycleScene(currentSceneIndex);
        currentSceneIndex = 0;
        SetScene(0);
    }

    /// <summary>
    /// Initialize scene, e.g. player position, etc.
    /// </summary>
    /// <param name="sceneIndex"></param>
    private void InitScene(int sceneIndex)
    {
        if (!sceneList[sceneIndex].gameObject.activeInHierarchy)
        {
            sceneList[sceneIndex].gameObject.SetActive(true);
        }
        PlayerView.Instance.Controller.Reset(sceneList[sceneIndex]);
    }

    /// <summary>
    /// Set the scene inactive.
    /// </summary>
    /// <param name="sceneIndex">Recycled scene index.</param>
    private void RecycleScene(int sceneIndex)
    {
        sceneList[sceneIndex].gameObject.SetActive(false);
    }
}

