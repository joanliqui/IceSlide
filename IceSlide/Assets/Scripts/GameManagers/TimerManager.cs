
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float time = 60;
    private float initialTime;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Color warningColor = Color.red;

    PostProcessingHandler volume;
    bool stopTimer = false;

    bool ff = true;

    private void Awake()
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

    }
    private void Start()
    {
        volume = GameObject.FindGameObjectWithTag("PostProcessing").GetComponent<PostProcessingHandler>();
        initialTime = time;

        LevelManager.Instance.onLevelComplete.AddListener(StopTimer);
        //LevelManager.Instance.onLevelComplete.AddListener(RestoreContrast);
        LevelManager.Instance.onTimeEnded.AddListener(RestoreContrast);
    }

    private void Update()
    {
        //Por algun motivo ese primer frame hace que el tiempo baje de golpe así que ponemos un frame donde el tiempo no baja
        if (ff)
        {
            ff = false;
            return;
        }

        if (!stopTimer)
        {
            if(time > 0)
            {
                time -= Time.unscaledDeltaTime;
            }
            else
            {
                time = 0;
                stopTimer = true;
                LevelManager.Instance.onTimeEnded?.Invoke();
            }
            if(timerText)
                DisplayTime(time);

            //if(volume)
            //    volume.SetSaturationValue(CalculateSaturationLevel());
        }
    }

    private int CalculateSaturationLevel()
    {
        float t = Mathf.InverseLerp(0, initialTime, time);
        int r = (int)Mathf.Lerp(-100, 0, t);
        r = Mathf.Clamp(r, -100, 0);
        return r;
    }

    void DisplayTime(float timeToDisplay)
    {
        
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        if(seconds <= 5)
        {
            timerText.color = warningColor;
        }
        string s = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = s;
    }
    
    public void StopTimer()
    {
        stopTimer = true;
    }

    public void RestoreContrast()
    {
        volume.SetSaturationValue(0);
    }
}
