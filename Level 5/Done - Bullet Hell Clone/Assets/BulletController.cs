using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;
    public int damage;
    private float lifeTime = 20f;
    private float timer;
    PlayerController pc;
    void OnEnable()
    {
        timer = 0f;
        pc = (FindAnyObjectByType(typeof(PlayerController)) as PlayerController);
        
    }

    void Update()
    {
        damage = pc.playerStats.bulletDamage;
        speed = pc.playerStats.bulletSpeed;
        // Di chuyển theo hướng forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Hết thời gian sống thì tắt
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
            timer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            var boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                pc.money += 1* pc.playerStats.moneyMultiplier;
                pc.UpdateShopUI();
                pc.UpdateMoneyUI();
            }

            gameObject.SetActive(false);
        }
    }
}
