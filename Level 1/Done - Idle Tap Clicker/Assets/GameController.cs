using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float timeScale;
    public TextMeshProUGUI totalIncomePerSecond;
    public TextMeshProUGUI GingerCount;
    public List<Generator> generators;
    public double primaryCurrency;
    public double lifetimeCurrency;
    public double maxCurrencyThisRun;
    [Min(0.1f)]public double ginerPerClick = 0.1f;
    void Update()
    {
        primaryCurrency += GetTotalIncomePerSecond() * Time.deltaTime;
        Time.timeScale = timeScale;
    }
    void FixedUpdate()
    {
        totalIncomePerSecond.text = GetTotalIncomePerSecond().ToString("0.##") + "/s";
        GingerCount.text = primaryCurrency.ToString("0.##") + "";
    }
    public double GetTotalIncomePerSecond()
    {
        return generators.Sum(g => g.GetIncomePerSecond());
    }

    public Generator GetCheapestGenerator()
    {
        return generators.OrderBy(g => g.GetCostNext()).First();
    }

    public double ComputePrestigeAdCap()
    {
        return 150 * Math.Sqrt(lifetimeCurrency / 1e15);
    }

    public double ComputePrestigeRealm()
    {
        return (Math.Sqrt(1 + 8 * (maxCurrencyThisRun / 1e12)) - 1) / 2;
    }
    public void AddGingers()
    {
        ginerPerClick = GetTotalIncomePerSecond() / 20d;
        ginerPerClick = Math.Clamp(ginerPerClick, 0.1d, double.MaxValue);
        primaryCurrency += ginerPerClick;
     }
}
