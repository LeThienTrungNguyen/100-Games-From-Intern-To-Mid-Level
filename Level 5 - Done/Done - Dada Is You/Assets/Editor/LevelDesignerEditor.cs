using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDesigner))]
public class LevelDesignerEditor : Editor
{
    private LevelDesigner t;
    private int customW, customH;
    private string[] availableOptions;
    
    // Lưu vị trí thanh cuộn
    private Vector2 gridScrollPos;

    private void OnEnable()
    {
        t = (LevelDesigner)target;
        
        // Cập nhật lại Grid Data nếu thiếu
        if (t.CurrentLevel != null)
        {
            var lvl = t.CurrentLevel;
            if (lvl.gridData == null || lvl.gridData.Length != lvl.height || (lvl.gridData.Length > 0 && lvl.gridData[0].cells.Length != lvl.width))
            {
                t.ResizeGrid(lvl.width, lvl.height);
            }
            customW = lvl.width;
            customH = lvl.height;
        }

        // Lấy danh sách toàn bộ các Loại Object từ Enum có sẵn
        string[] enumNames = System.Enum.GetNames(typeof(ObjectType));
        availableOptions = new string[enumNames.Length + 1];
        availableOptions[0] = "[Rỗng]"; // Thay chữ null cho dễ hiểu
        for (int i = 0; i < enumNames.Length; i++)
        {
            availableOptions[i + 1] = enumNames[i].ToLower();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Level Manager", EditorStyles.boldLabel);
        
        // BOX QUẢN LÝ LEVEL
        GUILayout.BeginVertical("helpbox");
        
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Level:", GUILayout.Width(100));
        
        // Slider giới hạn từ 0 tới giới hạn lớn nhất của Levels count
        int maxIndex = Mathf.Max(0, t.levels.Count - 1);
        int newLevelIndex = EditorGUILayout.IntSlider(t.currentLevelIndex, 0, maxIndex);
        if (newLevelIndex != t.currentLevelIndex)
        {
            Undo.RecordObject(t, "Change Level");
            t.currentLevelIndex = newLevelIndex;
            var switchLvl = t.CurrentLevel;
            if (switchLvl != null)
            {
                customW = switchLvl.width;
                customH = switchLvl.height;
            }
            EditorUtility.SetDirty(t);
        }
        GUILayout.EndHorizontal();

        LevelData currentLvl = t.CurrentLevel;
        if (currentLvl != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Name:", GUILayout.Width(100));
            string newName = EditorGUILayout.TextField(currentLvl.levelName);
            if (newName != currentLvl.levelName)
            {
                Undo.RecordObject(t, "Change Level Name");
                currentLvl.levelName = newName;
                EditorUtility.SetDirty(t);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
        if (GUILayout.Button("+ Tạo Level Mới", GUILayout.Height(25)))
        {
            Undo.RecordObject(t, "Add Level");
            t.AddNewLevel();
            customW = t.CurrentLevel.width;
            customH = t.CurrentLevel.height;
            EditorUtility.SetDirty(t);
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("- Xoá Level Này", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Xóa Level?", "Bạn có chắc muốn xóa vĩnh viễn Level này không?", "Có", "Không"))
            {
                Undo.RecordObject(t, "Remove Level");
                t.RemoveCurrentLevel();
                customW = t.CurrentLevel.width;
                customH = t.CurrentLevel.height;
                EditorUtility.SetDirty(t);
            }
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        // HIỂN THỊ KHAY AUTOTILE SETTINGS (Vì Custom Editor ghi đè sẽ che mất List mặc định của Unity)
        GUILayout.Space(10);
        SerializedProperty autotileProp = serializedObject.FindProperty("autotileRules");
        if (autotileProp != null)
        {
            EditorGUILayout.LabelField("Advance Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autotileProp, new GUIContent("Autotile Configs"), true);
        }

        if (currentLvl == null) return;

        GUILayout.Space(15);
        EditorGUILayout.LabelField("Level Layout Matrix", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Bấm vào ô hình vuông để chọn các đối tượng vật thể thả xuống.\nMỗi ô có thể chọn rất nhiều vật thể đè lên nhau dạng FLAG checkbox!\nCuộn để xem thêm.", MessageType.Info);

        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Số Cột (Columns):", GUILayout.Width(110));
        customW = EditorGUILayout.IntField(customW, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Số Hàng (Rows):", GUILayout.Width(110));
        customH = EditorGUILayout.IntField(customH, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        if (GUILayout.Button("Cập Nhật Kích Thước Bảng", GUILayout.Height(30)))
        {
            Undo.RecordObject(t, "Resize Grid");
            t.ResizeGrid(customW, customH);
            EditorUtility.SetDirty(t);
        }
        GUILayout.EndVertical();

        GUILayout.Space(15);
        
        // VẼ GRID NHIỀU Ô
        if (currentLvl.gridData != null && currentLvl.gridData.Length == currentLvl.height)
        {
            GUIStyle btnStyle = new GUIStyle(EditorStyles.popup);
            btnStyle.alignment = TextAnchor.MiddleCenter;
            btnStyle.fixedHeight = 35; // Làm to nút một chút cho dễ bấm
            
            // Bọc toàn bộ bảng bằng ScrollView, giới hạn chiều cao tối thiểu để nhìn cho thoải mái
            gridScrollPos = EditorGUILayout.BeginScrollView(gridScrollPos, GUILayout.MinHeight(400), GUILayout.MaxHeight(600));
            GUILayout.BeginVertical("box");
            
            for (int y = 0; y < currentLvl.height; y++)
            {
                GUILayout.BeginHorizontal();
                if (currentLvl.gridData[y] != null && currentLvl.gridData[y].cells != null)
                {
                    for (int x = 0; x < currentLvl.width; x++)
                    {
                        string val = currentLvl.gridData[y].cells[x];
                        bool isNull = (val == "null" || string.IsNullOrEmpty(val) || val == "[Rỗng]");
                        
                        // Đổi màu nếu ô rỗng
                        GUI.backgroundColor = isNull ? new Color(0.9f, 0.9f, 0.9f) : new Color(0.8f, 1f, 0.8f);
                        
                        string displayVal = isNull ? "[---]" : val;

                        // Ép MinWidth = 80 để khi bảng to thì nó không bị bóp nghẹt mà đẩy ra thanh cuộn ngang
                        if (EditorGUILayout.DropdownButton(new GUIContent(displayVal), FocusType.Keyboard, btnStyle, GUILayout.MinWidth(80), GUILayout.ExpandWidth(true)))
                        {
                            GenericMenu menu = new GenericMenu();
                            int safeX = x;
                            int safeY = y;
                            
                            // Hàm cục bộ kiểm tra xem option này có đang được chọn không (để hiện dấu Tick dạng FLAG)
                            System.Func<string, bool> isMenuChecked = (string opt) => {
                                if (opt == "[Rỗng]" || opt == "null") return isNull;
                                if (isNull) return false;
                                string currentData = currentLvl.gridData[safeY].cells[safeX];
                                return System.Array.Exists(currentData.Split('|'), e => e.Trim() == opt);
                            };

                            foreach (string opt in availableOptions)
                            {
                                bool isCurrentlyChecked = isMenuChecked(opt);
                                
                                menu.AddItem(new GUIContent(opt), isCurrentlyChecked, () => {
                                    Undo.RecordObject(t, "Select Object Flags");
                                    
                                    if (opt == "[Rỗng]" || opt == "null")
                                    {
                                        currentLvl.gridData[safeY].cells[safeX] = "null";
                                    }
                                    else
                                    {
                                        string current = currentLvl.gridData[safeY].cells[safeX];
                                        
                                        if (current == "null" || string.IsNullOrEmpty(current) || current == "[Rỗng]")
                                        {
                                            currentLvl.gridData[safeY].cells[safeX] = opt;
                                        }
                                        else
                                        {
                                            // Xử lý bật/tắt dạng biến cờ (FLAG toggle)
                                            var parts = new System.Collections.Generic.List<string>(current.Split('|'));
                                            for(int i=0; i<parts.Count; i++) parts[i] = parts[i].Trim();
                                            
                                            if (parts.Contains(opt))
                                            {
                                                parts.Remove(opt); // Tắt tick -> gỡ bỏ
                                            }
                                            else
                                            {
                                                parts.Add(opt); // Bật tick -> thêm vào xếp chồng
                                            }
                                            
                                            if (parts.Count == 0) currentLvl.gridData[safeY].cells[safeX] = "null";
                                            else currentLvl.gridData[safeY].cells[safeX] = string.Join("|", parts.ToArray());
                                        }
                                    }
                                    EditorUtility.SetDirty(t);
                                });
                            }
                            menu.ShowAsContext();
                        }

                        GUI.backgroundColor = Color.white;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView(); // Đóng ScrollView
        }

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = new Color(0.4f, 1f, 0.4f);
        if (GUILayout.Button("Sinh Level Hiện Tại (Bake Level)", GUILayout.Height(40)))
        {
            t.SpawnLevel();
        }
        GUI.backgroundColor = Color.white;

        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("Xoá Sạch Scene (Clear Level)", GUILayout.Height(40)))
        {
            t.ClearLevel();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
