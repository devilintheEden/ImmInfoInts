using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SingleCardFunction : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text Title;
    public Text URL;
    public Text Snippet;
    public GameObject SidePanel;
    public GameObject Content;

    private Vector3 originalPosition;
    private Vector3 beginRayPosition;
    private GameObject sphere;
    private GameObject savedItems;
    private GameObject deletedItems;
    private SessionManager sessionManager;
    private RayCastRecord rayCastRecord;

    private bool dragActive = true;

    private int serialNum;

    public void Start()
    {
        sphere = GameObject.FindGameObjectWithTag("Sphere");
        savedItems = GameObject.FindGameObjectWithTag("VRUISavedItems");
        deletedItems = GameObject.FindGameObjectWithTag("VRUIDeletedItems");
        sessionManager = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>();
        rayCastRecord = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<RayCastRecord>();
    }

    public void LateStart(int index, Dictionary<string, object> content)
    {
        Title.text = (string)content["title"];
        URL.text = (string)content["url"];
        Snippet.text = (string)content["snippet"];
        serialNum = index;
        Invoke(nameof(UpdateColliderSize), 0.01f);
    }

    public void Update()
    {
        if (sphere == null)
        {
            sphere = GameObject.FindGameObjectWithTag("Sphere");
        }
    }

    public void SideButtonActive(bool b)
    {
        SidePanel.SetActive(b);
        gameObject.GetComponent<BoxCollider>().center = b ? new Vector3(-20, 0, 0) : Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (sessionManager.InteractType != InteractMode.Menu)
        {
            originalPosition = transform.position;
            beginRayPosition = sphere.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (sessionManager.InteractType == InteractMode.Swipe)
        {
            if (dragActive)
            {
                float delta_x = (sphere.transform.position - beginRayPosition).x;
                if (delta_x > 1)
                {
                    delta_x = 1;
                    rayCastRecord.WriteDragAction(originalPosition, transform.position, true, serialNum);
                    dragActive = false;
                    transform.position = originalPosition;
                    SaveThisCard();
                }
                else if (delta_x < -1)
                {
                    delta_x = -1;
                    rayCastRecord.WriteDragAction(originalPosition, transform.position, true, serialNum);
                    dragActive = false;
                    transform.position = originalPosition;
                    DeleteThisCard();
                }
                transform.position = new Vector3(delta_x, 0, 0) + originalPosition;
            }
        }
        if (sessionManager.InteractType == InteractMode.Place)
        {
            if (dragActive)
            {
                Vector3 temp = sphere.transform.position - beginRayPosition + originalPosition;
                transform.position = new Vector3(temp.x, temp.y, originalPosition.z);
                if (Vector3.Distance(transform.position, savedItems.transform.position) <= 1.46f)
                {
                    rayCastRecord.WriteDragAction(originalPosition, transform.position, true, serialNum);
                    dragActive = false;
                    SaveThisCard();
                }
                if (Vector3.Distance(transform.position, deletedItems.transform.position) <= 1.46f)
                {
                    rayCastRecord.WriteDragAction(originalPosition, transform.position, true, serialNum);
                    dragActive = false;
                    DeleteThisCard();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (sessionManager.InteractType != InteractMode.Menu)
        {
            rayCastRecord.WriteDragAction(originalPosition, transform.position, false, serialNum);
            transform.DOMove(originalPosition, Vector3.Distance(transform.position, originalPosition) * 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sessionManager.InteractType == InteractMode.Menu)
        {
            bool temp = SidePanel.activeSelf;
            sessionManager.CancelAllSidePanel();
            SideButtonActive(!temp);
        }
    }

    public void SaveThisCard()
    {
        SideButtonActive(false);
        rayCastRecord.WriteSaveDeleteAction(serialNum, true);
        sessionManager.RemoveCard(gameObject, serialNum, true);
    }

    public void DeleteThisCard()
    {
        SideButtonActive(false);
        rayCastRecord.WriteSaveDeleteAction(serialNum, false);
        sessionManager.RemoveCard(gameObject, serialNum, false);
    }

    private void UpdateColliderSize()
    {
        Vector2 temp = Content.GetComponent<RectTransform>().sizeDelta;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(temp.x, temp.y, 4);
    }

    public string GetSerialNumString()
    {
        return serialNum.ToString();
    }
}
