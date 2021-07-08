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

    private Vector3 originalPosition;
    private Vector3 beginRayPosition;
    private GameObject sphere;
    private GameObject savedItems;
    private GameObject deletedItems;
    private SessionManager SessionManager;

    private bool dragActive = true;

    private int SerialNum;

    public void Start()
    {
        sphere = GameObject.FindGameObjectWithTag("Sphere");
        savedItems = GameObject.FindGameObjectWithTag("VRUISavedItems");
        deletedItems = GameObject.FindGameObjectWithTag("VRUIDeletedItems");
        SessionManager = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>();
    }

    public void LateStart(int index, Dictionary<string, object> content)
    {
        Title.text = (string)content["title"];
        URL.text = (string)content["url"];
        Snippet.text = (string)content["snippet"];
        SerialNum = index;
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (SessionManager.InteractType != InteractMode.Menu)
        {
            originalPosition = transform.position;
            beginRayPosition = sphere.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SessionManager.InteractType == InteractMode.Swipe)
        {
            if (dragActive)
            {
                float delta_x = (sphere.transform.position - beginRayPosition).x;
                if (delta_x > 1)
                {
                    delta_x = 1;
                    dragActive = false;
                    transform.position = originalPosition;
                    SaveThisCard();
                }
                else if (delta_x < -1)
                {
                    delta_x = -1;
                    dragActive = false;
                    transform.position = originalPosition;
                    DeleteThisCard();
                }
                transform.position = new Vector3(delta_x, 0, 0) + originalPosition;
            }
        }
        if (SessionManager.InteractType == InteractMode.Place)
        {
            if (dragActive)
            {
                Vector3 temp = sphere.transform.position - beginRayPosition + originalPosition;
                transform.position = new Vector3(temp.x, temp.y, originalPosition.z);
                if (Vector3.Distance(transform.position, savedItems.transform.position) <= 1.46f)
                {
                    dragActive = false;
                    SaveThisCard();
                }
                if (Vector3.Distance(transform.position, deletedItems.transform.position) <= 1.46f)
                {
                    dragActive = false;
                    DeleteThisCard();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (SessionManager.InteractType != InteractMode.Menu)
        {
            transform.DOMove(originalPosition, Vector3.Distance(transform.position, originalPosition) * 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SessionManager.InteractType == InteractMode.Menu)
        {
            bool temp = SidePanel.activeSelf;
            SessionManager.CancelAllSidePanel();
            SideButtonActive(!temp);
        }
    }

    public void SaveThisCard()
    {
        SideButtonActive(false);
        SessionManager.RemoveCard(gameObject, SerialNum, true);
    }

    public void DeleteThisCard()
    {
        SideButtonActive(false);
        SessionManager.RemoveCard(gameObject, SerialNum, false);
    }
}
