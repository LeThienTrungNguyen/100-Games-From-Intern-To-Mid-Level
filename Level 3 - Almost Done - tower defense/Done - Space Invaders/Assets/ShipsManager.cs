using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipsManager : MonoBehaviour
{
    public Transform shipPrefab;
    public Transform bulletPrefab;   // 🔴 prefab đạn
    public Vector2 startPos = new Vector2(-3.375f, 0);
    public int width, height;
    public float offset = 1f, moveSpace = 0.5f, boundXMin = -7.75f, boundXMax = 7.75f, speed = 2f;
    Vector2 direction = Vector2.right;

    public RectTransform panel;

    void Start()
    {
        GenerateShips();
        InvokeRepeating(nameof(MoveHorizontal), 1f, 1f / speed);
        InvokeRepeating(nameof(SpawnBullet), 2f, 2f); // 🔴 spawn đạn mỗi 2s
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Restart();
    }

    void MoveHorizontal()
    {
        Transform edgeShip = GetEdgeShip(direction.x > 0);
        if (edgeShip == null)
        {
            transform.position = startPos;
            CancelInvoke(nameof(MoveHorizontal));
            GenerateShips();
            speed++;
            InvokeRepeating(nameof(MoveHorizontal), 1f, 1f / speed);
            return;
        }

        bool hitRight = direction.x > 0 && edgeShip.position.x + moveSpace > boundXMax;
        bool hitLeft = direction.x < 0 && edgeShip.position.x - moveSpace < boundXMin;

        if (hitRight || hitLeft)
        {
            direction *= -1;
            transform.position += Vector3.down * moveSpace;
        }
        else
        {
            transform.position += (Vector3)direction * moveSpace;
        }

        Transform lowestShip = GetLowestShip();
        if (lowestShip != null && lowestShip.position.y <= -4.75f)
        {
            CancelInvoke(nameof(MoveHorizontal));
            GameOver();
        }
    }

    void SpawnBullet()
    {
        if (transform.childCount == 0) return;

        // 🔴 chọn ngẫu nhiên 1 con ship để bắn
        int randIndex = Random.Range(0, transform.childCount);
        Transform shooter = transform.GetChild(randIndex);

        if (bulletPrefab != null)
        {
            var bullet = Instantiate(bulletPrefab, shooter.position, Quaternion.identity);
            bullet.tag = "Enemy";
        }
    }

    Transform GetEdgeShip(bool max)
    {
        if (transform.childCount == 0) return null;
        Transform edge = transform.GetChild(0);
        foreach (Transform ship in transform)
        {
            if ((max && ship.position.x > edge.position.x) || (!max && ship.position.x < edge.position.x))
                edge = ship;
        }
        return edge;
    }

    Transform GetLowestShip()
    {
        if (transform.childCount == 0) return null;
        Transform lowest = transform.GetChild(0);
        foreach (Transform ship in transform)
            if (ship.position.y < lowest.position.y)
                lowest = ship;
        return lowest;
    }

    void GenerateShips()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                Instantiate(shipPrefab, transform.position + new Vector3(i * offset, j * offset, 0), Quaternion.identity, transform);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        panel.gameObject.SetActive(true);
    }

    void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
