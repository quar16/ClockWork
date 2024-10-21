using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileFunction : MonoBehaviour
{
    public AllManager manager;

    //스크립트 연결

    public Vector3Int tileXZIndex;
    public Vector3Int tileHexIndex;
    //헥스의 좌표 배열의 좌표

    public GameObject Highlight;
    public GameObject RangeHex;
    //빨간, 파란색의 헥스 강조 표시
    public GameObject TargetHex;
    //타겟이 있음을 강조하는 헥스

    public EffectManager.Target onTileObj;
    public GameObject onObj;
    //자신 위에 있는 오브젝트 1.플레이어  2.에너미   3.장애물   4.없음    N.타일(이건 여기서 안씀)

    public void _OnMouseEnter()
    {
        manager.battleMap.mouseXZIndex = tileXZIndex;
        Highlight.SetActive(true);

    }
    //마우스가 올라오면 마우스의 헥스,배열 좌표를 지정
    
    public void _OnMouseExit()
    {
        if (manager.battleMap.mouseXZIndex == tileXZIndex)
            manager.battleMap.mouseXZIndex = new Vector3Int(-1, -1, -1);

        Highlight.SetActive(false);
    }
    //마우스가 나가면 초기화

    public bool isTargetOn = false;
    public void SetRange()
    {
        Vector3Int activeHex = manager.battleMap.XZtoHex(manager.character.Playable[manager.character.selectedPlayer].playerXZIndex);
        Vector3Int Vrange = tileHexIndex - activeHex;
        int range = (int)((Abs(Vrange.x) + Abs(Vrange.y) + Abs(Vrange.z)) * 0.5f);

        switch (manager.effect.rangeType)
        {
            case EffectManager.RangeType.ALL:
                break;

            case EffectManager.RangeType.RANGE:
                if (manager.effect.rangeIndex1 < range || manager.effect.rangeIndex2 > range)
                   return;
                break;

            case EffectManager.RangeType.SIX:
                if (manager.effect.rangeIndex1 < range || (tileHexIndex.x != activeHex.x && tileHexIndex.y != activeHex.y && tileHexIndex.z != activeHex.z))
                   return;
                break;

            case EffectManager.RangeType.DEFAULT:
                if (manager.effect.rangeIndex1 < range)
                    return;
                break;
        }
        rangeDispalyOn = true;
        manager.effect.cardData.GetTile(this);
        StartCoroutine(rangeDispaly());
    }
    bool rangeDispalyOn = false;
    IEnumerator rangeDispaly()
    {
        while (rangeDispalyOn)
        {
            if (onTileObj == manager.effect.target || manager.effect.target == EffectManager.Target.ALL)
            {
                RangeHex.SetActive(false);
                TargetHex.SetActive(true);
                isTargetOn = true;
            }
            else
            {
                RangeHex.SetActive(true);
                TargetHex.SetActive(false);
                isTargetOn = false;
            }
            yield return null;
        }
        RangeHex.SetActive(false);
        TargetHex.SetActive(false);
        isTargetOn = false;
    }

    public bool CheckRange()
    {
        if (isTargetOn)
        {
            isTargetOn = false;
            return true;
        }
        return false;
    }
        
    public void DestroyRange()
    {
        rangeDispalyOn = false;
    }



    int Abs(int i)
    {
        if (i < 0)
            i *= -1;
        return i;
    }
}
