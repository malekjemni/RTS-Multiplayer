using UnityEngine;
using System;

public class CountDownTimer : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute;
    public static int Hour;

    private float secondsToRealTime = 60f;
    private float timer;

    void Start()
    {
        Minute = 0;
        Hour = 60;
        timer = secondsToRealTime;
    }
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer >= 0)
        {
            Minute--;
            OnMinuteChanged?.Invoke();
            if (Minute < 0)
            {
                Minute = 59;
                Hour--;
                OnHourChanged?.Invoke();
                if (Hour < 0)
                {
                    Hour = 23;
                }              
            }           
            timer = secondsToRealTime;
        }
    }
}
