using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public Rigidbody playerRb;

    public AudioSource Jump;
    public AudioSource Walk;
    public AudioSource InAir;
    public static float InAirFadeOutDistance = 75.0f;
    public AudioSource Bounce;
    public AudioSource CollectCoin;
    public AudioSource CollectMail;
    public AudioSource SubmitMail;
    public AudioSource Push;
    public AudioSource BGM;
    public AudioSource CompleteGuide;
    public AudioSource Cheer;
    public AudioSource Button;
    public AudioSource Whoosh;
    public AudioSource Landing;
    public AudioSource Hit;
    public AudioSource GameStart;
    public AudioSource BGMMenu;
    public AudioSource BGMTutorial;
    public AudioSource BGMLevel1;
    public AudioSource BGMLevel2;

    public const float timeToMaxVolume = 3.0f;

    private Coroutine inAirCo;
    public bool isInAir;

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
    }

    private void Update()
    {

    }
    public void SFXBounce()
    {
        Bounce.PlayOneShot(Bounce.clip);
    }

    public void SFXWalk()
    {
        Walk.PlayOneShot(Walk.clip);
    }

    public void SFXJump()
    {
        Jump.Play();
    }

    public void SFXCollectCoin()
    {
        CollectCoin.PlayOneShot(CollectCoin.clip);
    }

    public void SFXPush()
    {
        Push.Play();
    }

    public void SFXCollectMail()
    {
        CollectMail.PlayOneShot(CollectMail.clip);
    }

    public void SFXSubmitMail()
    {
        SubmitMail.Play();
    }

    public void SFXCompleteGuide()
    {
        CompleteGuide.Play();
    }

    public void SFXCheer()
    {
        Cheer.Play();
    }

    public void SFXButton()
    {
        Button.PlayOneShot(Button.clip);
    }

    public void SFXLanding()
    {
        Landing.PlayOneShot(Landing.clip);
    }

    public void SFXHit()
    {
        Hit.PlayOneShot(Hit.clip);
    }

    public void SFXWhoosh()
    {
        Whoosh.PlayOneShot(Whoosh.clip);
    }

    public void SFXGameStart()
    {
        GameStart.PlayOneShot(GameStart.clip);
    }

    public void SFXBGMMenu()
    {
        if (!BGMMenu.isPlaying)
            BGMMenu.Play();
    }

    public void SFXBGMTutorial()
    {
        if (!BGMTutorial.isPlaying)
            BGMTutorial.Play();
    }

    public void SFXBGMLevel1()
    {
        if (!BGMLevel1.isPlaying)
            BGMLevel1.Play();
    }

    public void SFXBGMLevel2()
    {
        if (!BGMLevel2.isPlaying)
            BGMLevel2.Play();
    }

    public void SFXInAir(PLAYER_STATE state)
    {
        if (state == PLAYER_STATE.Air && !isInAir)
        {
            inAirCo = StartCoroutine(SFXInAirCo());
        }
    }

    public void StopAllAudios()
    {
        AudioSource[] allAudios;
        allAudios = FindObjectsOfType<AudioSource>();
        foreach(AudioSource a in allAudios)
        {
            a.Stop();
        }
    }

    private IEnumerator SFXInAirCo()
    {
        isInAir = true;
        while (PlayerView.Instance.Model.playerState == PLAYER_STATE.Air)
        {
            if(playerRb.velocity.y > 0)
            {
                if (!InAir.isPlaying)
                {
                    InAir.Play();
                    InAir.volume = 0.0f;
                }
                else
                {
                    InAir.volume += Time.deltaTime / 5.0f;
                }
            }
            if(playerRb.velocity.y < 0)
            {
                if(PlayerStats.DistanceToBelow < AudioManager.InAirFadeOutDistance)
                {
                    InAir.volume -= Time.deltaTime * (PlayerStats.DistanceToBelow/AudioManager.InAirFadeOutDistance);
                }
                else
                {
                    if (!InAir.isPlaying)
                    {
                        InAir.Play();
                        InAir.volume = 0.0f;
                    }
                    else
                    {
                        InAir.volume += Time.deltaTime / 5.0f;
                    }
                }
            }
            yield return null;
        }
        if (PlayerView.Instance.Model.playerState == PLAYER_STATE.Grounded)
            InAir.volume = 0.0f;
        isInAir = false;
        yield break;
    }
}
