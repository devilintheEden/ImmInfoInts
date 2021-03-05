using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSettingsScript : MonoBehaviour
{
    public GameObject leftColumn;
    public GameObject middleColumn;
    public GameObject rightColumn;
    public GameObject newCard;
    public int interactionType = 1;
    public GameObject SwipeHint;
    private int columnNumber = 1;
    private ContentUpdateScript contentUpdateScript;

    public void Start()
    {
        GameObject theNewCard = Instantiate(newCard, middleColumn.transform);
        contentUpdateScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ContentUpdateScript>();
        theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[0];
        theNewCard = Instantiate(newCard, middleColumn.transform);
        theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[1];
        theNewCard = Instantiate(newCard, middleColumn.transform);
        theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[2];
    }

    public void ChangeColumnNumberToOne(bool b)
    {
        int temp = contentUpdateScript.pendingItems.Count;
        if (columnNumber != 1)
        {
            foreach (Transform child in leftColumn.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in rightColumn.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in middleColumn.transform)
            {
                Destroy(child.gameObject);
            }
            if (temp >= 3)
            {
                GameObject theNewCard = Instantiate(newCard, middleColumn.transform);
                contentUpdateScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ContentUpdateScript>();
                theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[0];
                theNewCard = Instantiate(newCard, middleColumn.transform);
                theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[1];
                theNewCard = Instantiate(newCard, middleColumn.transform);
                theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[2];
            }
            columnNumber = 1;
            contentUpdateScript.onScreenItemsNum = temp < 3 ? temp : 3;
        }
    }

    public void ChangeColumnNumberToTwo(bool b)
    {
        int temp = contentUpdateScript.pendingItems.Count;
        if (columnNumber == 3)
        {
            foreach (Transform child in leftColumn.transform)
            {
                Destroy(child.gameObject);
            }
            if (temp >= 6)
            {
                ChangeColumnNumberToOne(true);
                ChangeColumnNumberToTwo(true);
            }
        }
        else if (columnNumber == 1)
        {
            for (int i = 3; i < (temp < 6 ? temp : 6); i++)
            {
                GameObject theNewCard = Instantiate(newCard, rightColumn.transform);
                theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[i];
            }
        }
        columnNumber = 2;
        contentUpdateScript.onScreenItemsNum = temp < 6 ? temp : 6;
    }

    public void ChangeColumnNumberToThree(bool b)
    {
        GameObject theNewCard;
        int temp = contentUpdateScript.pendingItems.Count;
        if (columnNumber != 3)
        {
            if (columnNumber == 1)
            {
                for (int i = 3; i < (temp < 6 ? temp : 6); i++)
                {
                    theNewCard = Instantiate(newCard, rightColumn.transform);
                    theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[i];
                }
            }
            for (int i = 6; i < (temp < 9 ? temp : 9); i++)
            {
                theNewCard = Instantiate(newCard, leftColumn.transform);
                theNewCard.GetComponent<ClickOnCardScript>().serialNum = contentUpdateScript.pendingItems[i];
            }
            columnNumber = 3;
            contentUpdateScript.onScreenItemsNum = temp < 9 ? temp : 9;
        }
    }

    public void ChangeInteractionTypeToMenu(bool b)
    {
        if (interactionType != 1)
        {
            if (interactionType == 2)
            {
                SwipeHint.SetActive(false);
            }
            interactionType = 1;
        }
    }

    public void ChangeInteractionTypeToSwipe(bool b)
    {
        if (interactionType != 2)
        {
            if (interactionType == 1)
            {
                CancelAllSidePanel();
            }
            interactionType = 2;
            SwipeHint.SetActive(true);
        }
    }

    public void ChangeInteractionTypeToPlace(bool b)
    {
        if (interactionType != 3)
        {
            if (interactionType == 1)
            {
                CancelAllSidePanel();
            }
            if (interactionType == 2)
            {
                SwipeHint.SetActive(false);
            }
            interactionType = 3;
        }
    }

    private void CancelAllSidePanel()
    {
        for (int i = 0; i < rightColumn.transform.childCount; i++)
        {
            rightColumn.transform.GetChild(i).gameObject.GetComponent<ClickOnCardScript>().sideButtonSet.SetActive(false);
            rightColumn.transform.GetChild(i).gameObject.GetComponent<ClickOnCardScript>().sideButtonOut = false;
        }
        for (int i = 0; i < middleColumn.transform.childCount; i++)
        {
            middleColumn.transform.GetChild(i).gameObject.GetComponent<ClickOnCardScript>().sideButtonSet.SetActive(false);
            middleColumn.transform.GetChild(i).gameObject.GetComponent<ClickOnCardScript>().sideButtonOut = false;
        }
        for (int i = 0; i < leftColumn.transform.childCount; i++)
        {
            leftColumn.transform.GetChild(i).gameObject.GetComponent<ClickOnCardScript>().sideButtonSet.SetActive(false);
            leftColumn.transform.GetChild(i).gameObject.GetComponent<ClickOnCardScript>().sideButtonOut = false;
        }
    }
}