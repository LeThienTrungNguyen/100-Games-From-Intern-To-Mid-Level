using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingCounter_Visual : MonoBehaviour
{
    PlayerController controller;
    public Transform PutDownPosition;
    public Transform PutDownObject;
    public RectTransform CuttingProcessBar;
    public RectTransform CuttingBackGround;
    public RectTransform CuttingBorder;

    int cuttingMaxCount;
    int cuttingProgress;
    void Awake()
    {
        controller = FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
    }
    void FixedUpdate()
    {
        DisableCuttingUI();
        if (PutDownObject != null && cuttingProgress > 0)
        {
            EnableCuttingUI();
        }
    }
    void DisableCuttingUI()
    {
        CuttingProcessBar.gameObject.SetActive(false);
        CuttingBackGround.gameObject.SetActive(false);
        CuttingBorder.gameObject.SetActive(false);

    }

    void EnableCuttingUI()
    {
        CuttingProcessBar.gameObject.SetActive(true);
        CuttingBackGround.gameObject.SetActive(true);
        CuttingBorder.gameObject.SetActive(true);
    }
    public void CuttingHandle()
    {
        if (!LayerUtils.IsContainLayer(PutDownObject.gameObject.layer, controller.cutableLayer)) return;
        cuttingProgress++;
        HandleCuttingProgressBar();

        if (cuttingProgress >= cuttingMaxCount)
        {
            SpawnCuttedObject();
            cuttingProgress = 0;
            Invoke(nameof(DisableCuttingUI), 1f);
        }
    }
    void SpawnCuttedObject()
    {
        var cuttedObject = Instantiate(PutDownObject.GetComponent<KitchenObject>().CuttedObject,PutDownPosition.position,Quaternion.identity);
        Destroy(PutDownObject.gameObject);
        PutDownObject = cuttedObject;
    }
    void HandleCuttingProgressBar()
    {
        float progress = cuttingProgress;
        float max = cuttingMaxCount;
        float progressPrecent = progress / max;
        CuttingProcessBar.localScale = new Vector3(progressPrecent, 1, 1);

        
    }

    public void SetCuttingMaxCount()
    {
        cuttingMaxCount = PutDownObject.GetComponent<KitchenObject>().cuttingMaxCount;
    }
}
