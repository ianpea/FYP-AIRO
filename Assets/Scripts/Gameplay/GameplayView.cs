using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayView : MonoBehaviour
{
    private static GameplayView _instance;
    public static GameplayView Instance { get { return _instance; } }

    private static GameplayModel _model = new GameplayModel();
    public GameplayModel Model { get { return _model; } }

    private static GameplayController _controller = new GameplayController();
    public GameplayController Controller { get { return _controller; } }

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

    private void Start()
    {
        Controller.Start();
    }

    private void Update()
    {
        Controller.Update();
    }
}