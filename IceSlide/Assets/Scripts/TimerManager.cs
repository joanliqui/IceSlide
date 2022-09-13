
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float time = 60;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Color warningColor = Color.red;
    bool stopTimer = false;
    

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
            }

            DisplayTime(time);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        else if(timeToDisplay > 0)
        {
            timeToDisplay += 1;
        }
        
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
