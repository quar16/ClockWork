using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawCall : MonoBehaviour
{
    public AllManager manager;

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (manager.flow.Ap[manager.character.selectedPlayer] >= 3)
        {
            manager.flow.Ap[manager.character.selectedPlayer] /= 2;
            manager.flow.setAP(manager.character.selectedPlayer, 0);
            StartCoroutine(manager.card[manager.character.selectedPlayer].CardDraw());
            StartCoroutine(manager.card[manager.character.selectedPlayer].CardDraw());
        }
    }
    float angle = 0;
    public void Update()
    {
        angle += Time.deltaTime * 3;
        gameObject.transform.rotation = Quaternion.Euler(65, 0, angle);
    }
}
