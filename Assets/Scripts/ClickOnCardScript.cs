using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickOnCardScript : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int serialNum = -1;
    public GameObject sideButtonSet;
    public RectTransform content;
    public bool sideButtonOut = false;
    public GameObject newCard;
    public GameObject localScriptHolder;
    public Text title;
    public Text url;
    public Text snippet;
    private Vector3 originalPosition;
    private Vector3 beginRayPosition;
    private GameObject sphere;
    private GameObject savedItems;
    private GameObject deletedItems;
    private ContentUpdateScript contentUpdateScript;
    private bool dragActive = true;
    private bool contentUpdated = false;

    public void Start()
    {
        sphere = GameObject.FindGameObjectWithTag("Sphere");
        savedItems = GameObject.FindGameObjectWithTag("VRUISavedItems");
        deletedItems = GameObject.FindGameObjectWithTag("VRUIDeletedItems");
        contentUpdateScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ContentUpdateScript>();
    }

    public void Update()
    {
        if(serialNum != -1 && !contentUpdated)
        {
            title.text = (string)contentUpdateScript.data[serialNum]["title"];
            url.text = (string)contentUpdateScript.data[serialNum]["url"];
            snippet.text = (string)contentUpdateScript.data[serialNum]["snippet"];
            contentUpdated = true;
        }
        if(sphere == null)
        {
            sphere = GameObject.FindGameObjectWithTag("Sphere");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ControlSettingsScript>().interactionType != 1)
        {
            originalPosition = transform.position;
            beginRayPosition = sphere.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ControlSettingsScript>().interactionType == 2)
        {
            if (dragActive)
            {
                float delta_x = (sphere.transform.position - beginRayPosition).x;
                if (delta_x > 1)
                {
                    delta_x = 1;
                    dragActive = false;
                    transform.position = originalPosition;
                    localScriptHolder.GetComponent<SaveOrDeleteScript>().SaveCard();
                }
                else if (delta_x < -1)
                {
                    delta_x = -1;
                    dragActive = false;
                    transform.position = originalPosition;
                    localScriptHolder.GetComponent<SaveOrDeleteScript>().DeleteCard();
                }
                transform.position = new Vector3(delta_x, 0, 0) + originalPosition;
            }
        }
        if(GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ControlSettingsScript>().interactionType == 3)
        {
            if (dragActive)
            {
                contentUpdateScript.DebugLog(transform.position.ToString("2f"));
                Vector3 temp = sphere.transform.position - beginRayPosition + originalPosition;
                transform.position = new Vector3(temp.x, temp.y, originalPosition.z);
                if (Vector3.Distance(transform.position, savedItems.transform.position) <= 1.46f)
                {
                    dragActive = false;
                    localScriptHolder.GetComponent<SaveOrDeleteScript>().SaveCard();
                }
                if (Vector3.Distance(transform.position, deletedItems.transform.position) <= 1.46f)
                {
                    dragActive = false;
                    localScriptHolder.GetComponent<SaveOrDeleteScript>().DeleteCard();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ControlSettingsScript>().interactionType != 1)
        {
            transform.DOMove(originalPosition, Vector3.Distance(transform.position, originalPosition) * 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ControlSettingsScript>().interactionType == 1)
        {
            if (!sideButtonOut)
            {
                sideButtonSet.SetActive(true);
                sideButtonOut = true;
            }
            else
            {
                sideButtonSet.SetActive(false);
                sideButtonOut = false;
                transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
            }
        }
    }
}
