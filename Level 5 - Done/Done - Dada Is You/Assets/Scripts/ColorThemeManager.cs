using UnityEngine;
using System.Collections.Generic;

public static class ColorThemeManager
{
    private static Dictionary<ObjectType, Color> currentLevelThemes = new Dictionary<ObjectType, Color>();

    // Bảng màu rực rỡ và chuyên nghiệp cho từng nhóm đối tượng
    private static readonly Dictionary<string, string[]> palettes = new Dictionary<string, string[]>()
    {
        { "Hero",    new[] { "#FFB7D5", "#FF9DCA", "#FFD1E6", "#FFFFFF" } }, // DADA, YOU
        { "Goal",    new[] { "#FBFF5F", "#FFF700", "#FFD300", "#E6E600" } }, // FLAG, WIN
        { "Obstacle",new[] { "#929292", "#7A7A7A", "#B0B0B0", "#606060" } }, // WALL, STOP
        { "Nature",  new[] { "#5FBFFF", "#0095FF", "#00D4FF", "#5F85FF" } }, // WATER, SINK
        { "Danger",  new[] { "#FF5F5F", "#FA3232", "#FF0000", "#FF8080" } }, // SKULL, LAVA, DEFEAT, HOT
        { "Flora",   new[] { "#79FF5F", "#46E632", "#BCFF9D", "#00A100" } }, // GRASS
        { "Heavy",   new[] { "#C89B5F", "#A07846", "#E6BA82", "#8C6432" } }, // ROCK, PUSH
        { "Neutral", new[] { "#FFFFFF", "#E0E0E0", "#F0F0F0", "#D0D0D0" } }  // IS
    };

    public static void PrepareNewLevel()
    {
        currentLevelThemes.Clear();
    }

    public static Color GetColorForObject(ObjectType type)
    {
        // 1. Tìm Base Type (ví dụ TEXT_WATER -> WATER)
        ObjectType baseType = GetBaseType(type);

        // 2. Nếu đã có màu được chọn cho level này rồi thì trả về luôn
        if (currentLevelThemes.ContainsKey(baseType))
        {
            return currentLevelThemes[baseType];
        }

        // 3. Nếu chưa có, chọn ngẫu nhiên 1 màu từ palette tương ứng
        string category = GetCategory(baseType);
        string[] hexColors = palettes[category];
        string chosenHex = hexColors[Random.Range(0, hexColors.Length)];
        
        Color chosenColor;
        if (ColorUtility.TryParseHtmlString(chosenHex, out chosenColor))
        {
            currentLevelThemes[baseType] = chosenColor;
            return chosenColor;
        }

        return Color.white;
    }

    private static ObjectType GetBaseType(ObjectType type)
    {
        string typeName = type.ToString();
        if (typeName.StartsWith("TEXT_"))
        {
            string baseTypeName = typeName.Substring(5);
            // Một số Text đặc biệt không đi kèm object
            if (baseTypeName == "IS" || baseTypeName == "YOU" || baseTypeName == "WIN" || 
                baseTypeName == "PUSH" || baseTypeName == "STOP" || baseTypeName == "SINK" || 
                baseTypeName == "DEFEAT" || baseTypeName == "HOT" || baseTypeName == "MELT")
            {
                return type;
            }

            if (System.Enum.TryParse(baseTypeName, out ObjectType result))
            {
                return result;
            }
        }
        return type;
    }

    private static string GetCategory(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.DADA:
            case ObjectType.TEXT_DADA:
            case ObjectType.TEXT_YOU:
                return "Hero";
            case ObjectType.FLAG:
            case ObjectType.TEXT_FLAG:
            case ObjectType.TEXT_WIN:
                return "Goal";
            case ObjectType.WALL:
            case ObjectType.TEXT_WALL:
            case ObjectType.TEXT_STOP:
                return "Obstacle";
            case ObjectType.WATER:
            case ObjectType.TEXT_WATER:
            case ObjectType.TEXT_SINK:
            case ObjectType.TEXT_MELT:
                return "Nature";
            case ObjectType.LAVA:
            case ObjectType.TEXT_LAVA:
            case ObjectType.SKULL:
            case ObjectType.TEXT_SKULL:
            case ObjectType.TEXT_DEFEAT:
            case ObjectType.TEXT_HOT:
                return "Danger";
            case ObjectType.GRASS:
            case ObjectType.TEXT_GRASS:
                return "Flora";
            case ObjectType.ROCK:
            case ObjectType.TEXT_ROCK:
            case ObjectType.TEXT_PUSH:
                return "Heavy";
            default:
                return "Neutral";
        }
    }
}
