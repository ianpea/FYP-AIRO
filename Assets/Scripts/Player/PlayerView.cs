using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private static PlayerView _instance;
    public static PlayerView Instance { get { return _instance; } }

    [SerializeField] private PlayerModel _model = new PlayerModel();
    public PlayerModel Model { get { return _model; } }

    private PlayerController _controller = new PlayerController();
    public PlayerController Controller { get { return _controller; } }


    private void Awake()
    {
        if(_instance == null)
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
        Instance.Controller.Awake();
    }

    private void Start()
    {
        Controller.Start();
    }

    private void Update()
    {
        Controller.Update();
    }
    private void FixedUpdate()
    {
        Controller.FixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Controller.OnCollisionEnter(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        Controller.OnCollisionStay(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Controller.OnCollisionExit(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        Controller.OnTriggerEnter(other);
    }
}
