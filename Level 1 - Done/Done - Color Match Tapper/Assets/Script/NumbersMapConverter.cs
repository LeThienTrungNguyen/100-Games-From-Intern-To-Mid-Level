using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class NumbersMapConverter : MonoBehaviour
{public  GameController gameController;
    public string[] paths = new string[] {
        "Level/8x8","Level/16x16","Level/32x32","Level/64x64","Level/128x128","Level/256x256"
    };
    public Texture2D sourceImage;     // Ảnh gốc (có thể không readable)
    public Texture2D previewImage;    // Ảnh hiển thị số (debug)
    [SerializeField] private List<Color32> palette;    // Danh sách màu
    [SerializeField] private int[,] numberMap;         // Bản đồ số cho từng pixel

    public Transform imagePiece;      // Prefab của square
    public float scalePiece;
    public BasicColorTabController bctc; //BasicColorTabController
    void Start()
    {
        int choosenPathIndex = Random.Range(0, paths.Length);
        string choosenPath = paths[choosenPathIndex];
        Texture2D[] pngFiles = Resources.LoadAll<Texture2D>(choosenPath);
        int choosenSourceImageIndex = Random.Range(0, pngFiles.Count());
        sourceImage = pngFiles[choosenSourceImageIndex];
        // Clone ảnh để đảm bảo luôn readable
        sourceImage = MakeReadable(sourceImage);
        GenerateNumberMap();
        GenerateSquaresFromMap();
        bctc.AddColorList(palette);
    }

    void GenerateNumberMap()
    {
        palette = new List<Color32>();

        int width = sourceImage.width;
        int height = sourceImage.height;
        numberMap = new int[width, height];

        Color32[] pixels = sourceImage.GetPixels32();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 c = pixels[y * width + x];

                // Tìm màu trong palette
                int index = palette.IndexOf(c);

                // Nếu chưa có thì thêm vào
                if (index == -1)
                {
                    palette.Add(c);
                    index = palette.Count - 1;
                }

                // Lưu ID = index + 1 (bắt đầu từ 1)
                numberMap[x, y] = index + 1;
            }
        }

        Debug.Log("Số màu khác nhau: " + palette.Count);

        // Sinh preview texture = gray-scale theo số ID
        previewImage = new Texture2D(width, height, TextureFormat.RGB24, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float v = (float)numberMap[x, y] / (palette.Count + 1);
                previewImage.SetPixel(x, y, new Color(v, v, v));
            }
        }
        previewImage.Apply();
    }

    public int GetNumberAtPixel(int x, int y)
    {
        if (x < 0 || x >= numberMap.GetLength(0) || y < 0 || y >= numberMap.GetLength(1))
            return -1;
        return numberMap[x, y];
    }

    public Color32 GetColorByNumber(int number)
    {
        if (number <= 0 || number > palette.Count)
            return Color.magenta; // fallback
        return palette[number - 1];
    }

    // Hàm generate square từ numberMap
    void GenerateSquaresFromMap()
    {
        int width = numberMap.GetLength(0);
        int height = numberMap.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int number = numberMap[x, y];
                if (number <= 0) continue;

                Color32 c = GetColorByNumber(number);
                if (c.a == 0) continue;

                // Tạo square
                Transform sq = Instantiate(
                    imagePiece,
                    new Vector3(x, y, 0) * scalePiece,
                    Quaternion.identity,
                    transform
                );

                sq.gameObject.SetActive(true);
                sq.localScale = Vector2.one * scalePiece;
                // Đặt màu nền = trắng (để người chơi tự tô)
                SpriteRenderer sr = sq.GetComponent<SpriteRenderer>();
                Color savedColor = GetColorByNumber(GetNumberAtPixel(x, y));
                if (sr != null)
                    sr.color = Color.white;

                // Gán số ID vào TextMeshPro (nằm trong child)
                sq.GetComponent<PixelSlotController>().rightColorNumber = number;
                var tmp = sq.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = number.ToString();
                    tmp.color = savedColor;//Color.black; // để dễ nhìn
                }
            }
        }
        gameController.totalPiece = transform.childCount;
    }

    // Hàm clone texture sang bản readable
    Texture2D MakeReadable(Texture2D src)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            src.width, src.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(src, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTex = new Texture2D(src.width, src.height);
        readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTex;
    }
}
