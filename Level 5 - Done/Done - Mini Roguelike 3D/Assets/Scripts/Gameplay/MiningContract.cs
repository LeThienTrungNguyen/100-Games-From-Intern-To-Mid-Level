public enum ContractType { Illegal, Official, Advanced }

[System.Serializable]
public class MiningContract
{
    public ContractType type;
    public string contractName;
    public int kpiTarget;      // Mục tiêu số block tối thiểu
    public float baseSalary;   // Lương cứng nếu đạt KPI
    public float bonusPerBlock; // Thưởng thêm mỗi block vượt mức
}