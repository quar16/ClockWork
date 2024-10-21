using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public EventManager eventManager;

    public Vector2Int eventPoint;
    public EventManager.EventType eventType;
    // Start is called before the first frame update


    public void EventCall()
    {
        eventManager.StartEvent(eventType);
    }
}
