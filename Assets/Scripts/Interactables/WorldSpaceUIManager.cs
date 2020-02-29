using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUIManager : MonoBehaviour
{
    private static WorldSpaceUIManager _instance;
    public static WorldSpaceUIManager Instance { get { return _instance; } }

    public MenuButton[] menuButtons;
    public GameplayButton[] gameplayButtons;
    public PlayerModel player;
    public Material pressedMaterial;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerView.Instance.Model;
        menuButtons = GameObject.FindObjectsOfType<MenuButton>();
        gameplayButtons = GameObject.FindObjectsOfType<GameplayButton>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.cameraPivot.position, player.cameraPivot.forward, out hit, 1500.0f, 9))
        {
            GameObject m = hit.collider.gameObject;

            if (m.GetComponent<MenuButton>() != null)
            {
                m.GetComponent<MenuButton>().hitByRay = true;
                return;
            }
            if(m.GetComponent<GameplayButton>() != null)
            {
                m.GetComponent<GameplayButton>().hitByRay = true;
                return;
            }
            // If no button is hit, reset all buttons back to original position.
            ResetButtons();
        }   
    }

    void ResetButtons()
    {
        for(int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].Reset();
        }
        for(int i = 0; i < gameplayButtons.Length; i++)
        {
            gameplayButtons[i].Reset();
        }
    }

    public void Pressed(GameObject m)
    {
        AudioManager.Instance.SFXButton();
        Renderer renderer = m.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = pressedMaterial;
        }
    }

    public void HardReset()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].Reset();
            menuButtons[i].HardReset();
        }
        for (int i = 0; i < gameplayButtons.Length; i++)
        {
            gameplayButtons[i].Reset();
            gameplayButtons[i].HardReset();
        }
    }
}
