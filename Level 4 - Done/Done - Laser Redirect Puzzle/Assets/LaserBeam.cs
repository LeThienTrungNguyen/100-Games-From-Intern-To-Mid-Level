using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserBeam
{
    Vector3 pos, dir;
    GameObject laserObj;
    LineRenderer laser;
    List<Vector3> laserIndices = new List<Vector3>();

    int maxBounce = 20; // giới hạn phản xạ
    int currentBounce = 0;

    public LaserBeam(Vector3 pos, Vector3 dir, Material material)
    {
        this.laserObj = new GameObject("Laser Beam");
        this.laser = this.laserObj.AddComponent<LineRenderer>();

        this.laser.startWidth = 0.1f;
        this.laser.endWidth = 0.1f;
        this.laser.material = material;
        this.laser.startColor = Color.green;
        this.laser.endColor = Color.green;

        this.pos = pos;
        this.dir = dir;

        CastRay(pos, dir, laser, 0); // truyền số lần phản xạ ban đầu = 0
    }

    void CastRay(Vector2 pos, Vector2 dir, LineRenderer laser, int bounce)
    {
        laserIndices.Add(pos);

        if (bounce > maxBounce)
        {
            laserIndices.Add(pos + dir.normalized * 30f);
            UpdateLaser();
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 30f);

        if (hit.collider != null)
        {
            CheckHit(hit, dir, laser, bounce);
        }
        else
        {
            laserIndices.Add(pos + dir.normalized * 30f);
            UpdateLaser();
        }
    }

    void UpdateLaser()
    {
        laser.positionCount = laserIndices.Count;
        for (int i = 0; i < laserIndices.Count; i++)
            laser.SetPosition(i, laserIndices[i]);
    }

    void CheckHit(RaycastHit2D hitInfo, Vector2 direction, LineRenderer laser, int bounce)
    {
        if (hitInfo.collider == null)
            return;

        if (hitInfo.collider.CompareTag("Mirror"))
        {
            Vector2 pos = hitInfo.point;
            Vector2 dir = Vector2.Reflect(direction, hitInfo.normal);
            CastRay(pos + dir.normalized * 0.01f, dir, laser, bounce + 1); // dịch 1 chút để tránh trùng vị trí
        }
        else if (hitInfo.collider.CompareTag("Target"))
        {
            NextLevel();
        }
        else
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }

    public void NextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Nếu còn level tiếp theo
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("🎉 Hoàn thành tất cả level!");
            // Quay lại level đầu hoặc màn chính
            SceneManager.LoadScene(0);
        }
    }
}
