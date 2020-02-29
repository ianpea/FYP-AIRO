using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController
{
    /*
     * Simplify fields
     */
    GameplayView View; // Currently not used.
    GameplayModel Model;
    GameplayController Controller;
    GameObject player;

    private Coroutine airTimeCo;
    private bool isAirTime = false;

    public delegate void OnGrounded();
    public OnGrounded onGrounded;

    public void Start()
    {
        View = GameplayView.Instance;
        Model = View.Model;
        Controller = View.Controller;

        Physics.gravity = new Vector3(0, GameplayModel.GRAVITY, 0);
        player = PlayerView.Instance.Model.go;

        PlayerView.Instance.Controller.onAir += AudioManager.Instance.SFXInAir;
        PlayerView.Instance.Controller.onAir += StartAirTime;
        PlayerView.Instance.Controller.onAir += StopAirTime;
        PlayerView.Instance.Controller.onJump += AudioManager.Instance.SFXJump;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            LCManager.LeftController.RegisterLeftStep();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LCManager.RightController.RegisterRightStep();
        }     
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            LCManager.RightController.RegisterStepJump();
        }
    }

    public void StartAirTime(PLAYER_STATE state)
    {
        if(state == PLAYER_STATE.Air && !isAirTime)
        {
            airTimeCo = View.StartCoroutine(AirTimeCo());
        }
    }

    public IEnumerator AirTimeCo()
    {
        RaycastHit hitInfo;
        isAirTime = true;
        while(PlayerView.Instance.Model.playerState == PLAYER_STATE.Air)
        {
            Physics.Raycast(player.transform.position, -player.transform.up, out hitInfo);
            if(hitInfo.distance > AirTime.MIN_AIR_DIST)
            {
                if(hitInfo.distance >= AirTime.MAX_AIR_DIST)
                {
                    GameplayUIManager.Instance.Power.gameObject.SetActive(true);
                    AirTime.currentAirTime += Time.deltaTime * 50;
                }
                else
                {
                    AirTime.currentAirTime += Time.deltaTime;
                }
            }
            yield return null;
        }
        yield return null;
    }

    public void StopAirTime(PLAYER_STATE state)
    {
        if (state == PLAYER_STATE.Grounded && isAirTime)
        {
            View.StopCoroutine(airTimeCo);
            ScoreManager.Instance.ScoreAirTime(AirTime.currentAirTime);
            ResetAirTime();
        }
    }

    public void ResetAirTime()
    {
        AirTime.currentAirTime = 0.0f;
        isAirTime = false;
    }

    public void CompleteLevel()
    {
        GameplayUIManager.Instance.ActivateCompleteScreen();
        Model.isLevelComplete = false;
    }

    public void CancelCompleteLevel()
    {
        GameplayUIManager.Instance.DeactivateCompleteScreen();
        Model.isLevelComplete = false;
    }
}
