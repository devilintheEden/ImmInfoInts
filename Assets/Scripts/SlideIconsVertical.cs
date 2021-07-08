using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlideIconsVertical : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public GameObject sphere;
    private Vector3 originalPosition;
    private Vector3 beginRayPosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        beginRayPosition = sphere.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float target_y = originalPosition.y + (sphere.transform.position - beginRayPosition).y;
        target_y = Mathf.Clamp(target_y, -5, 5);
        transform.position = new Vector3(originalPosition.x, target_y, originalPosition.z);
    }
}
