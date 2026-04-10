using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tag
{
    public string dices = "Dices";
    public string skill_1 = "Skill 1";
    public string skill_2 = "Skill 2";
    public string skill_3 = "Skill 3";
    public string skill_4 = "Skill 4";
}

public class GameController : MonoBehaviour
{
    public Animator dice1AnimatorPlayer;
    public Animator dice2AnimatorPlayer;
    public Animator dice1AnimatorAI;
    public Animator dice2AnimatorAI;
    public Fighter player;
    public Fighter AI;
    public Transform dices;
    public Transform AIDices;
    public Transform skills;
    public Transform playerHealthBar;
    public Transform AIHealthBar;

    public bool isPlayerTurn;
    public bool canPlayerInteract;
    public bool canPlayerRoll;
    public bool canPlayerUseSkill;

    private bool aiActionInProgress = false;
    private Vector3 baseHealthScale = new Vector3(6f, 0.4f, 1f);
    bool gameOver;

    void Awake()
    {
        player = new Fighter("Player", 100);
        AI = new Fighter("AI", 100);

        playerHealthBar.localScale = baseHealthScale;
        AIHealthBar.localScale = baseHealthScale;

        DisableSkillsPlayer();
        canPlayerUseSkill = false;

        Debug.Log("🎮 Game Started!");
    }

    void Update()
    {
        if (gameOver)
        {
            StopAllCoroutines();
            return;
        }

        if (isPlayerTurn)
        {
            if (Input.GetMouseButtonDown(0) && canPlayerInteract)
            {
                string tag = GetTagOnClick();
                switch (tag)
                {
                    case null: return;
                    case "Dices": RollDicesPlayer(); break;
                    case "Skill 1": if (canPlayerUseSkill) StartCoroutine(ExecuteSkillRoutine(1, player, AI, true)); break;
                    case "Skill 2": if (canPlayerUseSkill) StartCoroutine(ExecuteSkillRoutine(2, player, AI, true)); break;
                    case "Skill 3": if (canPlayerUseSkill) StartCoroutine(ExecuteSkillRoutine(3, player, AI, true)); break;
                    case "Skill 4": if (canPlayerUseSkill) StartCoroutine(ExecuteSkillRoutine(4, player, AI, true)); break;
                }
            }
        }
        else
        {
            if (!aiActionInProgress)
                AIDecideAction();
        }
    }

    public void AIDecideAction()
    {
        aiActionInProgress = true;
        StartCoroutine(AIDecideActionRoutine());
    }

    IEnumerator AIDecideActionRoutine()
    {
        Debug.Log("🤖 AI starts its turn...");
        RollDicesAI();

        float waitTime = 2f / dice2AnimatorAI.speed;
        yield return new WaitForSeconds(waitTime);

        int skillId;
        if (((float)AI.HP / AI.MaxHP) * 100f < 30f)
            skillId = 2; // Heal
        else if (((float)AI.HP / AI.MaxHP) * 100f > 70f)
            skillId = 1; // Attack
        else
        {
            skillId = UnityEngine.Random.Range(1, 5);
            while (skillId == 4 && AI.lastSkill == SkillType.Buff)
                skillId = UnityEngine.Random.Range(1, 5);
        }

        yield return StartCoroutine(ExecuteSkillRoutine(skillId, AI, player, false));

        EnableDicesPlayer(dices);
        DisableAllSkills();
        aiActionInProgress = false;

        Debug.Log("🌀 AI finished its turn. Player’s turn begins!");
    }

    public Tuple<int, int> GetDicesValue()
    {
        int t1 = UnityEngine.Random.Range(1, 7);
        int t2 = UnityEngine.Random.Range(1, 7);
        return new Tuple<int, int>(t1, t2);
    }

    [ContextMenu("Roll Dices")]
    public void RollDicesPlayer()
    {
        if (!canPlayerRoll) return;
        DisablePlayerInteracble();

        dice1AnimatorPlayer.speed = 1;
        dice2AnimatorPlayer.speed = 0.8f;
        dice1AnimatorPlayer.Play("Dice Roll");
        dice2AnimatorPlayer.Play("Dice Roll");

        player.dicesValue = GetDicesValue();
        dice1AnimatorPlayer.SetInteger("Dice Value", player.dicesValue.Item1);
        dice2AnimatorPlayer.SetInteger("Dice Value", player.dicesValue.Item2);

        Debug.Log($"🎲 Player rolled [{player.dicesValue.Item1}, {player.dicesValue.Item2}] total {GetDicesTotal(player)} points.");
        if (IsDoubleDicesValue(player))
            Debug.Log($"🔥 Player rolled a DOUBLE! ({player.dicesValue.Item1}, {player.dicesValue.Item2}) . Player got a special effect");

        canPlayerRoll = false;
        DisableDicesPlayer(dices);

        Invoke(nameof(EnableSkillsPlayer), 2 / dice2AnimatorPlayer.speed);
        Invoke(nameof(EnablePlayerInterable), 2 / dice2AnimatorPlayer.speed);
    }

    public void RollDicesAI()
    {
        dice1AnimatorAI.speed = 1;
        dice2AnimatorAI.speed = 0.8f;
        dice1AnimatorAI.Play("Dice Roll");
        dice2AnimatorAI.Play("Dice Roll");

        AI.dicesValue = GetDicesValue();
        dice1AnimatorAI.SetInteger("Dice Value", AI.dicesValue.Item1);
        dice2AnimatorAI.SetInteger("Dice Value", AI.dicesValue.Item2);

        Debug.Log($"🎲 AI rolled [{AI.dicesValue.Item1}, {AI.dicesValue.Item2}] total {GetDicesTotal(AI)} points.");
        if (IsDoubleDicesValue(AI))
            Debug.Log($"🔥 AI rolled a DOUBLE! ({AI.dicesValue.Item1}, {AI.dicesValue.Item2}) . AI got a special effect !");
    }

    public IEnumerator ExecuteSkillRoutine(int skillID, Fighter excuter, Fighter target, bool isPlayerAction)
    {
        int totalDices = GetDicesTotal(excuter);
        var skill = ChooseSkill(skillID);
        string actor = excuter == player ? "Player" : "AI";

        if (isPlayerAction)
        {
            canPlayerUseSkill = false;
            DisableSkillsPlayer();
        }

        switch (skill)
        {
            case SkillType.Attack:
                {
                    int baseAttack = 20;
                    int buffDamage = excuter.hasBuff ? (baseAttack / 2) : 0;
                    bool isDoubleDices = IsDoubleDicesValue(excuter);
                    int totalDamage = (baseAttack + buffDamage + totalDices) * (isDoubleDices ? 2 : 1);

                    int targetDefenseValue = target.isDefending ? 20 : 0;
                    int targetBuffDefense = target.hasBuff ? targetDefenseValue / 2 : 0;
                    int totalEnemyDefenseValue = targetDefenseValue + targetBuffDefense;

                    int finalDamage = Mathf.Clamp((totalDamage - totalEnemyDefenseValue), 0, int.MaxValue);
                    target.HP -= finalDamage;

                    if (target.hasThorns)
                        excuter.HP -= (int)(finalDamage * 0.2f);

                    excuter.lastSkill = SkillType.Attack;

                    Debug.Log($"⚔️ {actor} used ATTACK and dealt {finalDamage} damage to {(actor == "Player" ? "AI" : "Player")}.");
                    break;
                }

            case SkillType.Heal:
                {
                    int baseHeal = 15;
                    int totalHeal = baseHeal + (excuter.hasBuff ? 10 : 0);
                    if (IsDoubleDicesValue(excuter))
                    {
                        excuter.HP = excuter.MaxHP;
                        Debug.Log($"💖 {actor} rolled DOUBLE and fully healed!");
                    }
                    else
                    {
                        excuter.HP += totalHeal;
                        Debug.Log($"💖 {actor} used HEAL and recovered {totalHeal} HP.");
                    }

                    excuter.lastSkill = SkillType.Heal;
                    break;
                }

            case SkillType.Defend:
                {
                    excuter.isDefending = true;
                    int defendType = UnityEngine.Random.Range(1, 3);
                    excuter.hasShieldStrong = (defendType == 1);
                    excuter.hasThorns = (defendType == 2);

                    string defendTypeText = excuter.hasShieldStrong ? "Strong Shield" : "Thorns";
                    Debug.Log($"🛡️ {actor} used DEFEND ({defendTypeText}).");
                    excuter.lastSkill = SkillType.Defend;
                    break;
                }

            case SkillType.Buff:
                {
                    if (excuter.lastSkill == SkillType.Buff) yield break;
                    excuter.hasBuff = true;
                    excuter.lastSkill = SkillType.Buff;
                    Debug.Log($"✨ {actor} used BUFF. Attack and heal boosted!");
                    break;
                }
        }

        excuter.HP = Mathf.Clamp(excuter.HP, 0, excuter.MaxHP);
        target.HP = Mathf.Clamp(target.HP, 0, target.MaxHP);

        StartCoroutine(UpdateHealthBar((target == player ? playerHealthBar : AIHealthBar), target));
        StartCoroutine(UpdateHealthBar((excuter == player ? playerHealthBar : AIHealthBar), excuter));

        yield return new WaitForSeconds(1.5f);
        if (player.HP <= 0 || AI.HP <= 0)
        {
            string winner = player.HP <= 0 ? "AI" : "Player";
            Debug.Log($"🏁 Game Over! {winner} Wins!");
            gameOver = true;
            yield break;
        }

        ChangePlayerTurn();
    }

    IEnumerator UpdateHealthBar(Transform targetHealthBar, Fighter target)
    {
        float currentRatio = targetHealthBar.localScale.x / baseHealthScale.x;
        float targetRatio = (float)target.HP / target.MaxHP;
        float duration = 0.5f;
        float t = 0;

        Vector3 startScale = targetHealthBar.localScale;
        Vector3 endScale = new Vector3(baseHealthScale.x * targetRatio, baseHealthScale.y, 1f);

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            targetHealthBar.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        targetHealthBar.localScale = endScale;
    }

    public void ChangePlayerTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        canPlayerUseSkill = false;

        if (!isPlayerTurn)
        {
            DisableSkillsPlayer();
            Debug.Log("⏳ Player’s turn ended. AI’s turn begins.");
        }
        else
        {
            EnableDicesPlayer(dices);
            DisableSkillsPlayer();
            canPlayerRoll = true;
            Debug.Log("🎯 Player’s turn begins!");
        }
    }

    void DisableDicesPlayer(Transform diceParent)
    {
        diceParent.GetChild(0).GetComponent<SpriteRenderer>().color = Color.gray;
        diceParent.GetChild(1).GetComponent<SpriteRenderer>().color = Color.gray;
    }

    void EnableDicesPlayer(Transform diceParent)
    {
        diceParent.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        diceParent.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
    }

    void DisablePlayerInteracble() => canPlayerInteract = false;
    void EnablePlayerInterable() => canPlayerInteract = true;

    public void DisableSkillsPlayer()
    {
        foreach (Transform skill in skills)
            skill.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public void EnableSkillsPlayer()
    {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.white };
        for (int i = 0; i < 4; i++)
            skills.GetChild(i).GetComponent<SpriteRenderer>().color = colors[i];

        canPlayerUseSkill = true;
    }

    public void DisableAllSkills()
    {
        foreach (Transform skill in skills)
            skill.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    int GetDicesTotal(Fighter figher) => figher.dicesValue.Item1 + figher.dicesValue.Item2;
    bool IsDoubleDicesValue(Fighter figher) => figher.dicesValue.Item1 == figher.dicesValue.Item2;

    public SkillType ChooseSkill(int skillId)
    {
        switch (skillId)
        {
            case 1: return SkillType.Attack;
            case 2: return SkillType.Heal;
            case 3: return SkillType.Defend;
            case 4: return SkillType.Buff;
            default: return SkillType.Attack;
        }
    }

    public string GetTagOnClick()
    {
        Collider2D c = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        return c == null ? null : c.tag;
    }
}
