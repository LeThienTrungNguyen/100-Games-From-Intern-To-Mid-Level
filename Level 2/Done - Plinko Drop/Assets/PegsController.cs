using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegsController : MonoBehaviour
{
    public Transform pegPrefab;
    public int rows;
    public int pegsStartRow;
    public float pegsSpacingX = 1.55f;
    public int pegsInRow;
    public Transform ground1;
    public Transform ground2;
    // Start is called before the first frame update
    void Start()
    {
        int type = Random.Range(0, 2);
        if(type == 0) GeneratePegsType1();
        else
        GeneratePegsType2();
    }

    void GeneratePegsType1()
    {
        // Tạo ra các peg theo dạng tam giác , hàng đầu tiên sẽ có pegsStartRow pegs , từ hàng tiếp theo sẽ có (pegstartRow + rows ) pegs
        // khoảng cách giữa các pegs trong cùng hàng là pegsSpacingX
        // khoảng cách giữa các hàng là pegsSpacingYMin và pegsSpacingYMax (random trong khoảng này)
        for (int i = 0; i < rows; i++)
        {
            int pegsInRow = pegsStartRow + i;
            float startX = -((pegsInRow - 1) * pegsSpacingX) / 2; // Tính toán vị trí bắt đầu để căn giữa hàng
            float posY = -i * 1.15f; // Vị trí Y của hàng hiện tại

            for (int j = 0; j < pegsInRow; j++)
            {
                float posX = startX + j * pegsSpacingX; // Vị trí X của peg hiện tại
                Instantiate(pegPrefab, new Vector3(posX, posY, 0), Quaternion.identity, transform);
            }
        }
        ground1.gameObject.SetActive(true);
        ground2.gameObject.SetActive(true);
    }

    void GeneratePegsType2()
    {
        // Tạo ra các peg theo dạng lưới , mỗi hàng có pegsStartRow pegs , khoảng cách giữa các pegs trong cùng hàng là pegsSpacingX
        // khoảng cách giữa các hàng là pegsSpacingYMin và pegsSpacingYMax (random trong khoảng này)
        // hàng lẻ sẽ lệch nửa khoảng cách pegsSpacingX so với hàng chẵn
        
        for (int i = 0; i < rows; i++)
        {
            float startX = -((pegsInRow - 1) * pegsSpacingX) / 2; // Tính toán vị trí bắt đầu để căn giữa hàng
            if (i % 2 != 0) // Nếu là hàng lẻ
            {
                startX += pegsSpacingX / 2; // Lệch nửa khoảng cách pegsSpacingX
            }
            float posY = -i * 1.15f; // Vị trí Y của hàng hiện tại

            for (int j = 0; j < pegsInRow; j++)
            {
                float posX = startX + j * pegsSpacingX; // Vị trí X của peg hiện tại
                Instantiate(pegPrefab, new Vector3(posX, posY, 0), Quaternion.identity, transform);
            }
        }
    }
}
