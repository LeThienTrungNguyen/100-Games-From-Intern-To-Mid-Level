using UnityEngine;
using DG.Tweening; // nhớ import DOTween

public class Gem : MonoBehaviour
{
    public int x, y;
    public GameManager board;

    void OnMouseDown() => board.OnGemClicked(this);

    public void SetPos(int newX, int newY, float duration = 0.2f)
    {
        x = newX; y = newY;
        transform.DOMove(new Vector2(newX, newY), duration).SetEase(Ease.OutQuad);
    }
}
