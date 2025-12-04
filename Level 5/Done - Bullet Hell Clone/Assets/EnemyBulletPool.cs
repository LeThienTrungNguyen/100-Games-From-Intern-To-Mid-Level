using System.Collections.Generic;
using UnityEngine;
public enum BulletMoveType
{
    Type1, // move down
    Type2,
    Type3,
    Type4

}
public class EnemyBulletPool : MonoBehaviour
{
    public static EnemyBulletPool Instance;

    [SerializeField] private List<GameObject> pool = new List<GameObject>(); // include inactive enemy bullet

    void Awake()
    {
        Instance = this;
    }
    [ContextMenu("Spawn Enemy Bullet")]
    public void SpawnBullet(EnemyBulletController bulletController, Vector3 position, Quaternion rotation, Transform parent, BulletMoveType type = BulletMoveType.Type1)
    {
        Transform bulletPrefab;

        if (TryGetBullet(bulletController, out bulletPrefab))
        {
            pool.Remove(bulletPrefab.gameObject);
        }
        else
        {
            bulletPrefab = Instantiate(bulletController, transform).transform;
        }

        bulletPrefab.position = position;
        bulletPrefab.GetComponent<EnemyBulletController>().parent = parent;

        // 🔹 Nếu là Type4 thì tự xoay về player tại thời điểm spawn
        if (type == BulletMoveType.Type4)
        {
            var type4 = bulletPrefab.GetComponent<EnemyBulletControllerType4>();
            if (type4 != null)
            {
                type4.SetupDirection(FindAnyObjectByType<PlayerController>());
            }
        }
        else
        {
            bulletPrefab.rotation = rotation;
        }

        // 🔹 Kích hoạt sau cùng
        bulletPrefab.gameObject.SetActive(true);
    }


    public void DestroyBullet(Transform bullet) // return bullet to pool
    {
        pool.Add(bullet.gameObject);
        bullet.gameObject.SetActive(false);
    }

    public bool ContainBulletController(EnemyBulletController bulletController)
    {
        return FindBulletTransform(bulletController) != null;
    }
    public bool TryGetBullet(EnemyBulletController bulletController, out Transform bulletPrefab)
    {
        // tìm transform (không remove trong helper)
        Transform found = FindBulletTransform(bulletController);
        if (found != null)
        {
            bulletPrefab = found;
            // bây giờ remove (chỉ remove 1 lần)
            pool.Remove(found.gameObject);
            return true;
        }

        bulletPrefab = null;
        return false;
    }
    private Transform FindBulletTransform(EnemyBulletController bulletController)
    {
        // nếu đối số đầu vào null => không thể tìm type
        if (bulletController == null)
        {
            Debug.LogError("FindBulletTransform() nhận vào bulletController = null");
            return null;
        }

        // duyệt toàn bộ pool
        foreach (var go in pool)
        {
            // nếu phần tử bị destroy hoặc null => bỏ qua
            if (go == null) continue;

            var controller = go.GetComponent<EnemyBulletController>();
            if (controller == null) continue;

            // so sánh type
            if (controller.GetType() == bulletController.GetType())
            {
                return go.transform;
            }
        }

        // không tìm thấy
        return null;
    }

}
