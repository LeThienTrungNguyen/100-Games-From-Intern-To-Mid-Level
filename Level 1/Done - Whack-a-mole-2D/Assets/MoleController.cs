using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MoleController : MonoBehaviour
{
    public List<Transform> moles;
    public bool isShowUp = false;
    public float timerGoDown = 0.5f;
    public Transform mole;
    int moleHp = 1;
    private float moveProgress = 0f;
    private float moveSpeed = 3f; // càng lớn, chuyển động càng nhanh

    private Vector3 upPos = new Vector3(0, 1.13f, 0);
    private Vector3 downPos = new Vector3(0, -1.12f, 0);

    void Awake()
    {
        DeactiveMoles();
        mole = GetRandomMoles();
        moleHp = 2;
        mole.gameObject.SetActive(true);
        mole.transform.parent.localPosition = downPos; // bắt đầu ở dưới
        ScoreController.instance.highScoreText.text = PlayerPrefs.GetInt("High Score")+"";
    }

    void Update()
    {
        if (mole == null) return;

        if (isShowUp)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            moveProgress = Mathf.Clamp01(moveProgress);

            // Chỉ bắt đầu đếm giờ xuống khi đã lên đủ
            if (moveProgress >= 1f)
            {
                timerGoDown -= Time.deltaTime;
                if (timerGoDown <= 0f)
                {
                    isShowUp = false;
                }
            }
        }
        else
        {
            moveProgress -= Time.deltaTime * moveSpeed;
            moveProgress = Mathf.Clamp01(moveProgress);

            // Khi xuống hết → chọn chuột mới
            if (moveProgress <= 0f)
            {
                timerGoDown = 1.5f;

                DeactiveMoles();
                mole = GetRandomMoles();
                moleHp = 2;
                mole.gameObject.SetActive(true);
                mole.transform.parent.localPosition = downPos;
                moveProgress = 0f;
            }
        }

        mole.transform.parent.localPosition = Vector3.Lerp(downPos, upPos, moveProgress);
    }


    Transform GetRandomMoles()
    {
        int r = Random.Range(0, moles.Count);
        return moles[r];
    }

    void DeactiveMoles()
    {
        foreach (Transform t in moles)
        {
            t.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("hit :" + mole, mole);
        if (mole.name.Equals("Mole"))
        {
            var animr = GetComponentInChildren<Animator>();
            animr.Play("Mole Hit");
            ScoreController.instance.AddScore(1);
        }
        else if (mole.name.Equals("MoleHat"))
        {
            var animr = GetComponentInChildren<Animator>();
            if (moleHp == 2)
            {
                animr.Play("MoleHatBreak");
                moleHp--;
            }
            else
            {
                animr.Play("MoleHatHit");
                ScoreController.instance.AddScore(2);
            }

        }
        else
        {
            if (PlayerPrefs.HasKey("High Score"))
            {
                int highScore = PlayerPrefs.GetInt("High Score");
                if (ScoreController.score > highScore)
                {
                    highScore = ScoreController.score;
                    PlayerPrefs.SetInt("High Score", highScore);
                }
            }
            else
            {
                PlayerPrefs.SetInt("High Score", ScoreController.score);
            }
            ScoreController.instance.highScoreText.text = PlayerPrefs.GetInt("High Score") + "";
            ScoreController.instance.ResetScore();
        }
    }
}
