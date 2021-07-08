using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RayCastRecord : MonoBehaviour
{
    public Text Statistics;
    public OVRInputModule ovrInput;
    public List<Dictionary<string, string>> keyValuePairs;
    //public List<RaycastResult> list;
    //public Vector2 screenPoint;

    void Start()
    {
        keyValuePairs = new List<Dictionary<string, string>>();
        StartCoroutine(UpdateEverySecond());
    }

    IEnumerator UpdateEverySecond()
    {
        while (true)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>
            {
                { "Current Time", DateTime.Now.ToString() },
                { "Pointer Origin", ovrInput.rayTransform.position.ToString().Replace(",",";") },
                { "Pointer Direction", ovrInput.rayTransform.forward.ToString().Replace(",",";") },
            };
            keyValuePairs.Add(temp);
            yield return new WaitForSeconds(1f);
        }
    }

    void OnApplicationPause()
    {
        CSVTool.Write(keyValuePairs);
    }

}
