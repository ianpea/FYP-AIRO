using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public const float slideSpeed = 10.0f;
    public const float slideTime = 1.0f;

    private static GameplayUIManager _instance;
    public static GameplayUIManager Instance { get { return _instance; } }

    public TMPro.TextMeshProUGUI Score;
    public TMPro.TextMeshProUGUI Add;
    public TMPro.TextMeshProUGUI Power;
    public TextMesh Clock;
    //public bool isClockStarted;
    //public const float waitTime = 1.5f;

    // Complete screen
    public TMPro.TextMeshProUGUI CompleteScreenDetails;
    public TMPro.TextMeshProUGUI NextLevelScreen;
    public TMPro.TextMeshProUGUI CollectedScore;
    public TMPro.TextMeshProUGUI CollectedCoin;
    public TMPro.TextMeshProUGUI CollectedMail;
    public TMPro.TextMeshProUGUI CollectedGmail;
    public Image CompleteScreenBackground;
    public GameObject CompleteScreen;
    public RectTransform CompleteScreenRectTransform;
    public Vector3 DefaultTransform;

    public bool isCountdown;

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
        CompleteScreen.gameObject.SetActive(false);
        DefaultTransform = CompleteScreenRectTransform.localPosition;
    }

    private void Update()
    {
        UpdateAirTime();
        UpdateScore();
        UpdateClock();
    }

    private void UpdateClock()
    {
        if (!GameplayView.Instance.Model.isLevelComplete)
        {
            GameTime.currentTime += Time.deltaTime;
            Clock.text = GameTime.currentTime.ToString("0.0");
        }
    }

    public void StopTimer()
    {
        GameplayView.Instance.Model.isLevelComplete = true;
    }

    public void UpdateAirTime()
    {
        if (AirTime.currentAirTime > 1.0f)
        {
            Add.gameObject.SetActive(true);
            Add.text = "current air time: " + (Mathf.Round(AirTime.currentAirTime *100f) / 100f).ToString();
        }
        else
        {
            Add.text = "";
            Power.gameObject.SetActive(false);
            Add.gameObject.SetActive(false);
        }
    }

    public void UpdateScore()
    {
        Score.text = (Mathf.Round(ScoreManager.Instance.currentScore)).ToString();
    }
    
    public void ActivateCompleteScreen()
    {
        VirtualScene currentScene = VirtualSceneManager.Instance.GetCurrentScene();
        if(currentScene.sceneIndex == 1)
            CompleteScreenDetails.text = "Tutorial Completed!";
        else
            CompleteScreenDetails.text = "Completed Level \n\"" + currentScene.sceneName + "\"";
        AudioManager.Instance.SFXSubmitMail();
        AudioManager.Instance.Invoke("SFXCheer", 1.824f);

        CollectedScore.text = ScoreManager.Instance.currentScore.ToString();
        CollectedCoin.text = ScoreManager.Instance.currentCoin.ToString() + "/" + currentScene.totalCoins.ToString();
        CollectedMail.text = ScoreManager.Instance.currentMail.ToString() + "/" + currentScene.totalMail.ToString();
        CollectedGmail.text = ScoreManager.Instance.currentGmail.ToString() + "/" + currentScene.totalGmail.ToString();
        CompleteScreen.SetActive(true);
        StartCoroutine(NextLevelCountdown(3.0f));
        StartCoroutine(SlideIntoView(CompleteScreenRectTransform, new Vector3(-13, 0, 0)));
    }

    public void DeactivateCompleteScreen()
    {
        StartCoroutine(SlideOutFromView(CompleteScreenRectTransform, DefaultTransform));
    }

    public IEnumerator SlideIntoView(RectTransform rt, Vector3 targetPos)
    {
        Vector3 oriPos = rt.localPosition;
        for (float t = 0.0f; t <= slideTime; t += Time.fixedDeltaTime)
        {
            rt.localPosition = Vector3.Lerp(oriPos, targetPos, t / slideTime);
            yield return null;
        }
        yield break;
    }

    public IEnumerator SlideOutFromView(RectTransform rt, Vector3 targetPos)
    {
        Vector3 oriPos = rt.localPosition;
        for (float t = 0.0f; t <= slideTime; t += Time.fixedDeltaTime)
        {
            rt.localPosition = Vector3.Lerp(oriPos, targetPos, t / slideTime);
            yield return null;
        }
        CompleteScreen.SetActive(false);
        yield break;
    }

    public IEnumerator NextLevelCountdown(float countdownTime)
    {
        isCountdown = true;
        if(VirtualSceneManager.Instance.currentSceneIndex == VirtualSceneManager.Instance.sceneList.Count - 1)
        {
            for (float t = countdownTime; t >= 0; t -= Time.deltaTime)
            {
                NextLevelScreen.text = "Return to menu: " + (int)t + "...";
                yield return null;
            }
        }
        else
        {
            for (float t = countdownTime; t >= 0; t -= Time.deltaTime)
            {
                NextLevelScreen.text = "Next level in: " + (int)t + "...";
                yield return null;
            }
        }
        isCountdown = false;
        DeactivateCompleteScreen();
        VirtualSceneManager.Instance.NextScene();
        yield break;
    }

    public void ResetClock()
    {
        GameTime.currentTime = 0.0f;
    }
}
