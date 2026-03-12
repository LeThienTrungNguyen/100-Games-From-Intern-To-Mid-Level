using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DotweenAnimationName : MonoBehaviour
{
    public static DotweenAnimationName Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    public void DoPulseEffect(Transform t , float scale1 , float scale2 , float time) {
        if (t == null) return;
        t.DOKill();
        t.localScale = Vector3.one * scale1;
        t.DOScale(scale2, time/2).SetUpdate(true).OnComplete(() =>
        {
            if (t != null)
            {
                t.DOScale(scale1, time/2).SetUpdate(true).OnComplete(() => {
                    if (t != null) t.DOKill();
                });
            }
        });
    }

    public void DoScaleUp(Transform t , float scale , float time , bool hideOnComplete = false) {
        if (t == null) return;
        t.DOKill();
        t.localScale = Vector3.zero;
        t.DOScale(scale, time).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
            if (t != null) t.DOKill();
        });
    }

    public void DoScaleDown(Transform t, float scale, float time, bool hideOnComplete = false)
    {
        if (t == null) return;
        t.DOKill();
        t.DOScale(scale, time).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            if (t != null)
            {
                t.DOKill();
                if (hideOnComplete) { t.gameObject.SetActive(false); }
            }
        });
    }

    public void DoPunchScale(Transform t, float strength, float time)
    {
        if (t == null) return;
        t.DOKill();
        t.DOPunchScale(Vector3.one * strength, time, 5, 1f).SetUpdate(true);
    }

    public void DoShakeCamera(float duration, float strength)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(duration, strength, 10, 90, false, true).SetUpdate(true);
    }

    public void DoBlinkEffect(Transform t1, Transform t2 , float time ,bool doSomething = false , Action action1 = null, Action action2 = null) {
        if (t1 == null || t2 == null) return;
        
        t1.DOKill();
        t2.DOKill();

        t1.DOScaleY(1, time/2).SetUpdate(true);
        t2.DOScaleY(1, time/2).SetUpdate(true).OnComplete(() => { 
            if(doSomething && action1 !=null ) action1.Invoke();
            
            if (t1 != null) t1.DOScaleY(0, time / 2).SetUpdate(true);
            if (t2 != null) t2.DOScaleY(0, time / 2).SetUpdate(true).SetDelay(0.05f).OnComplete(() => { 
                if (doSomething && action2 != null) action2.Invoke(); 
            });
        });
    }

    public void DoAnimateNumber(TextMeshProUGUI textComponent, string prefix, float targetValue, float durationPerNumber)
    {
        if (textComponent == null) return;
        float startValue = 0;
        DOTween.To(() => startValue, x => {
            if (textComponent != null)
            {
                startValue = x;
                textComponent.text = $"{prefix}{Mathf.FloorToInt(startValue)}";
            }
        }, targetValue, durationPerNumber * 1f).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public IEnumerator Co_AnimateNumberByStep(TextMeshProUGUI textComponent, string prefix, float targetValue, string suffix = "")
    {
        if (textComponent == null) yield break;
        float currentValue = 0;
        
        if (targetValue <= 0)
        {
            textComponent.text = $"{prefix}0{suffix}";
            yield break;
        }

        float power = Mathf.Floor(Mathf.Log10(targetValue / 10.1f)); 
        float step = Mathf.Pow(10, Mathf.Max(0, power));

        while (currentValue < targetValue)
        {
            if (textComponent == null) yield break; // Chốt chặn nếu text bị hủy
            
            currentValue += step;
            if (currentValue > targetValue) currentValue = targetValue;
            
            textComponent.text = $"{prefix}{Mathf.FloorToInt(currentValue)}{suffix}";

            // PHÁT TIẾNG ĐẾM SỐ
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayCounterTickSound(textComponent.transform.position);

            yield return new WaitForSecondsRealtime(0.05f); 
        }
        
        if (textComponent != null) textComponent.text = $"{prefix}{targetValue}{suffix}";
    }
}