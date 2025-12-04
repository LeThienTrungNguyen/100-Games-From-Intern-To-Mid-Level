using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum Phase
{
    Phase1,Phase2,Phase3,Phase4,Phase5
}
public class BossController : MonoBehaviour
{
    int baseHp = 100;
    float hpScale = 3f;
    [Min(0)] private int currentHpBars = 5;
    private int currentHp = 0;
    private int maxHp = 0;
    float movespd;// move speed
    bool rightDir;
    [Range(1, 5)] int currentPhase = 1;
    bool canFlip = true;
    public Phase1Controller pc1;
    public Phase2Controller pc2;
    public Phase3Controller pc3;
    public Phase4Controller pc4;
    public Phase5Controller pc5;

    public Slider hpUI;
    public TextMeshProUGUI hpUIText;
    void Awake()
    {
        SetHpByScaling();
    }
    void Start()
    {
        EnablePhase(currentPhase, true);
    }
    void SetHpByScaling()
    {
        maxHp = baseHp * (int)Math.Pow(hpScale, (5 - currentHpBars));
        UpdateHp(maxHp);
        UpdateHpUI();
    }
    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        var dir = rightDir ? Vector2.right : Vector2.left;

        if (transform.position.x < -5f && canFlip)
        {
            rightDir = true;
            canFlip = false;
        }
        else if (transform.position.x > 5f && canFlip)
        {
            rightDir = false;
            canFlip = false;
        }

        if (transform.position.x > -4.9f && transform.position.x < 4.9f)
        {
            canFlip = true;
        }

        GetComponent<Rigidbody2D>().velocity = dir;
    }

    [ContextMenu("Take Damage Test")]
    public void TakeDamage()
    {
        TakeDamage(1);
    }
    public void TakeDamage(int damage)
    {
        UpdateHp(currentHp - damage);
        UpdateHpUI();
        if (currentHp <= 0)
        {
            currentHpBars--;
            if (currentHpBars >= 0)
            {
                SetHpByScaling();
                ChangePhase(5 - currentHpBars + 1);
            }
            else
            {
                UpdateHp(0);
                UpdateHpUI();
                Debug.Log("Boss defeated!");
                Destroy(gameObject);
                Invoke(nameof(QuitGame), 3f);
            }

        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void UpdateHp(int hp)
    {
        currentHp = hp;
    }
    public void Replay()
    {
        currentHpBars = 5;
        currentPhase = 1;
        SetHpByScaling();
        DisablePhaseAll();
        EnablePhase(currentPhase);
    }
    public void UpdateHpUI()
    {
        hpUI.maxValue = maxHp;
        hpUI.value = currentHp;
        hpUIText.text = currentHpBars + "";
    }
    #region Phase Logic
    public void ChangePhase(int phase)
    {
        DisablePhaseAll();
        EnablePhase(phase);
    }
    public void EnablePhase(int phase, bool enable = true)
    {
        switch (phase)
        {
            case 1: EnablePhase1(enable);break;
            case 2: EnablePhase2(enable);break;
            case 3: EnablePhase3(enable);break;
            case 4: EnablePhase4(enable);break;
            case 5: EnablePhase5(enable);break;
            default : EnablePhase5(enable);break;
        }
    }
    public void EnablePhase1(bool enable)
    {
        pc1.gameObject.SetActive(enable);
    }
    public void EnablePhase2(bool enable)
    {
        pc2.gameObject.SetActive(enable);
    }
    public void EnablePhase3(bool enable)
    {
        pc3.gameObject.SetActive(enable);
    }
    public void EnablePhase4(bool enable)
    {
        pc4.gameObject.SetActive(enable);
    }
    public void EnablePhase5(bool enable)
    {
        pc5.gameObject.SetActive(enable);
    }
    public void EnablePhaseAll()
    {
        EnablePhase1(true);
        EnablePhase2(true);
        EnablePhase3(true);
        EnablePhase4(true);
    }
    public void DisablePhaseAll()
    {
        EnablePhase1(false);
        EnablePhase2(false);
        EnablePhase3(false);
        EnablePhase4(false);
    }
    #endregion 
    
}