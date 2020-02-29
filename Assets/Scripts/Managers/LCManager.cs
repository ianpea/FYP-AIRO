using UnityEngine;
using System.Threading;

public class LCManager : MonoBehaviour
{
    /*
     * Instance for Left Locomotion Controller (LC) manager. Manages the start, update of the Left LC.
     */
    private static LCManager _instance;
    public static LCManager Instance { get { return _instance; } }

    /*
     * Instance for Left Locomotion Controller (LC) model/database. Data/fields are written in here.
     */
    private static LCModel _modelL = new LCModel();
    public static LCModel LeftModel { get { return _modelL; } }

    private static LCModel _modelR = new LCModel();
    public static LCModel RightModel { get { return _modelR; } }

    /*
     * Instance for Left & right Locomotion Controller (LC) controller. Functions/method are written in here.
     */
    private static LCControllerL _controllerL = new LCControllerL();
    public static LCControllerL LeftController { get { return _controllerL; } }

    private static LCControllerR _controllerR = new LCControllerR();
    public static LCControllerR RightController { get { return _controllerR; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        LeftController.Start();
        RightController.Start();
    }

    private void Update()
    {
        LeftController.Update();
        RightController.Update();
    }

    private void FixedUpdate()
    {
        //LeftController.FixedUpdate();
        //RightController.FixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    public void RegisterStep()
    {
        LeftController.RegisterLeftStep();
        RightController.RegisterRightStep();
    }

    public void RegisterJump()
    {
        LeftController.RegisterStepJump();
        RightController.RegisterStepJump();
    }
}

