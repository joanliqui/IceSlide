
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float time = 60;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Color warningColor = Color.red;
    bool stopTimer = false;

    private void Awake()
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        Debug.Log(minutes);
        Debug.Log(seconds);
    }

    private void Update()
    {
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
            }

            DisplayTime(time);
        }
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
}
