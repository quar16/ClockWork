using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterFunc : MonoBehaviour
{
    public AllManager manager;
    public HpFunc hpFunc;

    //스크립트 연결, 매니저가 생성하면서 해줌
    
    public Vector3Int playerXZIndex;
    //배열 좌표

    public int ChrNumber;
    //플레이어의 고유 숫자

    public Break deathPref;
    public GameObject model;
    public GameObject deathEffect;

    public GameObject highlight;
    public Text MoveUI;
    //이동시 띄우는 UI, 매니저가 연결
    bool isSelected = false;
    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        isSelected = true;
        if (!isMoving)
        {
            manager.character.ChangeHighlight(ChrNumber);
            manager.flow.setAP(ChrNumber);
        }
    }
    //선택됐을 때 캐릭터 매니저에 선택됐다고 알려줌
    bool isMoving = false;

    Vector3Int temp;
    Vector3Int targetXZPos;
    int range;
    public void OnMouseDrag()
    {
        if (!isMoving&&isSelected)
        {
            MoveUI.rectTransform.position = Input.mousePosition + new Vector3(10, 10, 0);
            MoveUI.gameObject.SetActive(true);

            range = manager.battleMap.Distance(playerXZIndex, manager.battleMap.mouseXZIndex);

            if (manager.battleMap.mouseXZIndex == new Vector3Int(-1, -1, -1) || manager.battleMap.XZtoTile(manager.battleMap.mouseXZIndex).onTileObj != EffectManager.Target.TILE)
            {
                MoveUI.text = "X";
            }
            else if (range == manager.battleMap.HexDistance(playerXZIndex, manager.battleMap.mouseXZIndex))
            {
                MoveUI.text = range.ToString();
            }
            else
            {
                MoveUI.text = "X";
            }
        }
    }

    public void OnMouseUp()
    {
        if (!isMoving&&isSelected)
        {
            isSelected = false;
            targetXZPos = manager.battleMap.mouseXZIndex;
            MoveUI.gameObject.SetActive(false);

            if (MoveUI.text == "X")
                return;
            if (range > manager.flow.Ap[manager.character.selectedPlayer])
                return;
            if (manager.battleMap.XZtoTile(targetXZPos).onObj != null)
                return;

            manager.flow.setAP(ChrNumber, -range);

            Vector3 start = manager.battleMap.XZtoTile(playerXZIndex).transform.position;
            Vector3 end = manager.battleMap.XZtoTile(targetXZPos).transform.position;
            BattleTileFunction btf = manager.battleMap.XZtoTile(playerXZIndex);
            btf.onTileObj = EffectManager.Target.TILE;
            btf.onObj = null;
            playerXZIndex = targetXZPos;
            btf = manager.battleMap.XZtoTile(playerXZIndex);
            btf.onTileObj = EffectManager.Target.FRIENDLY;
            btf.onObj = gameObject;

            StartCoroutine(Move(start, end));

        }
    }
    public GameObject endPref;
    IEnumerator Move(Vector3 startPosition, Vector3 target)
    {
        GameObject temp = manager.InstantiateP(endPref);
        isMoving = true;
        temp.transform.position = target;
        LineEnd lineEnd = temp.GetComponent<LineEnd>();
        lineEnd.end = target;
        lineEnd.manager = manager;
        lineEnd.characterFunc = this;
        lineEnd.setStart();

        int myOrder = manager.flow.GetOrderNumber();
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.MOVE, ChrNumber);
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);
        if (isAlive)
        {
            float L = 0;
            float speed = 3.0f / range;
            while (L <= 1)
            {
                L += speed * Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition + Vector3.up * 0.75f, target + Vector3.up * 0.75f, L);
                yield return null;
            }
        }
        isMoving = false;
        manager.flow.NextOrder();
        lineEnd.SetEnd();
        Destroy(OIC);
    }
    public IEnumerator moveAct(Vector3 startPosition, Vector3 target,Vector3Int targetXZ)
    {
        if (isAlive)
        {
            BattleTileFunction btf = manager.battleMap.XZtoTile(playerXZIndex);
            btf.onTileObj = EffectManager.Target.TILE;
            btf.onObj = null;
            playerXZIndex = targetXZ;
            btf = manager.battleMap.XZtoTile(playerXZIndex);
            btf.onTileObj = EffectManager.Target.FRIENDLY;
            btf.onObj = gameObject;

            float L = 0;
            float speed = 3;
            while (L <= 1)
            {
                L += speed * Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition + Vector3.up * 0.75f, target + Vector3.up * 0.75f, L);
                yield return null;
            }
        }
    }
    public bool isAlive = true;
    public void Death()
    {
        isAlive = false;
        gameObject.layer = 2;
        BattleTileFunction btf = manager.battleMap.XZtoTile(playerXZIndex);
        btf.onTileObj = EffectManager.Target.TILE;
        btf.onObj = null;

        model.SetActive(false);
        deathPref.gameObject.SetActive(true);
        StartCoroutine(manager.camera.Shake(1, 0.3f));
        GameObject temp = manager.InstantiateP(deathEffect);
        temp.transform.position = gameObject.transform.position;
        deathPref.StartBreak();
    }

    
    int Abs(int i)
    {
        if (i < 0)
            i *= -1;
        return i;
    }
}
