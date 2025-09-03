using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool IsMouseDown;
    public Color choosenColor;
    public int choosenNumber;
    public Transform pixelSlots;
    public NumbersMapConverter nmc;
    public TextMeshProUGUI progressText;
    public float totalRightPixels;
    public float totalPiece;

    // Start is called before the first frame update
    void Awake()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) IsMouseDown = true;
        if (Input.GetMouseButtonUp(0)) IsMouseDown = false;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
        }
        
    }
    [SerializeField] float progres = 0;

    void FixedUpdate()
    {
        totalRightPixels = 0;

        foreach (PixelSlotController pixelSlotController in nmc.transform.GetComponentsInChildren<PixelSlotController>())
        {
            if (pixelSlotController.isRightColor) totalRightPixels++;
        }
        progres = totalRightPixels *100f / totalPiece ;
        Debug.Log(totalRightPixels);
        Debug.Log(totalPiece);
        progressText.text =  progres +"%";
    }
    public Color ColorMix(Color c1, Color c2)
    {
        return ColorMixing.Mix(c1, c2);
    }

    public void GetColor(RectTransform colorT)
    {
        choosenColor = colorT.GetComponent<Image>().color;
        choosenNumber = int.Parse(colorT.GetComponentInChildren<TextMeshProUGUI>().text);
    }
    public void UpdateCompletedPixelSlots(int number , Color32 color)
    { 
    }
    [ContextMenu("update progress")]
    public void UpdateProgress()
    {
        
        Debug.Log(totalPiece);
    }
}
