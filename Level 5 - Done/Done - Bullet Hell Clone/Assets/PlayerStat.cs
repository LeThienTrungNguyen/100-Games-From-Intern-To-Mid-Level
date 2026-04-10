using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class PlayerStat
{
    public int maxHp = 1;
    public float bulletSpeed= 1;
    public int bulletDamage= 1;
    public int fireRate= 1; // bullet count per second
    public int bulletCount= 1;
    public float moneyMultiplier= 1;

    public PlayerStat()
    {
        /*
        
        // */
    }
    public PlayerStat(int maxHp, int bulletSpeed, int bulletDamage, int fireRate, int bulletCount, float moneyMultiplier)
    {
        this.maxHp = maxHp;
        this.bulletSpeed = bulletSpeed;
        this.bulletDamage = bulletDamage;
        this.fireRate = fireRate;
        this.bulletCount = bulletCount;
        this.moneyMultiplier = moneyMultiplier;
    }
    public void ResetStat()
    {
        maxHp = 1;
        bulletSpeed = 1f;
        bulletDamage = 1;
        fireRate = 1;
        bulletCount = 1;
        moneyMultiplier = 1f; //1x
    }
}
