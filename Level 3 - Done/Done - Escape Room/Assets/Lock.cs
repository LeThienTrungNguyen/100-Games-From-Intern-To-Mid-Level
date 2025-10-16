using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    private string password = "12809";
    public Transform pipe;
    public Transform wheel0;
    public Transform wheel1;
    public Transform wheel2;
    public Transform wheel3;
    public Transform wheel4;
    public bool isUnlock;
    public Camera cameraLock;
    public bool isReady;
    void Update()
    {
        if (isReady)
        {
            cameraLock.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraLock.gameObject.SetActive(false);
            isReady = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            transform.root.GetComponent<Collider>().enabled = true;
        }
       if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            Ray ray = cameraLock.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f)) // 100f = khoảng cách tối đa
            {
                // Kiểm tra xem có phải Pillar không
                Wheel weel = hit.collider.GetComponent<Wheel>();
                if (weel != null)
                {
                    Debug.Log("Bạn vừa click vào: " + weel.name);
                    // Xử lý logic tại đây
                    weel.Rotate();
                }
            }
        }
    }

    public bool IsUnlock()
    {
        var wheel0 = GetWheel(this.wheel0).number;
        var char0 = GetPasswordIndex(0);

        var wheel1 = GetWheel(this.wheel1).number;
        var char1 = GetPasswordIndex(1);

        var wheel2 = GetWheel(this.wheel2).number;
        var char2 = GetPasswordIndex(2);


        var wheel3 = GetWheel(this.wheel3).number;
        var char3 = GetPasswordIndex(3);


        var wheel4 = GetWheel(this.wheel4).number;
        var char4 = GetPasswordIndex(4);

        var correct0 = wheel0 == int.Parse(char0.ToString());

        var correct1 = wheel1 == int.Parse(char1.ToString());

        var correct2 = wheel2 == int.Parse(char2.ToString());

        var correct3 = wheel3 == int.Parse(char3.ToString());

        var correct4 = wheel4 == int.Parse(char4.ToString());

        return correct0 && correct1 && correct2 && correct3 && correct4;
    }
    Wheel GetWheel(Transform t)
    {
        return t.GetComponent<Wheel>();
    }
    char GetPasswordIndex(int index)
    {
        return password[index];
    }

    public void CheckLock()
    {
        var isUnlock = IsUnlock();
        if (isUnlock)
        {
            pipe.transform.localPosition += pipe.up * 0.03f;
            Destroy(transform.root.gameObject, 2f);
            PlayerController playerController = GameObject.FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
            playerController.canMove = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
