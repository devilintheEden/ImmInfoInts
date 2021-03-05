using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentUpdateScript : MonoBehaviour
{
    public TextAsset sampleAIOText;
    public List<Dictionary<string, object>> data;
    public List<int> savedItems;
    public List<int> deletedItems;
    public List<int> pendingItems;
    public int onScreenItemsNum;
    public Text console;
    // Start is called before the first frame update
    void Awake()
    {
        //topic, rank, title, url, snippet
        data = CSVReader.Read(sampleAIOText);
        savedItems = new List<int>();
        deletedItems = new List<int>();
        pendingItems = new List<int>();
        for(int i = 0; i < data.Count; i++)
        {
            pendingItems.Add(i);
        }
        onScreenItemsNum = 3;
    }

    void Update()
    {
        console.text = "Saved Items: " + savedItems.Count + "\nDeleted items: " + deletedItems.Count + "\nPending Items: " + pendingItems.Count;
    }

    public void DebugLog(string s)
    {
        console.text = s;
    }
}
