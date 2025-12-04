using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum CardNumber { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King } public enum CardType { Spade, Heart, Club, Diamond }
[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    public CardNumber cardNumber;
    public CardType cardType;
    public Sprite image;
    public Sprite Cardback;
    public bool isFlip = false;

    private Desk desk;

    void Awake()
    {
        desk = FindFirstObjectByType(typeof(Desk)) as Desk;
        transform.name = cardNumber.ToString() + " " + cardType.ToString();
    }

    void Start()
    {
        ChangeImage();
    }

    public void OnCardClicked()
    {
        if (!isFlip) return; // lá úp không thao tác

        // Stock2: chỉ thao tác lá trên cùng
        if (transform.parent == desk.Stock2)
        {
            Card topCard = desk.Stock2.GetComponentsInChildren<Card>().Last();
            if (topCard != this) return;
        }

        // Tự động đưa lên Foundation nếu là lá trên cùng
        Card[] cardsInParent = transform.parent.GetComponentsInChildren<Card>();
        if (cardsInParent.Last() == this)
        {
            Transform f = desk.FindFoundationForCard(this);
            if (f != null)
            {
                Transform oldParent = transform.parent;
                desk.MoveCard(this, f);

                Card lastOld = desk.GetLastCardInTablue(oldParent);
                if (lastOld != null && !lastOld.isFlip)
                    lastOld.Flip(); // flip ngay

                Debug.Log($"{name} moved to Foundation {f.name}");
                return;
            }
        }

        // Nếu là ACE → ưu tiên Foundation trống
        if (cardNumber == CardNumber.Ace)
        {
            Transform emptyF = desk.FindEmptyFoundation();
            if (emptyF != null)
            {
                Transform oldParent = transform.parent;
                desk.MoveCard(this, emptyF);

                Card lastOld = desk.GetLastCardInTablue(oldParent);
                if (lastOld != null && !lastOld.isFlip)
                    lastOld.Flip();

                Debug.Log("Put ACE to Foundation");
                return;
            }
        }

        // Nếu là KING → ưu tiên Tableau trống
        if (cardNumber == CardNumber.King)
        {
            foreach (Transform tab in desk.tablueList)
            {
                if (desk.IsEmptyTablue(tab))
                {
                    List<Card> sequence = GetMovableSequence();
                    desk.MoveSequence(sequence, tab);
                    Debug.Log("Put KING to empty Tableau");
                    return;
                }
            }
        }

        // Di chuyển dãy bài hợp lệ lên Tableau
        List<Card> movableSequence = GetMovableSequence();

        foreach (Transform tab in desk.tablueList)
        {
            if (desk.IsEmptyTablue(tab)) continue;

            Card last = desk.GetLastCardInTablue(tab);
            if (last == null) continue;

            Card first = movableSequence[0];
            bool rankOK = ((int)first.cardNumber == (int)last.cardNumber - 1);
            bool colorOK = (IsRed(first.cardType) != IsRed(last.cardType));

            if (rankOK && colorOK)
            {
                desk.MoveSequence(movableSequence, tab);
                Debug.Log($"Move sequence starting {first.name} onto {last.name}");
                return;
            }
        }
    }

    public List<Card> GetMovableSequence()
    {
        List<Card> sequence = new List<Card> { this };

        Transform parentTablue = transform.parent;
        Card[] cardsInTablue = parentTablue.GetComponentsInChildren<Card>();
        int startIndex = System.Array.IndexOf(cardsInTablue, this);

        for (int i = startIndex; i < cardsInTablue.Length - 1; i++)
        {
            Card current = cardsInTablue[i];
            Card next = cardsInTablue[i + 1];

            if (!next.isFlip) break;

            bool colorOK = (IsRed(current.cardType) != IsRed(next.cardType));
            bool rankOK = ((int)current.cardNumber == (int)next.cardNumber + 1);

            if (colorOK && rankOK)
                sequence.Add(next);
            else
                break;
        }

        return sequence;
    }

    bool IsRed(CardType t) => t == CardType.Heart || t == CardType.Diamond;

    public void Flip()
    {
        isFlip = true; // lật ngay
        ChangeImage();
    }

    public void ChangeImage()
    {
        GetComponent<SpriteRenderer>().sprite = isFlip ? image : Cardback;
    }

    public void UpdateSortingOrder()
    {
        GetComponent<SpriteRenderer>().sortingOrder = transform.GetSiblingIndex();
    }
}
