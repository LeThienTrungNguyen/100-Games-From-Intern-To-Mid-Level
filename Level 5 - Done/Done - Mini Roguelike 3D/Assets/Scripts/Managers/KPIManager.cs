using System.Collections;
using UnityEngine;

public class KPIManager : MonoBehaviour
{
    public static KPIManager Instance;

    // Đã xóa enum ContractType ở đây vì đã có trong MiningContract.cs

    [System.Serializable]
    public class ContractSettings
    {
        public string label;
        public int kpiTarget;
        public float baseSalary;
        public float bonusPerBlock;
    }

    [Header("Contract Configurations")]
    public ContractSettings illegalConfig = new ContractSettings { label = "Illegal", kpiTarget = 30, baseSalary = 120f, bonusPerBlock = 5f };
    public ContractSettings officialConfig = new ContractSettings { label = "Official", kpiTarget = 300, baseSalary = 1500f, bonusPerBlock = 15f };
    public ContractSettings advancedConfig = new ContractSettings { label = "Advanced", kpiTarget = 1500, baseSalary = 10000f, bonusPerBlock = 50f };

    [Header("Contract Logic")]
    public ContractType currentContract = ContractType.Illegal;
    public ContractType pendingContract = ContractType.Illegal;

    [Header("End Day Trigger")]
    public EndDayTrigger endDayTrigger;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CheckKPIAchievement()
    {
        if (InventoryManager.Instance == null) return;

        if (endDayTrigger != null && InventoryManager.Instance.totalBlocks >= GetCurrentConfig().kpiTarget)
        {
            if (!endDayTrigger.gameObject.activeSelf)
            {
                Debug.Log("<color=yellow>KPI đã đạt! End Day Trigger đã xuất hiện.</color>");
                endDayTrigger.SetActiveTrigger(true);
            }
        }
    }

    public bool isCalculating = false;

    [ContextMenu("End Day")]
    public void EndDay()
    {
        // 1. Chuẩn bị đơn hàng cho ngày mai
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.PrepareDeliveryForNextDay();
        }

        // 2. Kích hoạt các cầu thang đang chờ xử lý (Giữ lại logic này vì thang được đặt trong thế giới)
        LadderController[] allLadders = FindObjectsOfType<LadderController>();
        foreach (var ladder in allLadders)
        {
            if (ladder.isPending) ladder.Activate();
        }

        // Ẩn trigger cho ngày mới
        if (endDayTrigger != null) endDayTrigger.SetActiveTrigger(false);

        // Reset UI Kho đồ (Inventory)
        if (InventoryManager.Instance != null) InventoryManager.Instance.ClearInventory();

        if (TeleportSystem.Instance != null) TeleportSystem.Instance.ExecuteTeleport();
        if (TimeManager.Instance != null) TimeManager.Instance.NextDay();
        if (TeleportSystem.Instance != null) TeleportSystem.Instance.ResetTeleportDaily();
    }

    public void ShowSalaryReport(System.Action onComplete)
    {
        StartCoroutine(Co_FullSalarySequence(onComplete));
    }

    private IEnumerator Co_FullSalarySequence(System.Action onComplete)
    {
        isCalculating = true;

        // Reset các text số liệu cũ về trống/mặc định trước khi chạy animation mới
        ResetSalaryUI();

        yield return StartCoroutine(Co_SetupUI());

        if (InventoryManager.Instance != null)
        {
            yield return StartCoroutine(Co_AnimateOreCounts(InventoryManager.Instance));
            yield return StartCoroutine(Co_AnimateTotalOres(InventoryManager.Instance));

            int exceededValue = InventoryManager.Instance.totalBlocks - GetCurrentConfig().kpiTarget;
            yield return StartCoroutine(Co_AnimateKPIResult(exceededValue, InventoryManager.Instance.totalBlocks));
            yield return StartCoroutine(Co_AnimateSalaryDetails(exceededValue, InventoryManager.Instance.totalBlocks));
        }
        
        isCalculating = false;
        
        // Hiện nút đóng
        UIManager.Instance.OpenPanel(UIManager.Instance.CloseSalarayReportPanelBtn);

        // Đợi người chơi đóng hoặc tự động đóng sau 10s
        float timer = 0f;
        bool closedManually = false;
        
        // Đăng ký sự kiện nút đóng (giả định bạn gán hàm này cho Button trong Unity)
        // Ở đây tôi dùng biến flag để kiểm tra
        while (timer < 10f)
        {
            if (!UIManager.Instance.SalarayReportPanel.gameObject.activeSelf) 
            {
                closedManually = true;
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (!closedManually)
        {
            // Tự động đóng nếu hết 10s
            CloseSalaryReportUI();
        }

        // Đợi animation đóng hoàn tất (0.2s theo ContractShop)
        yield return new WaitForSeconds(0.3f);
        
        onComplete?.Invoke();
    }

    public void CloseSalaryReportUI()
    {
        if (UIManager.Instance.SalarayReportPanel.gameObject.activeSelf)
        {
            DotweenAnimationName.Instance.DoScaleDown(UIManager.Instance.SalarayReportPanel.transform, 0, 0.2f, true);
            UIManager.Instance.SetUIState(false);
            UIManager.Instance.ClosePanel(UIManager.Instance.CloseSalarayReportPanelBtn);
        }
    }

    private void ResetSalaryUI()
    {
        // Ẩn nút Close khi bắt đầu ca làm/tính lương mới
        if (UIManager.Instance.CloseSalarayReportPanelBtn != null)
            UIManager.Instance.CloseSalarayReportPanelBtn.gameObject.SetActive(false);

        // Giữ nguyên nhãn, xóa giá trị số
        UIManager.Instance.stoneTxt.text = "<color=#808080>Stone: </color>";
        UIManager.Instance.goalTxt.text = "<color=#A52A2A>Goal: </color>";
        UIManager.Instance.ironTxt.text = "<color=#FFFFFF>Iron: </color>";
        UIManager.Instance.goldTxt.text = "<color=#FFD700>Gold: </color>";
        UIManager.Instance.diamondTxt.text = "<color=#00BFFF>Diamond: </color>";
        UIManager.Instance.totalCountTxt.text = "Total Ores: ";
        UIManager.Instance.kpiTxt.text = "KPI: ";
        UIManager.Instance.ExceededTxt.text = "Exceeded: ";
        UIManager.Instance.salaryDayTxt.text = "Salary Day: ";
        UIManager.Instance.salaryDayBonusTxt.text = "Salary Bonus: ";
        UIManager.Instance.TotalReceivedTxt.text = "Total Received: ";
    }

    private IEnumerator Co_SetupUI()
    {
        UIManager.Instance.OpenPanel(UIManager.Instance.SalarayReportPanel);
        
        // Cập nhật tiêu đề bảng lương theo ngày
        if (UIManager.Instance.salaryReportTitleTxt != null && TimeManager.Instance != null)
        {
            UIManager.Instance.salaryReportTitleTxt.text = $"Salary Day {TimeManager.Instance.currentDay}";
        }

        DotweenAnimationName.Instance.DoScaleUp(UIManager.Instance.SalarayReportPanel.gameObject.transform, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.kpiTxt.text = $"KPI: {GetCurrentConfig().kpiTarget}";
    }

    private IEnumerator Co_AnimateOreCounts(InventoryManager inv)
    {
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.stoneTxt, "<color=#808080>Stone: ", inv.countStone, "</color>"));
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.goalTxt, "<color=#A52A2A>Goal: ", inv.countGoal, "</color>"));
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.ironTxt, "<color=#FFFFFF>Iron: ", inv.countIron, "</color>"));
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.goldTxt, "<color=#FFD700>Gold: ", inv.countGold, "</color>"));
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.diamondTxt, "<color=#00BFFF>Diamond: ", inv.countDiamond, "</color>"));
    }

    private IEnumerator Co_AnimateTotalOres(InventoryManager inv)
    {
        string prefix = "Total Ores: ";
        string formula = $"<color=#808080>{inv.countStone}</color>+<color=#A52A2A>{inv.countGoal}</color>+<color=#FFFFFF>{inv.countIron}</color>+<color=#FFD700>{inv.countGold}</color>+<color=#00BFFF>{inv.countDiamond}</color>=  ";
        
        UIManager.Instance.totalCountTxt.text = prefix + formula;
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.totalCountTxt, prefix + formula, inv.totalBlocks, "</color>"));
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.totalCountTxt.text = $"{prefix}{inv.totalBlocks}";
    }

    private IEnumerator Co_AnimateKPIResult(int exceededValue, int totalBlocks)
    {
        string prefix = "Exceeded: ";
        int target = GetCurrentConfig().kpiTarget;

        UIManager.Instance.ExceededTxt.text = $"{prefix}{totalBlocks}"; yield return new WaitForSeconds(0.5f);
        UIManager.Instance.ExceededTxt.text = $"{prefix}{totalBlocks}-{target}"; yield return new WaitForSeconds(0.5f);
        UIManager.Instance.ExceededTxt.text = $"{prefix}{totalBlocks}-{target} = {exceededValue}"; yield return new WaitForSeconds(0.5f);

        string colorTag = exceededValue >= 0 ? "#00FF00" : "#FF0000";
        int finalExceeded = Mathf.Max(0, exceededValue);
        
        UIManager.Instance.ExceededTxt.text = $"{prefix}<color={colorTag}>{finalExceeded}</color>";
        UIManager.Instance.totalCountTxt.text = $"Total Ores: <color={colorTag}>{totalBlocks}</color>";
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator Co_AnimateSalaryDetails(int exceededValue, int totalBlocks)
    {
        ContractSettings config = GetCurrentConfig();
        int finalExceeded = Mathf.Max(0, exceededValue);
        float salaryBase = (totalBlocks >= config.kpiTarget) ? config.baseSalary : 0;
        float salaryBonus = finalExceeded * config.bonusPerBlock;

        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.salaryDayTxt, "Salary Day: <color=#000000>", salaryBase, "$</color>"));

        string bonusPrefix = "Salary Bonus: ";
        UIManager.Instance.salaryDayBonusTxt.text = $"{bonusPrefix}{finalExceeded}"; yield return new WaitForSeconds(0.3f);
        UIManager.Instance.salaryDayBonusTxt.text = $"{bonusPrefix}{finalExceeded} x {config.bonusPerBlock}"; yield return new WaitForSeconds(0.3f);
        
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.salaryDayBonusTxt, $"{bonusPrefix}{finalExceeded} x {config.bonusPerBlock} = ", salaryBonus, "$"));
        UIManager.Instance.salaryDayBonusTxt.text = $"{bonusPrefix}{salaryBonus}$";

        float total = salaryBase + salaryBonus;
        string totalPrefix = "Total Received: ";
        UIManager.Instance.TotalReceivedTxt.text = $"{totalPrefix}{salaryBase} + {salaryBonus}"; yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(DotweenAnimationName.Instance.Co_AnimateNumberByStep(UIManager.Instance.TotalReceivedTxt, $"{totalPrefix}{salaryBase} + {salaryBonus} = ", total, "$"));
        UIManager.Instance.TotalReceivedTxt.text = $"{totalPrefix}{total}$";

        // Cộng tiền vào tài khoản người chơi sau khi tính toán xong
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddMoney(total);
        }

        yield return new WaitForSeconds(0.5f);
    }

    void HandleContractUpgrade()
    {
        // 1. Kiểm tra nâng cấp hợp đồng (Shop)
        if (ContractShop.Instance.hasPendingContractRequest)
        {
            currentContract = pendingContract;
            ContractShop.Instance.hasPendingContractRequest = false;

            // REFRESH STATS KHI ĐỔI HỢP ĐỒNG
            if (PlayerStats.Instance != null) PlayerStats.Instance.RefreshDynamicStats();

            if (Mailbox.Instance != null)
            {
                if (currentContract == ContractType.Official)
                    Mailbox.Instance.ReceiveNewMail(MailType.ToOfficial);
                else if (currentContract == ContractType.Advanced)
                    Mailbox.Instance.ReceiveNewMail(MailType.ToAdvanced);
            }
        }
        else
        {
            // 2. Kiểm tra nếu hết sạch quặng trong thế giới
            if (VoxelChunk.TotalMineableBlocksCount <= 0)
            {
                if (Mailbox.Instance != null)
                {
                    Mailbox.Instance.ReceiveNewMail(MailType.NoMoreOres);
                }
            }
            // 3. Nếu không có nâng cấp và vẫn còn quặng, kiểm tra xem có đạt KPI không
            else if (InventoryManager.Instance != null && InventoryManager.Instance.totalBlocks < GetCurrentConfig().kpiTarget)
            {
                if (Mailbox.Instance != null)
                {
                    Mailbox.Instance.ReceiveNewMail(MailType.KPIFailed);
                }
            }
        }
    }

    public ContractSettings GetCurrentConfig()
    {
        // Đảm bảo các nhãn luôn là tiếng Anh đúng chuẩn
        if (illegalConfig != null) illegalConfig.label = "Illegal";
        if (officialConfig != null) officialConfig.label = "Official";
        if (advancedConfig != null) advancedConfig.label = "Advanced";

        ContractSettings config = currentContract switch
        {
            ContractType.Official => officialConfig,
            ContractType.Advanced => advancedConfig,
            _ => illegalConfig,
        };

        // Fallback để tránh null lỗi
        return config ?? illegalConfig ?? new ContractSettings { label = "Illegal", kpiTarget = 30 };
    }
    }