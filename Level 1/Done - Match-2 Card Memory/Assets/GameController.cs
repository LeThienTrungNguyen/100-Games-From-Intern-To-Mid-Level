using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Sprite NoFlipCard;
    public int boardSizeX;
    public int boardSizeY;
    public Card[] Cards;
    public Card choosenCard1;
    public Card choosenCard2;
    public int pairTotal;
    public float pairedOffset;
    public int pairedCount;
    public string level;
    public float timescale = 1;
    void Awake()
    {
        instance = this;
        level = PlayerPrefs.GetString("level");
        DontDestroyOnLoad(this);
        /*pairTotal = boardSizeX * boardSizeY / 2;
        pairedOffset = -8f / (float)pairTotal;
        pairedCount = 0;*/
    }
    [HideInInspector]public LevelInfomation levelInfo;
    public GameManager gameManager;
    public TimerController timerController;
    void LevelInfomationLoad(string level)
    {
        if (level == "easy")
        {
            levelInfo = gameManager.easy;
            Debug.Log(levelInfo);
        }
        else if (level == "medium")
        {
            levelInfo = gameManager.medium;
        }
        else
        {
            levelInfo = gameManager.hard;
        }

        boardSizeX = levelInfo.sizeX; boardSizeY = levelInfo.sizeY;
        pairTotal = boardSizeX * boardSizeY / 2;
        pairedOffset = levelInfo.pairedCardEndPosition / (float)pairTotal;
        pairedCount = 0;
        timerController.TimerCountDownStart = levelInfo.timer;
        timerController.Timer.minValue = 0;
        timerController.Timer.maxValue = timerController.TimerCountDownStart;
        
        Camera.main.transform.position = levelInfo.cameraPosition;
        Camera.main.transform.GetComponent<Camera>().orthographicSize = levelInfo.cameraSize;
    }

    [ContextMenu("restart")]
    public void Restart()
    {
        //remove all child of not paired and paired card
        var notPairedCard = GameObject.Find("NotPairedCard");
        var PairedCard = GameObject.Find("PairedCard");
        for (int i = notPairedCard.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = notPairedCard.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        for (int i = PairedCard.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = PairedCard.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        // 
        timerController.TimerCountDownStart = levelInfo.timer;
        SpawnBoard();
    }
    void Start()
    {
        LevelInfomationLoad(level);
        SpawnBoard();
    }
    public bool showCard;
    void SpawnBoard()
    {
        // Spawn Board On New List
        List<Card> board = new();
        for (int i = 0; i < boardSizeX * boardSizeY; i += 2)
        {
            int index = Random.Range(0, Cards.Length);
            board.Add(Cards[index]);
            board.Add(Cards[index]);
        }
        board = board.OrderBy(x => Random.value).ToList();
        var parent = GameObject.Find("Board/NotPairedCard").transform;
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                Transform cardT = Instantiate(board[i * boardSizeY + j].transform, parent);
                cardT.position = new Vector2(i, j);
                cardT.localScale *= 0.3f;
                HideCard(cardT.GetComponent<Card>());
                if (showCard) ShowCard(cardT.GetComponent<Card>());
            }
        }
    }
    public bool cardFlipping;
    public void FlipCard(Card card)
    {
        cardFlipping = true;
        card.transform.DORotate(new Vector3(0, transform.localRotation.y + 90, 0), 0.25f).OnComplete(() =>
        {
            if (!card.isShowing) ShowCard(card);
            else HideCard(card);
            card.isShowing = !card.isShowing;
            card.transform.DORotate(Vector3.zero, 0.25f).OnComplete(() =>
            {
                cardFlipping = true;
            });
        });
    }
    void ShowCard(Card card)
    {
        card.GetComponent<SpriteRenderer>().sprite = card.Image;
        card.transform.localScale = Vector2.one * 0.3f;
    }

    void HideCard(Card card)
    {
        card.GetComponent<SpriteRenderer>().sprite = NoFlipCard;
        card.transform.localScale = Vector2.one * 0.83f;
    }

    public void ChooseCard(Card card)
    {
        if (choosenCard1 == null)
        {
            choosenCard1 = card;
        }
        else
        {
            choosenCard2 = card;
        }
        if (choosenCard2 != null)
        {
            Debug.Log("you choose 2 card ,let check card!!");
            if (choosenCard1 == choosenCard2)
            {
                Debug.Log("2 cards choosen is one");
                choosenCard1 = null;
                choosenCard2 = null;
            }
            else
            {
                Debug.Log("2 cards choosen is differense");
                if (choosenCard1.Type == choosenCard2.Type)
                {
                    Debug.Log("2 cards choosen is differense , but same type");
                    var PairedCards = GameObject.Find("PairedCard").transform;
                    PairedCards.position = levelInfo.pairedCardStartPosition;
                    var PairedCardPosition = (Vector3)levelInfo.pairedCardStartPosition;

                    Debug.Log("PairedCardPosition:" + PairedCardPosition);
                    //Destroy(choosenCard1.gameObject,0.5f);
                    //Destroy(choosenCard2.gameObject,0.5f);
                    var jumpPosition = PairedCardPosition - pairedCount * pairedOffset * (Vector3)Vector2.down;
                    pairedCount++;
                    choosenCard1.transform.parent = PairedCards.transform;
                    choosenCard2.transform.parent = PairedCards.transform;
                    choosenCard1.GetComponent<SpriteRenderer>().sortingOrder = pairedCount;
                    choosenCard2.GetComponent<SpriteRenderer>().sortingOrder = pairedCount;
                    timerController.AddTime();
                    StartCoroutine(MoveToPairedCard(choosenCard1, choosenCard2, jumpPosition));
                }
                else
                {
                    Debug.Log("2 cards choosen is differense , and different type");
                    Invoke("FlipChoosenCard1", 0.5f);
                    Invoke("FlipChoosenCard2", 0.5f);
                }
            }
        }
    }

    public IEnumerator MoveToPairedCard(Card card1, Card card2, Vector3 endPosition)
    {
        yield return new WaitForSeconds(0.5f);
        float jumppower = Random.Range(-2, 2.1f);
        int jumpnumbs = Random.Range(1, 5);
        float dur = Random.Range(0.75f, 2f);
        card1.transform.DOJump(endPosition, jumppower, jumpnumbs, dur);
        card2.transform.DOJump(endPosition, jumppower, jumpnumbs, dur);
        choosenCard1 = null;
        choosenCard2 = null;
    }

    public void FlipChoosenCard1()
    {
        FlipCard(choosenCard1);
        choosenCard1 = null;
    }
    public void FlipChoosenCard2()


    {
        FlipCard(choosenCard2);
        choosenCard2 = null;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray;
                RaycastHit2D hit2D;
                ray = Camera.main.ScreenPointToRay(touch.position);
                hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit2D.collider != null)
                {
                    Debug.Log("clicked on " + hit2D.transform.gameObject);
                    if (cardFlipping && choosenCard2 != null) return;
                    Card card = hit2D.transform.GetComponent<Card>();
                    FlipCard(card);
                    ChooseCard(card);
                }
            }
        }
    }
}
