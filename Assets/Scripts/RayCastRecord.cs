using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FileConstant
{
    public DateTime CurrentTime { get; set; }
    public string PointerOrigin { get; set; }
    public string PointerDirection { get; set; }
    public string HeadOrigin { get; set; }
    public string HeadDirection { get; set; }
    public string RayCollidePosition { get; set; }
    public string MouseOverObjectType { get; set; }   
    public string MouseOverAIO_ID { get; set; }
}

public class FileTrigger
{
    public DateTime CurrentTime { get; set; }
    public string EventType { get; set; }
    public string ClickCollidePosition { get; set; }
    public string DragStartPosition { get; set; }
    public string DragEndPosition { get; set; }
    public string InteractObjectType { get; set; }
    public string InteractAIO_ID { get; set; }

}

public class GlobalSettings
{
    public int UserId { get; set; }
    public int trialID { get; set; }
    public int ColumnNum { get; set; }
    public string InteractionType { get; set; }
    public int Topic { get; set; }
    public string AIOTextFile { get; set; }
    public string OutputConstantFile { get; set; }
    public string OutputTriggerFile { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime FinishTime { get; set; }

}

public class RayCastRecord : MonoBehaviour
{
    public GameObject centerEye;
    public Text Statistics;
    public OVRInputModule ovrInput;
    public List<FileConstant> keyValuePairs_Constant;
    public List<FileTrigger> keyValuePairs_Trigger;

    void Start()
    {
        keyValuePairs_Constant = new List<FileConstant>();
        keyValuePairs_Trigger = new List<FileTrigger>();
        StartCoroutine(UpdateEverySecond());
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            GameObject sphere = GameObject.FindGameObjectWithTag("Sphere");
            Physics.Raycast(ovrInput.rayTransform.position, ovrInput.rayTransform.forward, out RaycastHit hit);
            string temp = hit.collider == null ? "" : hit.collider.gameObject.name;
            string objectType = GetColliderType(temp);
            string AIO_ID = (objectType == "AIO Card") ? hit.collider.gameObject.GetComponent<SingleCardFunction>().GetSerialNumString() : "";
            AIO_ID = objectType.Contains("Button") ? hit.collider.transform.parent.parent.gameObject.GetComponent<SingleCardFunction>().GetSerialNumString() : AIO_ID;
            
            keyValuePairs_Trigger.Add(new FileTrigger
            {
                CurrentTime = DateTime.Now,
                EventType = (sphere != null) ? "Click" : "Space Click",
                ClickCollidePosition = (sphere == null) ? "" : sphere.transform.position.ToString("f3"),
                DragStartPosition = "",
                DragEndPosition = "",
                InteractObjectType = (sphere == null) ? "" : objectType,
                InteractAIO_ID = AIO_ID
            });
        }
    }

    IEnumerator UpdateEverySecond()
    {
        while (true) 
        {
            GameObject sphere = GameObject.FindGameObjectWithTag("Sphere");
            Physics.Raycast(ovrInput.rayTransform.position, ovrInput.rayTransform.forward, out RaycastHit hit);
            string temp = hit.collider == null ? "" : hit.collider.gameObject.name;
            string objectType = GetColliderType(temp);

            string AIO_ID = (objectType == "AIO Card") ? hit.collider.gameObject.GetComponent<SingleCardFunction>().GetSerialNumString() : "";
            AIO_ID = objectType.Contains("Button") ? hit.collider.transform.parent.parent.gameObject.GetComponent<SingleCardFunction>().GetSerialNumString() : AIO_ID;

            keyValuePairs_Constant.Add(new FileConstant
            {
                CurrentTime = DateTime.Now,
                PointerOrigin = ovrInput.rayTransform.position.ToString("f3"),
                PointerDirection = ovrInput.rayTransform.forward.ToString("f3"),
                HeadOrigin = centerEye.transform.position.ToString("f3"),
                HeadDirection = (centerEye.transform.rotation * Vector3.forward).ToString("f3"),
                RayCollidePosition = (sphere == null) ? "" : sphere.transform.position.ToString("f3"),
                MouseOverObjectType = (sphere == null) ? "" : objectType,
                MouseOverAIO_ID = AIO_ID
            }) ;
            
            yield return new WaitForSeconds(1f);
        }        
    }

    void OnApplicationPause()
    {
        CSVTool.Write(keyValuePairs_Constant, true);
        CSVTool.Write(keyValuePairs_Trigger, false);
    }

    private string GetColliderType(string temp)
    {
        string objectType = temp.Contains("Card") ? "AIO Card" : "";
        objectType = temp.Contains("Saved") ? "Saving Spot" : objectType;
        objectType = temp.Contains("Deleted") ? "Deleting Spot" : objectType;
        objectType = temp.Contains("Button-S") ? "Save Button" : objectType;
        objectType = temp.Contains("Button-D") ? "Delete Button" : objectType;
        objectType = objectType == "" ? "Others" : objectType;
        return objectType;
    }

    public void WriteSaveDeleteAction(int AIO_ID, bool save)
    {
        keyValuePairs_Trigger.Add(new FileTrigger
        {
            CurrentTime = DateTime.Now,
            EventType = save ? "Save Interaction" : "Remove Interaction",
            ClickCollidePosition = "",
            DragStartPosition = "",
            DragEndPosition = "",
            InteractObjectType = "AIO Card",
            InteractAIO_ID = AIO_ID.ToString()
        });
    }

    public void WriteDragAction(Vector3 startPos, Vector3 endPos, bool complete, int ID)
    {
        string temp = ID >= 0 ? "AIO Card" : "Deleting Spot";
        temp = ID == -1 ? "Saving Spot" : temp;
        keyValuePairs_Trigger.Add(new FileTrigger
        {
            CurrentTime = DateTime.Now,
            EventType = complete ? "Drag" : "Incomplete Drag",
            ClickCollidePosition = "",
            DragStartPosition = startPos.ToString("f3"),
            DragEndPosition = endPos.ToString("f3"),
            InteractObjectType = temp,
            InteractAIO_ID = ID.ToString()
        });
    }
}
