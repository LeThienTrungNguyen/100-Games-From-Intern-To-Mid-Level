using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int width = 8, height = 8;
    public GameObject[] gems;

    private GameObject[,] grid;
    private Gem firstSelected;
    private bool isBusy;

    void Start()
    {
        grid = new GameObject[width, height];
        SetupBoard();
    }

    void SetupBoard()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                SpawnGem(x, y);
    }

    void SpawnGem(int x, int y, bool fromAbove = false)
    {
        Vector2 spawnPos = fromAbove ? new Vector2(x, y + 2) : new Vector2(x, y);
        GameObject gem = Instantiate(gems[Random.Range(0, gems.Length)], spawnPos, Quaternion.identity);
        Gem g = gem.GetComponent<Gem>();
        g.x = x; g.y = y; g.board = this;
        grid[x, y] = gem;

        if (fromAbove) g.SetPos(x, y, 0.3f);
    }

    // ---------- INPUT ----------
    public void OnGemClicked(Gem clicked)
    {
        if (isBusy) return;

        if (firstSelected == null) { Select(clicked); return; }
        if (firstSelected == clicked) { Deselect(); return; }
        if (!IsAdjacent(firstSelected, clicked)) { Select(clicked); return; }

        StartCoroutine(DoSwap(firstSelected, clicked));
        Deselect();
    }

    void Select(Gem g) { if (firstSelected) Highlight(firstSelected, false); firstSelected = g; Highlight(g, true); }
    void Deselect() { Highlight(firstSelected, false); firstSelected = null; }
    void Highlight(Gem g, bool on) { if (g) g.transform.localScale = on ? Vector3.one * 1.15f : Vector3.one; }

    bool IsAdjacent(Gem a, Gem b) =>
        (Mathf.Abs(a.x - b.x) == 1 && a.y == b.y) ||
        (Mathf.Abs(a.y - b.y) == 1 && a.x == b.x);

    // ---------- SWAP ----------
    IEnumerator DoSwap(Gem a, Gem b)
    {
        isBusy = true;
        Swap(a, b, 0.2f);
        yield return new WaitForSeconds(0.25f);

        if (CheckMatches()) { ClearMatches(); }
        else { Swap(a, b, 0.2f); yield return new WaitForSeconds(0.25f); }
        isBusy = false;
    }

    void Swap(Gem a, Gem b, float duration = 0.2f)
    {
        int ax = a.x, ay = a.y, bx = b.x, by = b.y;
        grid[ax, ay] = b.gameObject; grid[bx, by] = a.gameObject;
        a.SetPos(bx, by, duration); b.SetPos(ax, ay, duration);
    }

    // ---------- MATCH & CLEAR ----------
    bool CheckMatches()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (IsMatchAt(x, y)) return true;
        return false;
    }

    bool IsMatchAt(int x, int y)
    {
        if (grid[x, y] == null) return false;
        string tag = grid[x, y].tag;
        return (x < width - 2 && MatchTag(x + 1, y, tag) && MatchTag(x + 2, y, tag)) ||
               (y < height - 2 && MatchTag(x, y + 1, tag) && MatchTag(x, y + 2, tag));
    }

    bool MatchTag(int x, int y, string tag) =>
        grid[x, y] != null && grid[x, y].tag == tag;

    void ClearMatches()
    {
        HashSet<GameObject> toDestroy = new HashSet<GameObject>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (IsMatchAt(x, y)) CollectMatch(toDestroy, x, y);

        foreach (var g in toDestroy)
        {
            Gem gm = g.GetComponent<Gem>();
            grid[gm.x, gm.y] = null;
            Destroy(g);
        }
        StartCoroutine(FillBoard());
    }

    void CollectMatch(HashSet<GameObject> list, int x, int y)
    {
        string tag = grid[x, y].tag;
        list.Add(grid[x, y]);
        if (x < width - 2 && MatchTag(x + 1, y, tag) && MatchTag(x + 2, y, tag))
            { list.Add(grid[x + 1, y]); list.Add(grid[x + 2, y]); }
        if (y < height - 2 && MatchTag(x, y + 1, tag) && MatchTag(x, y + 2, tag))
            { list.Add(grid[x, y + 1]); list.Add(grid[x, y + 2]); }
    }

    // ---------- FILL ----------
    IEnumerator FillBoard()
    {
        yield return new WaitForSeconds(0.2f);

        for (int x = 0; x < width; x++)
        {
            int empty = 0;
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null) empty++;
                else if (empty > 0)
                {
                    var g = grid[x, y]; grid[x, y] = null;
                    grid[x, y - empty] = g;
                    g.GetComponent<Gem>().SetPos(x, y - empty, 0.3f);
                }
            }
            for (int i = 0; i < empty; i++)
                SpawnGem(x, height - empty + i, true);
        }

        yield return new WaitForSeconds(0.3f);
        if (CheckMatches()) ClearMatches();
    }
}
