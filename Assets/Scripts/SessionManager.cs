using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InteractMode
{
    Menu,
    Swipe,
    Place
}

public class SessionManager : MonoBehaviour
{
    public TextAsset AIOSampleText;
    public Text Statistics;
    public GameObject TableCard;
    public GameObject TutorialParent;
    public GameObject newCard;
    public GameObject[] TutorialPrefabs;

    public List<Dictionary<string, object>> AIOSampleList;
    public int ColumnNum = 1;
    public InteractMode InteractType = InteractMode.Menu;

    private List<int> CardLeft;
    private List<int> CardLeftNotOnScreen;
    private List<int> CardLeftOnScreen;
    private List<GameObject> CardObjectOnScreen;

    private List<int> CardSaved;
    private List<int> CardDeleted;

    // Start is called before the first frame update
    void Start()
    {
        AIOSampleList = CSVTool.Read(AIOSampleText);
        CardLeft = Enumerable.Range(0, AIOSampleList.Count).ToList();
        CardSaved = new List<int>();
        CardDeleted = new List<int>();
        CardObjectOnScreen = new List<GameObject>();
        UpdateAllCards();
        UpdateStatistics();
    }

    public void UpdateColumnNum(int newNum)
    {
        if (newNum != ColumnNum)
        {
            ColumnNum = newNum;
            for (int i = 0; i < TableCard.transform.childCount; i++)
            {
                if (i < ColumnNum)
                {
                    TableCard.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    TableCard.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            TableCard.transform.GetChild(0).eulerAngles = new Vector3(0, newNum != 1 ? -20 : 0, 0);
            TableCard.transform.GetChild(1).eulerAngles = new Vector3(0, newNum == 2 ? 20 : 0, 0);
            TableCard.transform.GetChild(2).eulerAngles = new Vector3(0, 20, 0);
            UpdateAllCards();
        }
    }

    public void UpdateInteractType(InteractMode newType)
    {
        if (newType != InteractType)
        {
            if (InteractType == InteractMode.Menu)
            {
                CancelAllSidePanel();
            }
            InteractType = newType;
            Destroy(TutorialParent.transform.GetChild(0).gameObject);
            Instantiate(TutorialPrefabs[(int)newType], TutorialParent.transform);
        }
    }

    private void AddNewCardToView(int index, Transform parent)
    {
        Dictionary<string, object> content = AIOSampleList[index];
        GameObject theNewCard = Instantiate(newCard, parent);
        theNewCard.GetComponent<SingleCardFunction>().LateStart(index, content);
        CardObjectOnScreen.Add(theNewCard);
    }

    public void RemoveCard(GameObject card, int index, bool keep)
    {
        CardLeftOnScreen.Remove(index);
        CardLeft.Remove(index);
        (keep ? CardSaved : CardDeleted).Add(index);
        CardObjectOnScreen.Remove(card);
        Destroy(card);
        //UpdateAllCards();
        if (CardLeftNotOnScreen.Count != 0)
        {
            CardLeftOnScreen.Add(CardLeftNotOnScreen[0]);
            AddNewCardToView(CardLeftNotOnScreen[0], card.transform.parent);
            CardLeftNotOnScreen.RemoveAt(0);
        }
        UpdateStatistics();
    }

    void UpdateAllCards()
    {
        CardLeftNotOnScreen = new List<int>();
        CardLeftOnScreen = new List<int>();
        foreach (GameObject card in CardObjectOnScreen)
        {
            Destroy(card);
        }
        CardObjectOnScreen = new List<GameObject>();
        int temp = Mathf.Min(ColumnNum * 3, CardLeft.Count);
        //CardLeft = CardLeft.GetRange(0, temp);
        CardLeftOnScreen = CardLeft.GetRange(0, temp);
        CardLeftNotOnScreen = CardLeft.GetRange(temp, CardLeft.Count - temp);
        for(int i = 0; i < CardLeftOnScreen.Count; i++)
        {
            AddNewCardToView(CardLeftOnScreen[i], TableCard.transform.GetChild(i / 3));
        }
    }
    
    public void CancelAllSidePanel()
    {
        foreach(GameObject card in CardObjectOnScreen)
        {
            card.GetComponent<SingleCardFunction>().SideButtonActive(false);
        }        
    }

    void Update()
    {
        UpdateStatistics();
    }
    private void UpdateStatistics()
    {
        Statistics.text = "Saved Items: " + CardSaved.Count + "\nDeleted items: " + CardDeleted.Count +
            "\nPending Items: " + CardLeft.Count;
    }
}
