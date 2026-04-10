using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter_Visual : MonoBehaviour
{
    public Transform PutDownPosition;
    public Transform PutDownObject;

    public RectTransform StoveProgressBar;
    public RectTransform StoveBackGroundProgressBar;
    public RectTransform StoveBorderProgressBar;
    public float cookingMaxTimer = 5f;
    public float cookingProgress = 0f;
    public float baseWidthProgressBar = 1f;
    public Transform cookedMeatPrefab;
    public Transform burnedMeatPrefab;
    public Transform SizzlingParticles;
    public Transform StoveOnVisual;
    public bool isCooking = false;
    void FixedUpdate()
    {
        if (PutDownObject != null && PutDownObject.gameObject.layer == 13)
        {
            StoveProgressBar.gameObject.SetActive(true);
            StoveBackGroundProgressBar.gameObject.SetActive(true);
            StoveBorderProgressBar.gameObject.SetActive(true);
            SizzlingParticles.gameObject.SetActive(true);
            StoveOnVisual.gameObject.SetActive(true);

            Debug.Log("Activing Stove Progress Bar");
            cookingProgress += Time.deltaTime;
            if (cookingProgress >= cookingMaxTimer)
            {
                cookingProgress = 0;
                Transform cookedMeat = Instantiate(cookedMeatPrefab, PutDownPosition.position, Quaternion.identity).transform;
                Destroy(PutDownObject.gameObject);
                PutDownObject = cookedMeat;
            }
            float scaleX = cookingProgress / cookingMaxTimer * baseWidthProgressBar;
            StoveProgressBar.localScale = new Vector3(scaleX, 1, 1);

        }
        else if (PutDownObject != null && PutDownObject.gameObject.layer == 12)
        {
            StoveProgressBar.gameObject.SetActive(true);
            StoveBackGroundProgressBar.gameObject.SetActive(true);
            StoveBorderProgressBar.gameObject.SetActive(true);
            SizzlingParticles.gameObject.SetActive(true);
            StoveOnVisual.gameObject.SetActive(true);



            Debug.Log("Activing Stove Progress Bar");
            cookingProgress += Time.deltaTime;
            if (cookingProgress >= cookingMaxTimer)
            {
                cookingProgress = 0;
                Transform burnedMeat = Instantiate(burnedMeatPrefab, PutDownPosition.position, Quaternion.identity).transform;
                Destroy(PutDownObject.gameObject);
                PutDownObject = burnedMeat;
            }
            float scaleX = cookingProgress / cookingMaxTimer * baseWidthProgressBar;
            StoveProgressBar.localScale = new Vector3(scaleX, 1, 1);
        }
        else
        {
            cookingProgress = 0;
            StoveProgressBar.gameObject.SetActive(false);
            StoveBackGroundProgressBar.gameObject.SetActive(false);
            StoveBorderProgressBar.gameObject.SetActive(false);
            SizzlingParticles.gameObject.SetActive(false);
            StoveOnVisual.gameObject.SetActive(false);
        }
    }

}
