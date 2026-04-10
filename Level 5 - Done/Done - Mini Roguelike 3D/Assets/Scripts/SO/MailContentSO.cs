using UnityEngine;

public enum MailType { Welcome, ToOfficial, ToAdvanced, Final, NoMoreOres, KPIFailed, BombLicenseSuccess, ReviveGreeting, QuestNew, QuestSuccess, QuestFailed }

[CreateAssetMenu(fileName = "NewMailContent", menuName = "ScriptableObjects/MailContent")]
public class MailContentSO : ScriptableObject
{
    public MailType mailType;
    public string title;
    [TextArea(10, 20)]
    public string message;
}
