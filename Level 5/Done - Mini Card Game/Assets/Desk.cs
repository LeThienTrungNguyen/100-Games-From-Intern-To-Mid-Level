using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Desk : MonoBehaviour
{
    public List<Card> packTemplate;

    public List<Card> stock1Cards;
    public List<Card> stock2Cards = new();
    public Transform Stock1;
    public Transform Stock2;
    public Transform Foundation1;
    public Transform Foundation2;
    public Transform Foundation3;
    public Transform Foundation4;
    public Transform Tablue1;
    public Transform Tablue2;
    public Transform Tablue3;
    public Transform Tablue4;
    public Transform Tablue5;
    public Transform Tablue6;
    public Transform Tablue7;
    public float cardOffsetTablue;
    public List<Transform> tablueList;
    public List<Transform> foundationList;
    
    void Awake()
    {
        SpawnPack();
        FlipUpAllLastCardAtTablue();
    }
    void FlipUpAllLastCardAtTablue()
    {
        Tablue1.GetComponentsInChildren<Card>().Last().Flip();
        Tablue2.GetComponentsInChildren<Card>().Last().Flip();
        Tablue3.GetComponentsInChildren<Card>().Last().Flip();
        Tablue4.GetComponentsInChildren<Card>().Last().Flip();
        Tablue5.GetComponentsInChildren<Card>().Last().Flip();
        Tablue6.GetComponentsInChildren<Card>().Last().Flip();
        Tablue7.GetComponentsInChildren<Card>().Last().Flip();
    }
    void SpawnPack()
    {
        foreach (Card card in GetSuffledPack())
        {
            var cardObj = Instantiate(card, Stock1);
            stock1Cards.Add(cardObj);
        }

        for (int i = 6; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue1);
            stock1Cards.RemoveAt(0);
        }
        for (int i = 5; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue2);
            stock1Cards.RemoveAt(0);
        }
        for (int i = 4; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue3);
            stock1Cards.RemoveAt(0);
        }
        for (int i = 3; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue4);
            stock1Cards.RemoveAt(0);
        }
        for (int i = 2; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue5);
            stock1Cards.RemoveAt(0);
        }
        for (int i = 1; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue6);
            stock1Cards.RemoveAt(0);
        }
        for (int i = 0; i <= 6; i++)
        {
            MoveCard(stock1Cards[0], Tablue7);
            stock1Cards.RemoveAt(0);
        }
    }
    List<Card> GetSuffledPack()
    {
        List<Card> pack = new();
        foreach (Card card in packTemplate)
        {
            pack.Add(card);
        }
        pack = pack.OrderBy(x => Random.value).ToList();
        return pack;
    }
    #region Move Card
    public void MoveCard(Card card, Transform target)
    {
        // đặt vị trí
        if (target.CompareTag("Tablue"))
        {
            var cards = target.GetComponentsInChildren<Card>();
            if (cards.Length == 0)
            {
                card.transform.position = target.position;
            }
            else
            {
                card.transform.position =
                    cards.Last().transform.position + Vector3.down * cardOffsetTablue;
            }
        }
        else
        {
            card.transform.position = target.position;
        }

        // đổi parent
        card.transform.parent = target;
        // cập nhật sorting cho tất cả card trong target
        for (int i = 0; i < target.childCount; i++)
        {
            Card c = target.GetChild(i).GetComponent<Card>();
            c.UpdateSortingOrder();
        }
        // SET SORTING ORDER = số lượng card hiện tại trong parent
        //int newOrder = target.childCount;
        //card.GetComponent<SpriteRenderer>().sortingOrder = newOrder;
    }
    public Transform FindFoundationForCard(Card card)
    {
        foreach (Transform f in foundationList)
        {
            // Trường hợp foundation trống → chỉ Ace được đặt
            if (f.childCount == 0)
            {
                if (card.cardNumber == CardNumber.Ace)
                    return f;
                continue;
            }

            Card last = f.GetComponentsInChildren<Card>().Last();

            // Cùng chất và cao hơn foundation 1 đơn vị
            bool suitOK = (last.cardType == card.cardType);
            bool rankOK = ((int)card.cardNumber == (int)last.cardNumber + 1);

            if (suitOK && rankOK)
                return f;
        }

        return null;
    }

    public Transform FindEmptyFoundation()
    {
        foreach (Transform f in foundationList)
        {
            Debug.Log(f.childCount == 0 ? $"{f.name} is empty" : $"{f.name} is not empty");
            if (f.childCount == 0) return f;
        }
        return null;
    }
    public Card LastCardInTablue(Transform tablue)
    {
        return tablue.GetComponentsInChildren<Card>().Last();
    }

    // kiểm tra nếu lá bài hiện tại lớn hơn bài target 1 đơn vị
    public bool IsOneRankHigher(Card current, Card target)
    {
        return (int)current.cardNumber == (int)target.cardNumber + 1;
    }

    // kiểm tra nếu lá bài hiện tại nhỏ hơn bài target 1 đơn vị
    public bool IsOneRankLower(Card current, Card target)
    {
        return (int)target.cardNumber == (int)current.cardNumber + 1;
    }
    public bool IsSameSuit(Card current, Card target)
    {
        return current.cardType == target.cardType;
    }
    public Card GetLastCardInTablue(Transform tablue)
    {
        if (!tablue.CompareTag("Tablue")) return null;
        var cards = tablue.GetComponentsInChildren<Card>();
        if (cards.Length == 0) return null;
        return cards.Last();
    }
    public bool IsEmptyTablue(Transform tablue)
    {
        return tablue.childCount == 0;
    }
    public bool CanPlaceOnFoundation(Card card, Transform foundation)
    {
        if (foundation.childCount == 0)
        {
            // chỉ Ace mới vào foundation trống
            return card.cardNumber == CardNumber.Ace;
        }

        Card last = foundation.GetComponentsInChildren<Card>().Last();
        return last.cardType == card.cardType &&
               (int)card.cardNumber == (int)last.cardNumber + 1;
    }
    public void DrawFromStock()
    {
        // Stock1 còn bài → lật 1 lá
        if (stock1Cards.Count > 0)
        {
            Card cardToDraw = stock1Cards[0];
            stock1Cards.RemoveAt(0);

            MoveCard(cardToDraw, Stock2);
            stock2Cards.Add(cardToDraw);

            if (!cardToDraw.isFlip) cardToDraw.Flip();
            cardToDraw.GetComponent<SpriteRenderer>().sortingOrder = Stock2.childCount;

            return;
        }

        // Stock1 rỗng → reset Stock2
if (stock2Cards.Count > 0)
{
    for (int i = 0; i < stock2Cards.Count; i++)
    {
        Card card = stock2Cards[i];

        // Kiểm tra card vẫn còn ở Stock2
        if (card.transform.parent != Stock2) continue;

        card.isFlip = false; // úp lại
        card.ChangeImage();
        MoveCard(card, Stock1);
        stock1Cards.Add(card);
    }

    // Chỉ xóa các lá thực sự reset
    stock2Cards.RemoveAll(c => c.transform.parent == Stock1);
}

    }



    void Update()
{
    
    if (!Input.GetMouseButtonDown(0)) return;

    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // 1. Kiểm tra click Stock1
    Collider2D hitStock1 = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("StockArea"));
    if (hitStock1 != null && hitStock1.transform == Stock1)
    {
        DrawFromStock();
        return;
    }

    // 2. Raycast để tìm lá ngửa trên cùng (Tableau / Stock2)
    RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
    Card topCard = null;

    foreach (var hit in hits)
    {
        Card card = hit.collider.GetComponent<Card>();
        if (card != null && card.isFlip)
        {
            // Stock2: chỉ chọn lá trên cùng
            if (card.transform.parent == Stock2)
            {
                Card topStock2 = Stock2.GetComponentsInChildren<Card>().Last();
                if (card != topStock2) continue;
            }

            if (topCard == null || card.GetComponent<SpriteRenderer>().sortingOrder > topCard.GetComponent<SpriteRenderer>().sortingOrder)
            {
                topCard = card;
            }
        }
    }

    if (topCard != null)
    {
        topCard.OnCardClicked();
    }
}


    public void MoveSequence(List<Card> sequence, Transform target)
    {
        Transform oldParent = sequence[0].transform.parent;

        for (int i = 0; i < sequence.Count; i++)
        {
            Card card = sequence[i];
            MoveCard(card, target); // dùng MoveCard hiện tại
        }

        // Flip lá cuối cột cũ nếu còn bài
        Card lastOld = GetLastCardInTablue(oldParent);
        if (lastOld != null && !lastOld.isFlip) lastOld.Flip();
    }
public void CheckWinCondition()
{
    int kingCount = 0;

    foreach (Transform f in foundationList)
    {
        if (f.childCount == 0) continue;

        Card last = f.GetComponentsInChildren<Card>().Last();
        if (last.cardNumber == CardNumber.King)
        {
            kingCount++;
        }
    }

    if (kingCount == 4)
    {
        Debug.Log("🎉 You win! All 4 Kings are in Foundations!");
    }
}



    // kiểm tra nếu lá bài cùng chất với lá bài target
    #endregion
}
