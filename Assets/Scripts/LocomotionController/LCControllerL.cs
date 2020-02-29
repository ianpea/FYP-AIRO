using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Collections.Generic;


public class LCControllerL
{
    /*
     * Simplify fields.
     */
    LCManager Manager;
    LCModel Model;
    LCControllerL Controller;
    private float lastUpdateTime = 0.0f;
    private float updateCooldown = 0.02f;

    /*
     * Thread implementation
     */
    private Queue inputQueue; // From Arduino to Unity

    private Thread thread;

    private void StartThread()
    {
        //thread = new Thread(ThreadUpdate);
        //thread.Start();
    }

    private void ThreadUpdate()
    {
        if (Time.time - lastUpdateTime < updateCooldown) return;
        string dataString = "null received";

        if (Model.stream.IsOpen)
        {
            try
            {
                dataString = Model.stream.ReadLine();
            }
            catch (System.IO.IOException ioe)
            {
                Debug.Log("IOException: " + ioe.Message);
            }

        }
        else
        {
            dataString = "NOT OPEN";
        }
        //Debug.Log("RCV_ : " + dataString);

        if (!dataString.Equals("NOT OPEN"))
        {
            // recived string is  like  "accx;accy;accz;gyrox;gyroy;gyroz"
            char splitChar = ';';
            string[] dataRaw = dataString.Split(splitChar);

            //string a = "";
            //foreach(string s in dataRaw)
            //{
            //    a += s + " ";
            //}
            //Debug.Log(a);

            // nNormalize accelerometer values.
            float ax = Int32.Parse(dataRaw[0]) * LC_CONST.accNormalizer;
            float ay = Int32.Parse(dataRaw[1]) * LC_CONST.accNormalizer;
            float az = Int32.Parse(dataRaw[2]) * LC_CONST.accNormalizer;

            // Prevent
            if (Mathf.Abs(ax) - 1 < 0) ax = 0;
            if (Mathf.Abs(ay) - 1 < 0) ay = 0;
            if (Mathf.Abs(az) - 1 < 0) az = 0;


            Model.offsetX += ax;
            Model.offsetY += ay;
            Model.offsetZ += 0f; // The IMU module have value of z axis of 16600 caused by gravity


            //// Decrease noise effect
            //if (Mathf.Abs(gx) < LCCONST.noiseCancel) gx = 0f;
            //if (Mathf.Abs(gy) < LCCONST.noiseCancel) gy = 0f;
            //if (Mathf.Abs(gz) < LCCONST.noiseCancel) gz = 0f;

            //Model.angleX += gx;
            //Model.angleY += gy;
            //Model.angleZ += gz;

            //Debug.LogWarning("LEFT AY: " + ay);
            //Debug.LogWarning("LEFT AZ: " + az);
            //Debug.LogWarning(Model.isJump);
            if (PlayerView.Instance.Model.playerState == PLAYER_STATE.Grounded)
            {
                // Jump
                if (ay >= 1.0f && !PlayerView.Instance.Model.inAir)
                {
                    Model.isJump = true;
                    return;
                }
                
                if (ay <= -5.0f && !Model.isUp)
                {
                    RegisterLeftStep();
                    Model.isUp = true;
                }
                else if(ay >= -2.0f)
                {
                    Model.isUp = false;
                }
            }
        }
        Model.stream.DiscardInBuffer();
    }

    public void Start()
    {
        Manager = LCManager.Instance;
        Model = LCManager.LeftModel;
        Controller = LCManager.LeftController;
        //StartThread();
        /*
         * Initialize port, make sure Unity project settings is NET2.0 and NOT NET2.0 Subset.
         */
        Model.PortName = "COM5";
        Model.stream = new SerialPort("\\\\.\\" + Model.PortName, LC_CONST.baudrate);

        try
        {
            Model.stream.ReadTimeout = LC_CONST.readTimeout;
        }
        catch (System.IO.IOException ioe)
        {
            Debug.Log("IOException: " + ioe.Message);
        }

        Model.stream.Open();
    }

    public void Update()
    {
        ThreadUpdate();
    }

    // Left controller register Left step
    public void RegisterLeftStep()
    {
        if (PlayerView.Instance.Model.locomotionState == LOCOMOTION_STATE.Right
            || PlayerView.Instance.Model.locomotionState == LOCOMOTION_STATE.None)
        {
            PlayerView.Instance.Model.StepQueue.Add(StepType.Left);
            PlayerView.Instance.Model.locomotionState = LOCOMOTION_STATE.Left;
            PlayerView.Instance.Controller.PlayerStep();
        }
    }

    public void RegisterStepJump()
    {
        lock (LC_CONST.LOCKER)
        {
            if (PlayerView.Instance.Model.playerState == PLAYER_STATE.Grounded)
            {
                PlayerView.Instance.Controller.PlayerJump();
            }
        }
    }
}

