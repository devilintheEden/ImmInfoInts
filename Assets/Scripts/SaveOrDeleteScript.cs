using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveOrDeleteScript : MonoBehaviour
{
    public RectTransform currentCard;
    public RectTransform content;
    public GameObject sideButtonSet;
    public GameObject newCard;
    private ContentUpdateScript contentUpdateScript;
    private int serialNum;

    public void Start()
    {
        content.sizeDelta = new Vector2(200, 0);
        content.DOSizeDelta(new Vector2(200, 140), 0.3f, false);
        currentCard.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentUpdateScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ContentUpdateScript>();
        serialNum = currentCard.gameObject.GetComponent<ClickOnCardScript>().serialNum;
    }

    public void SaveCard()
    {
        contentUpdateScript.savedItems.Add(serialNum);
        RemoveCardFromView();
        AddNewCardToView();
    }

    public void DeleteCard()
    {
        contentUpdateScript.deletedItems.Add(serialNum);
        RemoveCardFromView();
        AddNewCardToView();
    }

    private void RemoveCardFromView()
    {
        sideButtonSet.SetActive(false);
        contentUpdateScript.pendingItems.Remove(serialNum);
        contentUpdateScript.onScreenItemsNum -= 1;
        currentCard.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
        content.DOSizeDelta(new Vector2(200, 0), 0.3f, false);
    }

    private void AddNewCardToView()
    {
        List<int> temp1 = contentUpdateScript.pendingItems;
        int temp2 = contentUpdateScript.onScreenItemsNum;
        if (temp1.Count > temp2)
        {
            GameObject theNewCard = Instantiate(newCard, currentCard.transform.parent);
            theNewCard.transform.localPosition = new Vector3(0, -188, 0);
            theNewCard.GetComponent<ClickOnCardScript>().serialNum = temp1[temp2];
            contentUpdateScript.onScreenItemsNum += 1;
        }
        StartCoroutine(WaitingDestroy());
    }

    IEnumerator WaitingDestroy()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(currentCard.gameObject);
    }
}
