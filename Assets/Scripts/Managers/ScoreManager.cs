using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }

    public const int scoreMultiplier = 10;

    public int currentScore = 0;
    public int currentMail = 0;
    public int currentCoin = 0;
    public int currentGmail = 0;
    public float totalAirTime = 0.0f;

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

    public void Score(Collectible c)
    {
        if(c.name == "MailBasic")
        {
            currentMail += 1;
        }else if(c.name == "MailGmail")
        {
            currentGmail += 1;
        }
        else if(c.name == "Coin")
        {
            currentCoin += 1;
        }
        currentScore += c.score;
    }

    public void Score(Vehicle v)
    {
        currentScore += v.score;
    }

    public void ScoreAirTime(float airTime)
    {
        totalAirTime += airTime;
        currentScore += (int)airTime * scoreMultiplier;
    }
}
