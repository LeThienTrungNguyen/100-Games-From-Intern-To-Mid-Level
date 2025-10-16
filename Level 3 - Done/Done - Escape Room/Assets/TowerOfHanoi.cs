using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TowerOfHanoi : MonoBehaviour
{
    public bool isReady;
    public Pillar[] pillars;
    public Piece[] pieces;
    public float[] yPos;
    public Piece choosenPiece;
    public Camera cameraTower;
    void Update()
    {
        if (!isReady) return;
        cameraTower.gameObject.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraTower.gameObject.SetActive(false);
            isReady = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            Ray ray = cameraTower.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f)) // 100f = khoảng cách tối đa
            {
                // Kiểm tra xem có phải Pillar không
                Pillar pillar = hit.collider.GetComponent<Pillar>();
                if (pillar != null)
                {
                    Debug.Log("Bạn vừa click vào: " + pillar.name);
                    // Xử lý logic tại đây
                    if (choosenPiece != null)
                    {
                        Debug.Log("Child count :" + pillar.transform.childCount, pillar);
                        Debug.Log("choosenPiece value:" + choosenPiece.value, choosenPiece);
                        if (pillar.transform.childCount == 0 || choosenPiece.value < pillar.LastPiece().value)
                        {
                            PickDownPiece(choosenPiece, pillar);
                            Debug.Log("lastpiece value:" + pillar.LastPiece().value, pillar);
                            choosenPiece = null;
                        }

                    }
                    else
                    {
                        choosenPiece = pillar.transform.GetChild(pillar.transform.childCount - 1).GetComponent<Piece>();
                        PickupPiece(choosenPiece);
                        choosenPiece.transform.parent = null;
                    }

                }
            }
        }
    }

    public void PickupPiece(Piece piece)
    {
        piece.transform.localPosition = new Vector3(piece.transform.localPosition.x, 1.2f, piece.transform.localPosition.z);

    }
    public void PickDownPiece(Piece piece, Pillar pillarParent)
    {
        int index = pillarParent.transform.childCount;
        Debug.Log(index);
        piece.transform.parent = pillarParent.transform;
        piece.transform.localPosition = new Vector3(0, yPos[index], 0);
        CheckWin();
    }
    void CheckWin()
    {
        if (Check9PiecesInPillar3())
        {
            Win();
        }
    }
    void Win() { Destroy(gameObject); Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;}
    bool Check9PiecesInPillar3()
    {
        return pillars[2].transform.childCount == 9;
    }
}
