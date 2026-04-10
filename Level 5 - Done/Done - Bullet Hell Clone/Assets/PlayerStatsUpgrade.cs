using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUpgrade : MonoBehaviour
{
    public PlayerController pc;
    public BossController bossController;
    
    public TextMeshProUGUI moneyTxt;
    public TextMeshProUGUI maxHpUpgradeTxt;
    public TextMeshProUGUI bulletSpeedUpgradeTxt;
    public TextMeshProUGUI bulletDamageUpgradeTxt;
    public TextMeshProUGUI bulletCountUpgradeTxt;
    public TextMeshProUGUI firerateUpgradeTxt;

    public TextMeshProUGUI moneyMultiplierUpgradeTxt;
    private float baseCost = 25;
    void OnEnable()
    {
        Time.timeScale = 0;
        UpdateUI();
    }
    void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void UpgradeMaxHp()
    {
        pc.money -= baseCost * pc.playerStats.maxHp;
        pc.playerStats.maxHp++;
        UpdateUI();
    }
    public void UpgradeBulletSpeed()
    {
        pc.money -= baseCost * pc.playerStats.bulletSpeed;
        pc.playerStats.bulletSpeed++;
        UpdateUI();
    }
    public void UpgradeBulletDamage()
    {
        pc.money -= baseCost * pc.playerStats.bulletDamage;
        pc.playerStats.bulletDamage++;
        UpdateUI();
    }
    public void UpgradeBulletCount()
    {
        pc.money -= baseCost * pc.playerStats.bulletCount;
        pc.playerStats.bulletCount++;
        UpdateUI();
    }
    public void UpgradeFireRate()
    {
        pc.money -= baseCost * pc.playerStats.fireRate;
        pc.playerStats.fireRate++;
        UpdateUI();
    }
    public void UpgradeMoneyMultiplier()
    {
        pc.money -= baseCost * pc.playerStats.moneyMultiplier;
        pc.playerStats.moneyMultiplier += 0.1f;
        UpdateUI();
    }

    public void UpdateUI()

    {
        moneyTxt.text = $"Money : {pc.money}";
        maxHpUpgradeTxt.text = $"Max Hp : {pc.playerStats.maxHp}";
        if (pc.money < baseCost * pc.playerStats.maxHp) maxHpUpgradeTxt.GetComponentInChildren<Button>().interactable = false;
        else maxHpUpgradeTxt.GetComponentInChildren<Button>().interactable = true;

        bulletSpeedUpgradeTxt.text = $"Bullet Speed : {pc.playerStats.bulletSpeed}";
        if (pc.money < baseCost * pc.playerStats.bulletSpeed) bulletSpeedUpgradeTxt.GetComponentInChildren<Button>().interactable = false;
        else bulletSpeedUpgradeTxt.GetComponentInChildren<Button>().interactable = true;

        bulletDamageUpgradeTxt.text = $"Bullet Damage : {pc.playerStats.bulletDamage}";
        if (pc.money < baseCost * pc.playerStats.bulletDamage) bulletDamageUpgradeTxt.GetComponentInChildren<Button>().interactable = false;
        else bulletDamageUpgradeTxt.GetComponentInChildren<Button>().interactable = true;

        bulletCountUpgradeTxt.text = $"Bullet Count : {pc.playerStats.bulletCount}";
        if (pc.money < baseCost * pc.playerStats.bulletCount) bulletCountUpgradeTxt.GetComponentInChildren<Button>().interactable = false;
        else bulletCountUpgradeTxt.GetComponentInChildren<Button>().interactable = true;

        firerateUpgradeTxt.text = $"Fire rate : {pc.playerStats.fireRate}";
        if (pc.money < baseCost * pc.playerStats.fireRate) firerateUpgradeTxt.GetComponentInChildren<Button>().interactable = false;
        else firerateUpgradeTxt.GetComponentInChildren<Button>().interactable = true;

        moneyMultiplierUpgradeTxt.text = $"Money Multiplier : {pc.playerStats.moneyMultiplier}";
        if (pc.money < baseCost * pc.playerStats.moneyMultiplier) moneyMultiplierUpgradeTxt.GetComponentInChildren<Button>().interactable = false;
        else moneyMultiplierUpgradeTxt.GetComponentInChildren<Button>().interactable = true;
    }

    public void Continue()
    {
        gameObject.SetActive(false);
        pc.Replay();
        bossController.Replay();
    }
}
