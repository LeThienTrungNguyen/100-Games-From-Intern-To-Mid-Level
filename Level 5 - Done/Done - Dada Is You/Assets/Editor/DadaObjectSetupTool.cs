using UnityEngine;
using UnityEditor;
using System.IO;

public class DadaObjectSetupTool : EditorWindow
{
    [MenuItem("Dada/Auto Setup All DadaObjects in Scene")]
    public static void SetupAllInScene()
    {
        DadaObject[] objects = FindObjectsOfType<DadaObject>();
        int count = 0;
        foreach (var obj in objects)
        {
            obj.AutoSetup();
            count++;
        }
        Debug.Log($"<color=green>Đã tự động setup {count} đối tượng DadaObject trong Scene!</color>");
    }

    [MenuItem("Dada/Auto Generator System")]
    public static void ShowWindow()
    {
        GetWindow<DadaObjectSetupTool>("Hệ Thống Tự Định Nghĩa");
    }

    private DefaultAsset spriteFolder;

    private void OnGUI()
    {
        GUILayout.Label("Tạo Object Hàng Loạt Từ Thư Mục", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("Chọn một thư mục chứa hình ảnh (Sprites) để hệ thống tự đọc tên file, kéo hình ảnh vào và tự định nghĩa Enum (text, object, noun, property...) ứng với tên đó.", MessageType.Info);
        
        spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField("Thư mục chứa Ảnh:", spriteFolder, typeof(DefaultAsset), false);

        if (GUILayout.Button("Tự động tạo và cấu hình GameObjects", GUILayout.Height(30)))
        {
            if (spriteFolder != null)
            {
                GenerateObjectsFromFolder();
            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn thư mục chứa hình ảnh (Sprites) trước!", "OK");
            }
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Cấu Hình Scene Hiện Tại", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Tự động cấu hình lại tất cả trong Scene", GUILayout.Height(30)))
        {
            SetupAllInScene();
        }
    }

    private void GenerateObjectsFromFolder()
    {
        string path = AssetDatabase.GetAssetPath(spriteFolder);
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });
        
        int createdCount = 0;
        
        // Tạo một GameObject gốc để chứa tụi nó cho gọn
        GameObject parentGo = new GameObject($"Generated_From_{Path.GetFileName(path)}");
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                GameObject go = new GameObject(sprite.name);
                go.transform.SetParent(parentGo.transform);
                
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                
                DadaObject dada = go.AddComponent<DadaObject>();
                // Gọi Explicitly để chắc ăn (mặc định AddComponent cũng gọi Reset rồi)
                dada.AutoSetup();
                
                createdCount++;
            }
        }
        
        EditorUtility.DisplayDialog("Hoàn tất", $"Đã tạo và tự động định nghĩa {createdCount} GameObject từ thư mục {path}.\nKiểm tra ở Hierarchy!", "OK");
    }
}
