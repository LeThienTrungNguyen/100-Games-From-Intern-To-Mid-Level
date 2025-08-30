using UnityEngine;
using DG.Tweening;
public class CanvasController : MonoBehaviour
{
    public RectTransform ScrollView;
    public float HideScrollViewPosX;
    public float ShowScrollViewPosX;
    public bool IsScrollViewShowing = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsScrollViewShowing) HideScrollView(0);
        else ShowScrollView(0);
    }

    public void ToggleScrollView()
    {
        //DOTween.KillAll();
        IsScrollViewShowing = !IsScrollViewShowing;
        if (IsScrollViewShowing) ShowScrollView();
        else HideScrollView();
    }
    public void ShowScrollView(float time = 1f)
    {
        ScrollView.DOLocalMoveX(ShowScrollViewPosX, time);
    }

    public void HideScrollView(float time = 1f)
    { 
        ScrollView.DOLocalMoveX(HideScrollViewPosX, time);
    }
}
