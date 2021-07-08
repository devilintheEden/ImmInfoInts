using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSettings : MonoBehaviour
{
    public GameObject SessionManager;

    public void ChangeColumnNumberToOne(bool b)
    {
        SessionManager.GetComponent<SessionManager>().UpdateColumnNum(1);
    }

    public void ChangeColumnNumberToTwo(bool b)
    {
        SessionManager.GetComponent<SessionManager>().UpdateColumnNum(2);
    }

    public void ChangeColumnNumberToThree(bool b)
    {
        SessionManager.GetComponent<SessionManager>().UpdateColumnNum(3);
    }

    public void ChangeInteractionTypeToMenu(bool b)
    {
        SessionManager.GetComponent<SessionManager>().UpdateInteractType(InteractMode.Menu);
    }

    public void ChangeInteractionTypeToSwipe(bool b)
    {
        SessionManager.GetComponent<SessionManager>().UpdateInteractType(InteractMode.Swipe);
    }

    public void ChangeInteractionTypeToPlace(bool b)
    {
        SessionManager.GetComponent<SessionManager>().UpdateInteractType(InteractMode.Place);
    }

}