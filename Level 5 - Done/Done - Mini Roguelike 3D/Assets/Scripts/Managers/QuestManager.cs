using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [System.Serializable]
    public class QuestRequirement
    {
        public VoxelChunk.BlockType blockType;
        public int amountNeeded;
        public int currentAmount;
    }

    [System.Serializable]
    public class EmergencyQuest
    {
        public string questName;
        public string mailTitle;
        public string mailDescription;
        public List<QuestRequirement> requirements = new List<QuestRequirement>();
        public float rewardMoney;
        public ContractType targetContract;
    }

    public List<EmergencyQuest> officialQuestPool = new List<EmergencyQuest>();
    public List<EmergencyQuest> advancedQuestPool = new List<EmergencyQuest>();

    public EmergencyQuest activeQuest = null;
    private int daysUntilNextQuest = 0;
    public bool isQuestCompletedToday = false; 
    public bool isQuestSubmitted = false;
    private bool isQuestRunning = false;

    private void Awake()
    {
        Debug.Log("<color=yellow>[QuestManager] Hệ thống Nhiệm vụ đã khởi tạo thành công!</color>");
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeQuestPools();
    }

    private void InitializeQuestPools()
    {
        // --- 10 OFFICIAL QUESTS (Scale: Stone:100, Iron:30, Gold:20, Diamond:10) ---
        officialQuestPool.Add(new EmergencyQuest {
            questName = "Monument Restoration",
            mailTitle = "[TASK] City Square Restoration",
            mailDescription = "The central hero statue has been damaged by weather. We require high-quality materials to restore its base and gilding.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 100 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 20 }
            },
            rewardMoney = 1500f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Railway Expansion",
            mailTitle = "[URGENT] New Rail Tracks",
            mailDescription = "The Department of Transport is expanding the local rail network. We need a large shipment of iron to cast new tracks.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 50 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 15 } 
            },
            rewardMoney = 1200f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Public Housing Phase 1",
            mailTitle = "[CONTRACT] Affordable Housing Project",
            mailDescription = "The city council is building new apartments for workers. Basic construction materials are needed immediately.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 120 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 36 }
            },
            rewardMoney = 1800f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Museum Exhibition",
            mailTitle = "[INVITATION] Geological Wonders Display",
            mailDescription = "The City Museum is preparing a new exhibit. We need representative samples of all common and rare ores found in this region.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 40 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 12 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 8 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 4 }
            },
            rewardMoney = 2500f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Industrial Upgrade",
            mailTitle = "[TASK] Factory Modernization",
            mailDescription = "Local factories are upgrading their machinery. High-grade iron is essential for the new industrial components.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 60 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 18 } 
            },
            rewardMoney = 1400f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Mayor's Private Garden",
            mailTitle = "[PRIVATE] Ornamental Decorations",
            mailDescription = "The Mayor is redesigning his private estate. He has requested high-quality stones and golden accents for the garden path.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 80 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 16 }
            },
            rewardMoney = 2200f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Bridge Reinforcement",
            mailTitle = "[SAFETY] River Bridge Maintenance",
            mailDescription = "Structural inspectors found weaknesses in the main river bridge. We need materials for urgent reinforcement work.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 90 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 27 }
            },
            rewardMoney = 1600f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Bank Vault Security",
            mailTitle = "[SECRET] Vault Fortification",
            mailDescription = "The central bank is upgrading its security measures. Extra iron plating and golden decorative seals are required.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 30 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 20 }
            },
            rewardMoney = 3000f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "Educational Supplies",
            mailTitle = "[COMMUNITY] School Workshop Tools",
            mailDescription = "Local vocational schools need raw materials for their workshops to train the next generation of engineers.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 50 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 15 } 
            },
            rewardMoney = 1000f,
            targetContract = ContractType.Official
        });

        officialQuestPool.Add(new EmergencyQuest {
            questName = "City Fountain Plaza",
            mailTitle = "[PLAZA] Fountain Construction",
            mailDescription = "Construction of the grand centerpiece for the North Plaza has begun. We need stone for the base and a diamond for the water prism.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 150 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 15 }
            },
            rewardMoney = 2800f,
            targetContract = ContractType.Official
        });

        // --- 10 ADVANCED QUESTS (Scale: Stone:200, Iron:60, Gold:40, Diamond:20) ---
        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Deep Space Telescope",
            mailTitle = "[RESEARCH] Orbital Observatory Array",
            mailDescription = "NASA is requesting ultra-pure materials for the new deep space telescope mirrors and frame.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 40 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 20 }
            },
            rewardMoney = 15000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Experimental Fusion Reactor",
            mailTitle = "[ELITE] Advanced Energy Shielding",
            mailDescription = "The State Energy Commission needs rare materials for shielding a new experimental fusion core. Heat resistance is key.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 25 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 75 }
            },
            rewardMoney = 25000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Museum Heist Recovery",
            mailTitle = "[SECRET] Asset Liquidation",
            mailDescription = "Following a high-profile heist, certain 'investors' are looking to replenish their diamond and gold reserves discreetly.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 50 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 25 }
            },
            rewardMoney = 18000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "The Royal Crown Jewel",
            mailTitle = "[ROYAL] Coronation Preparations",
            mailDescription = "The crown jeweler requires a flawless diamond and high-purity gold to prepare for the upcoming Royal Coronation.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 30 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 60 }
            },
            rewardMoney = 35000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Quantum Supercomputer Array",
            mailTitle = "[TECH] Supercomputer Casings",
            mailDescription = "A leading tech giant needs gold-plated iron casings for their latest quantum computing hardware.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 80 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 120 }
            },
            rewardMoney = 22000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Fusion Core Housing",
            mailTitle = "[ENERGY] Extreme Environment Housing",
            mailDescription = "Constructing a housing unit for a portable fusion core requires the structural integrity of iron and the thermal properties of diamonds.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 180 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 30 }
            },
            rewardMoney = 30000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Secret Bunker Construction",
            mailTitle = "[CLASSIFIED] Secure Vault Project",
            mailDescription = "An anonymous client is building a fallout-proof bunker. We need immense amounts of stone, iron, and diamonds for the blast doors.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 400 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 120 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Diamond, amountNeeded = 20 }
            },
            rewardMoney = 40000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Satellite Array Upgrade",
            mailTitle = "[NETWORK] High-Bandwidth Transponders",
            mailDescription = "Communication satellites are being upgraded. We need 200 gold bars to plate the new high-bandwidth antenna arrays.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 300 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 60 } 
            },
            rewardMoney = 28000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Bio-Dome Structural Frame",
            mailTitle = "[ECOLOGY] Mars Prototype Dome",
            mailDescription = "Materials needed for the structural frame of a self-sustaining eco-dome project designed for harsh environments.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Iron, amountNeeded = 150 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 500 }
            },
            rewardMoney = 26000f,
            targetContract = ContractType.Advanced
        });

        advancedQuestPool.Add(new EmergencyQuest {
            questName = "Ancient Artifact Preservation",
            mailTitle = "[ARCHAEOLOGY] Ancient Seal Replication",
            mailDescription = "Scientists need to replicate an ancient seal to preserve a crumbling relic. Gold and specific stone types are required.",
            requirements = new List<QuestRequirement> { 
                new QuestRequirement { blockType = VoxelChunk.BlockType.Gold, amountNeeded = 100 },
                new QuestRequirement { blockType = VoxelChunk.BlockType.Stone, amountNeeded = 500 }
            },
            rewardMoney = 20000f,
            targetContract = ContractType.Advanced
        });
    }

    public void OnNewDayStarted()
    {
        // 1. Kiểm tra kết quả nhiệm vụ hôm qua (Nếu có)
        if (isQuestRunning && activeQuest != null && !isQuestSubmitted)
        {
            Debug.Log($"<color=red>[Quest] Nhiệm vụ '{activeQuest.questName}' đã hết hạn và thất bại.</color>");
            SendQuestResultMail(false);
        }

        // 2. Dọn dẹp trạng thái cũ
        activeQuest = null;
        isQuestCompletedToday = false;
        isQuestSubmitted = false;
        isQuestRunning = false;

        // 3. Xử lý đếm ngược hoặc giao nhiệm vụ mới
        if (daysUntilNextQuest <= 0)
        {
            TryGenerateQuest();
        }
        else
        {
            daysUntilNextQuest--;
            Debug.Log($"<color=white>[Quest] Hôm nay không có nhiệm vụ mới. Lượt tiếp theo sau {daysUntilNextQuest} ngày.</color>");
        }
    }

    private void TryGenerateQuest()
    {
        if (KPIManager.Instance == null) return;
        ContractType current = KPIManager.Instance.currentContract;

        if (current == ContractType.Official || current == ContractType.Advanced)
        {
            List<EmergencyQuest> pool = (current == ContractType.Official) ? officialQuestPool : advancedQuestPool;
            if (pool.Count > 0)
            {
                activeQuest = pool[Random.Range(0, pool.Count)];
                // Reset tiến độ
                foreach (var req in activeQuest.requirements) req.currentAmount = 0;
                
                daysUntilNextQuest = Random.Range(1, 4); 
                isQuestRunning = true;
                isQuestCompletedToday = false;
                isQuestSubmitted = false;
                
                Debug.Log($"<color=cyan>[Quest] NHIỆM VỤ MỚI: {activeQuest.questName}. Lượt tiếp theo sau ít nhất {daysUntilNextQuest} ngày nghỉ.</color>");
                SendQuestAnnounceMail();
            }
        }
        else
        {
            Debug.Log("<color=white>[Quest] Hôm nay không có nhiệm vụ mới (Hợp đồng hiện tại: Illegal).</color>");
        }
    }

    private string GetColoredBlockName(VoxelChunk.BlockType type)
    {
        switch (type)
        {
            case VoxelChunk.BlockType.Stone: return "<color=#AAAAAA>Stone</color>";
            case VoxelChunk.BlockType.Iron: return "<color=#CCCCCC>Iron</color>";
            case VoxelChunk.BlockType.Gold: return "<color=#FFD700>Gold</color>";
            case VoxelChunk.BlockType.Diamond: return "<color=#00FFFF>Diamond</color>";
            default: return type.ToString();
        }
    }

    private void SendQuestAnnounceMail()
    {
        if (Mailbox.Instance == null || activeQuest == null) return;
        
        string detail = activeQuest.mailDescription + "\n\n<b>RESOURCES NEEDED:</b>\n";
        foreach(var r in activeQuest.requirements) 
        {
            detail += $"{GetColoredBlockName(r.blockType)}: {r.currentAmount} / {r.amountNeeded}\n";
        }
        
        Mailbox.Instance.ReceiveNewMail(MailType.QuestNew, activeQuest.mailTitle, detail);
    }

    private void SendQuestResultMail(bool success)
    {
        if (Mailbox.Instance == null || activeQuest == null) return;

        if (success)
        {
            Debug.Log($"<color=green>[Quest] HOÀN THÀNH: {activeQuest.questName}. Nhận thưởng {activeQuest.rewardMoney}$</color>");
            
            // CỘNG TIỀN THƯỞNG CHO PLAYER
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.AddMoney(activeQuest.rewardMoney);
            }

            string successMsg = $"Congratulations! You have completed the task: {activeQuest.questName}.\n\nReward: ${activeQuest.rewardMoney} has been added to your account.";
            Mailbox.Instance.ReceiveNewMail(MailType.QuestSuccess, "Task Completed!", successMsg);
        }
        else
        {
            string failMsg = $"The deadline for the task '{activeQuest.questName}' has passed. You failed to provide the required resources on time.";
            Mailbox.Instance.ReceiveNewMail(MailType.QuestFailed, "Task Failed", failMsg);
        }
    }

    public bool TryAddQuestItem(VoxelChunk.BlockType type)
    {
        if (activeQuest == null || isQuestCompletedToday || isQuestSubmitted) return false;

        QuestRequirement req = activeQuest.requirements.Find(r => r.blockType == type && r.currentAmount < r.amountNeeded);
        if (req != null)
        {
            req.currentAmount++;
            Debug.Log($"<color=orange>[Quest Progress] {type}: {req.currentAmount}/{req.amountNeeded}</color>");

            CheckQuestCompletion();
            UpdateQuestMailProgress();
            return true; 
        }

        return false;
    }

    public void SubmitActiveQuest()
    {
        if (activeQuest == null || !isQuestCompletedToday || isQuestSubmitted) return;

        isQuestSubmitted = true;
        isQuestRunning = false;

        // TRỪ VẬT PHẨM KHỎI INVENTORY
        if (InventoryManager.Instance != null)
        {
            foreach (var req in activeQuest.requirements)
            {
                InventoryManager.Instance.SubtractItem(req.blockType, req.amountNeeded);
            }
        }

        SendQuestResultMail(true);
        UpdateQuestMailProgress();
        Debug.Log("<color=green>[Quest] ĐÃ NỘP KHOÁNG SẢN VÀ NHẬN THƯỞNG!</color>");
    }

    private void UpdateQuestMailProgress()
    {
        if (Mailbox.Instance == null || activeQuest == null) return;

        string statusHeader = "";
        if (isQuestSubmitted) statusHeader = "<color=green>(Submitted)</color>\n\n";
        else if (isQuestCompletedToday) statusHeader = "<color=yellow>(Completed, Do you want to submit the ores? (Y/N))</color>\n\n";

        string detail = activeQuest.mailDescription + "\n\n" + statusHeader + "<b>RESOURCES NEEDED:</b>\n";
        foreach (var r in activeQuest.requirements)
        {
            detail += $"{GetColoredBlockName(r.blockType)}: {r.currentAmount} / {r.amountNeeded}\n";
        }

        Mailbox.Instance.UpdateMailContent(MailType.QuestNew, activeQuest.mailTitle, detail);
    }

    private void CheckQuestCompletion()
    {
        if (isQuestSubmitted) return;

        bool allDone = true;
        foreach (var req in activeQuest.requirements)
        {
            if (req.currentAmount < req.amountNeeded)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            isQuestCompletedToday = true;
            Debug.Log("<color=green>[Quest] TẤT CẢ YÊU CẦU ĐÃ XONG! Đang chờ người chơi nộp...</color>");
        }
    }
}
