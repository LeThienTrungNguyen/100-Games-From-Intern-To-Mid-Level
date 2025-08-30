using System;
using UnityEngine;
[System.Serializable]
public class Generator
{
    public string name;
    public Sprite icon; 
    public double baseCost;
    public double costGrowthRate;
    public double baseIncome;
    public int ownedCount;
    public double multiplier = 1;

    public double GetCostNext()
    {
        return  baseCost * Math.Pow(costGrowthRate, ownedCount);
    }

    public double GetIncomePerSecond()
    {
        return baseIncome * ownedCount * multiplier;
    }
}
