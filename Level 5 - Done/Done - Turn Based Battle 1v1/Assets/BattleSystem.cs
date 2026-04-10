using System;

public enum SkillType { Attack, Heal, Defend, Buff }
[System.Serializable]
public class Fighter {
    public string name;
    public int HP;
    public int MaxHP;
    public bool hasBuff;
    public SkillType lastSkill;
    public bool isDefending;
    public bool hasShieldStrong;
    public bool hasThorns;
    public int thornPoints = 25;
    public Tuple<int, int> dicesValue;

    public Fighter(string n, int hp) {
        name = n;
        MaxHP = hp;
        HP = hp;
        dicesValue = new Tuple<int, int>(1, 1);
    }
}
