using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileFunction : MonoBehaviour
{
    public MapManager mapManager;
    public Vector2Int tilePoint;
    public GameObject eventIcon;

    public void _OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            mapManager.MovePlayer(transform.position, tilePoint, eventIcon);
        }
    }
    public void SetIcon(GameObject prefab)
    {
        eventIcon = Instantiate(prefab);
        eventIcon.transform.SetParent(gameObject.transform);
        eventIcon.transform.localPosition = Vector3.zero;
    }
}
