using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public Slider Timer;
    public CanvasController canvas;
    public  float TimerCountDownStart;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        TimerCountDownStart -= Time.deltaTime;
        if (TimerCountDownStart <= 0)
        {
            canvas.GameOverPanel.gameObject.SetActive(true);
        }
        Timer.value = TimerCountDownStart;
    }

    public  void AddTime()
    {
        AddTime(2);
    }
    public  void AddTime(float time)
    {
        TimerCountDownStart += time;
    }
}
