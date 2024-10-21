using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public AllManager manager;


    public TileParent tileParent;

    public Camera camera;

    public enum RangeType { ALL, SIX, RANGE, DEFAULT }
    public enum Target { TILE, ENEMY, FRIENDLY, NONE, OBSTACLE, ALL }
    public enum TargetObject { SINGLE, ENEMY, FRIENDLY, ALL, NONE }
    bool focusCall = false;
    public struct CardData
    {
        public Vector3Int activeXZ;                 //카드를 발동한 플레이어의 헥스
        public Vector3Int targetXZ;                 //카드의 효과를 발동시킬 타겟 헥스
        public Vector3 targetPos;                   //타겟 헥스의 포지션
        public int cardIndex;                       //카드의 인덱스(1~시작
        public int charaIndex;                      //발동한 플레이어 인덱스
        public BattleTileFunction[] inRangeTiles;   //카드의 범위를 전부 받아옴
        public int inRangeTileNumber;               //받아온 범위의 갯수
        public GameObject[] target;                 //발동할 타겟들
        public int targetN;                         //타겟 개수
        //이 변수들은 카드의 효과를 사용할 때 사용함
        public void GetTile(BattleTileFunction btf)
        {
            inRangeTiles[inRangeTileNumber] = btf;
            inRangeTileNumber++;
        }
    }
    
    public CardData cardData;
    
    public RangeType rangeType;
    public Target target;
    public TargetObject targetObject;
    public int rangeIndex1, rangeIndex2;
    //범위 계산
    public int cost;
    //코스트 계산

    public void SetRange(int i)
    {
        cardData.cardIndex = i;
        cardData.inRangeTileNumber = 0;
        cardData.inRangeTiles = new BattleTileFunction[58];
        cardData.targetN = 0;
        cardData.target = new GameObject[12];
        cardData.charaIndex = manager.character.selectedPlayer;

        int csvIndex = i - 1;
        CardManager card = manager.card[manager.character.selectedPlayer];
        target = (Target)System.Enum.Parse(typeof(Target), card.data[cardData.charaIndex][csvIndex]["Target"].ToString());
        targetObject = (TargetObject)System.Enum.Parse(typeof(TargetObject), card.data[cardData.charaIndex][csvIndex]["Object"].ToString());
        rangeType = (RangeType)System.Enum.Parse(typeof(RangeType), card.data[cardData.charaIndex][csvIndex]["Type"].ToString());
        rangeIndex1 = (int)card.data[cardData.charaIndex][csvIndex]["RangeIndex1"];
        rangeIndex2 = (int)card.data[cardData.charaIndex][csvIndex]["RangeIndex2"];
        cost = (int)card.data[cardData.charaIndex][csvIndex]["Cost"];

        tileParent.CallTiles();
    }
    
    public bool CheckRange()
    {
        if (manager.battleMap.mouseXZIndex == new Vector3Int(-1, -1, -1))
        {
            ReleaseRange();
            return false;
        }
        BattleTileFunction BTF = manager.battleMap.XZtoTile(manager.battleMap.mouseXZIndex);

        if (BTF.CheckRange())
        {
            if (cost <= manager.flow.Ap[manager.character.selectedPlayer])
            {
                ReleaseRange();
                manager.flow.setAP(manager.character.selectedPlayer, -cost);

                cardData.activeXZ = manager.character.Playable[manager.character.selectedPlayer].playerXZIndex;
                cardData.targetXZ = manager.battleMap.mouseXZIndex;
                cardData.targetPos = manager.battleMap.XZtoTile(cardData.targetXZ).transform.position;
                switch (targetObject)
                {
                    case TargetObject.SINGLE:
                        cardData.target[0] = manager.battleMap.XZtoTile(cardData.targetXZ).onObj;
                        break;
                    case TargetObject.ENEMY:
                        for (int i = 0; i < cardData.inRangeTileNumber; i++)
                        {
                            if (cardData.inRangeTiles[i].onTileObj == Target.ENEMY)
                            {
                                cardData.target[cardData.targetN] = cardData.inRangeTiles[i].onObj;
                                cardData.targetN++;
                            }
                        }
                        break;
                    case TargetObject.FRIENDLY:
                        for (int i = 0; i < cardData.inRangeTileNumber; i++)
                        {
                            if (cardData.inRangeTiles[i].onTileObj == Target.FRIENDLY)
                            {
                                cardData.target[cardData.targetN] = cardData.inRangeTiles[i].onObj;
                                cardData.targetN++;
                            }
                        }
                        break;
                    case TargetObject.ALL:
                        for (int i = 0; i < cardData.inRangeTileNumber; i++)
                        {
                            if (cardData.inRangeTiles[i].onTileObj == Target.ENEMY || cardData.inRangeTiles[i].onTileObj == Target.FRIENDLY)
                            {
                                cardData.target[cardData.targetN] = cardData.inRangeTiles[i].onObj;
                                cardData.targetN++;
                            }
                        }
                        break;
                }

                StartCoroutine(Active(cardData));
                return true;
            }
        }

        ReleaseRange();
        return false;
    }

    public void ReleaseRange()
    {
        tileParent.DestroyChild();
    }

    public int []DamageAdd = { 0,0,0 };
    public int[] DamageMlt = { 1, 1, 1 };
    int DamageCalc(int delta, int chr)
    {
        delta += DamageAdd[chr];
        delta *= DamageMlt[chr];
        return delta;
    }


    public IEnumerator Active(CardData cardData)
    {
        int myOrder = manager.flow.GetOrderNumber();
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.ACTIVE, cardData.charaIndex);
        focusCall = true;
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);
        yield return new WaitForSeconds(0.5f);
        if (manager.character.Playable[cardData.charaIndex].isAlive)
        {
            switch (cardData.charaIndex)
            {
                case 0:
                    switch (cardData.cardIndex)
                    {
                        case 1:
                            yield return StartCoroutine(rupture(cardData));
                            break;
                        case 2:
                            yield return StartCoroutine(unstable_cask(cardData));
                            break;
                        case 3:
                            yield return StartCoroutine(corrosive_flask(cardData));
                            break;
                        case 4:
                            yield return StartCoroutine(reassemble(cardData));
                            break;
                        case 5:
                            yield return StartCoroutine(call_lightning(cardData));
                            break;
                        case 6:
                            yield return StartCoroutine(overcharge(cardData));
                            break;
                        case 7:
                            yield return StartCoroutine(focus(cardData));
                            break;
                        case 8:
                            yield return StartCoroutine(transference(cardData));
                            break;
                        case 9:
                            yield return StartCoroutine(chain_reaction(cardData));
                            break;
                        case 10:
                            yield return StartCoroutine(trail_of_fire(cardData));
                            break;
                        case 11:
                            yield return StartCoroutine(gravity_field(cardData));
                            break;
                        case 12:
                            yield return StartCoroutine(cataclysm(cardData));
                            break;
                        case 13:
                            yield return StartCoroutine(superconductor(cardData));
                            break;
                    }
                    break;
                case 1:
                    switch (cardData.cardIndex)
                    {
                        case 1:
                            yield return StartCoroutine(ready(cardData));
                            break;
                        case 2:
                            yield return StartCoroutine(jab(cardData));
                            break;
                        case 3:
                            yield return StartCoroutine(strike(cardData));
                            break;
                        case 4:
                            yield return StartCoroutine(accelerate(cardData));
                            break;
                        case 5:
                            yield return StartCoroutine(PowerHit(cardData));
                            break;
                        case 6:
                            yield return StartCoroutine(Rush(cardData));
                            break;
                        case 7:
                            yield return StartCoroutine(Protect(cardData));
                            break;
                        case 8:
                            yield return StartCoroutine(RocketGrap(cardData));
                            break;
                        case 9:
                            yield return StartCoroutine(Counter(cardData));
                            break;
                        case 10:
                            yield return StartCoroutine(IronWall(cardData));
                            break;
                        case 11:
                            yield return StartCoroutine(Excute(cardData));
                            break;
                        case 12:
                            yield return StartCoroutine(Unbreakable(cardData));
                            break;
                    }
                    break;
                case 2:
                    break;
            }
            
        }
        manager.flow.NextOrder();
        Destroy(OIC);
    }

    public GameObject[] MageEffect;
    public GameObject[] GuardEffect;
    #region Mage
    IEnumerator rupture(CardData cardData)
    {
        Vector3Int delta = manager.battleMap.XZtoHex(cardData.targetXZ) - manager.battleMap.XZtoHex(cardData.activeXZ);
        GameObject[] prefab = new GameObject[3];
        BattleTileFunction[] btf = new BattleTileFunction[3];
        int tryN = 0;
        for(int i = 0; i < 3; i++)
        {
            try
            {
                Vector3Int v3 = manager.battleMap.HexToXZ(manager.battleMap.XZtoHex(cardData.activeXZ) + delta * (i + 1));
                btf[tryN] = manager.battleMap.XZtoTile(v3);
                prefab[tryN] = manager.InstantiateP(MageEffect[0]);
                prefab[tryN].transform.position = btf[i].transform.position;
                tryN++;
            }
            catch { }
        }
        for (int i = 0; i < tryN; i++)
        {
            if (btf[i].onTileObj == Target.ENEMY)
                btf[i].onObj.GetComponent<HpFunc>().SetHp(DamageCalc(-1, cardData.charaIndex));
        }
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < tryN; i++)
        {
            Destroy(prefab[i]);
        }
    }
    IEnumerator unstable_cask(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[1]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
        yield return new WaitForSeconds(2);
        Destroy(prefab);
    }
    IEnumerator corrosive_flask(CardData cardData)
    {
        float l = 0;
        Vector3 start = manager.character.Playable[manager.character.selectedPlayer].transform.position;
        Vector3 end = cardData.targetPos;
        GameObject prefab = manager.InstantiateP(MageEffect[2]);
        prefab.transform.LookAt(cardData.targetPos);

        while (l < 1)
        {
            prefab.transform.position = Vector3.Lerp(start, end, l);
            l += Time.deltaTime;
            prefab.transform.position += new Vector3(0, 4.5f + (1 - Mathf.Pow(((l * 2) - 1), 2)) * 10, 0);
            prefab.transform.rotation = Quaternion.Euler(l * 300, l * 250, 0);
            yield return null;
        }
        Destroy(prefab);
        GameObject prefab2 = manager.InstantiateP(MageEffect[3]);
        prefab2.transform.position = cardData.targetPos;
        yield return new WaitForSeconds(1);
        StartCoroutine(poison(cardData));
        Destroy(prefab2);
    }
    IEnumerator poison(CardData cardData)
    {
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1);
            cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-1, cardData.charaIndex));
            if (manager.flow.IsNoOrder() && cardData.target[0].GetComponent<HpFunc>().Hp <= 0)
            {
                int myOrder = manager.flow.GetOrderNumber();
                manager.flow.NextOrder();
            }
        }

    }
    IEnumerator reassemble(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[4]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-1, cardData.charaIndex));
        manager.flow.setAP(cardData.charaIndex, 2);
        yield return new WaitForSeconds(1);
        Destroy(prefab);
    }
    IEnumerator call_lightning(CardData cardData)
    {
        GameObject[] prefab = new GameObject[manager.enemy.enemyN];
        for(int i = 0; i < manager.enemy.enemyN; i++)
        {
            prefab[i] = manager.InstantiateP(MageEffect[5]);
            prefab[i].transform.position = cardData.target[i].transform.position;
        }
        for (int i = 0; i < cardData.targetN; i++)
        {
            cardData.target[i].GetComponent<HpFunc>().SetHp(DamageCalc(-99, cardData.charaIndex));
        }
        yield return StartCoroutine(manager.camera.Shake(1));
        for (int i = 0; i < manager.enemy.enemyN; i++)
        {
            Destroy(prefab[i]);
        }
    }
    IEnumerator overcharge(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[6]);
        CharacterFunc select = manager.character.Playable[cardData.charaIndex];
        prefab.transform.position = select.transform.position;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AfterOverCharge(prefab,select));
    }
    IEnumerator AfterOverCharge(GameObject prefab, CharacterFunc select)
    {
        float l = 0;
        float t = 0;
        DamageAdd[select.ChrNumber]--;
        while (l < 46)
        {
            l += Time.deltaTime;
            t += Time.deltaTime;
            if (t > 15)
            {
                select.GetComponent<HpFunc>().SetHp(-1);
                if (manager.flow.IsNoOrder() && select.GetComponent<HpFunc>().Hp <= 0)
                {
                    int myOrder = manager.flow.GetOrderNumber();
                    manager.flow.NextOrder();
                }
                t -= 15;
            }
            prefab.transform.position = select.transform.position;
            yield return null;
            if (!select.isAlive)
            {
                Destroy(prefab);
                yield break;
            }
        }
        DamageAdd[select.ChrNumber]++;
        Destroy(prefab);
    }
    IEnumerator focus(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[7]);
        CharacterFunc select = manager.character.Playable[cardData.charaIndex];
        prefab.transform.position = select.transform.position;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AfterFocus(prefab, select));
    }
    IEnumerator AfterFocus(GameObject prefab, CharacterFunc select)
    {
        float l = 3;
        focusCall = false;

        while (l != 0)
        {
            if (focusCall)
            {
                StartCoroutine(manager.card[select.ChrNumber].CardDraw());
                focusCall = false;
                l--;
            }
            prefab.transform.position = select.transform.position;
            yield return null;

            if (!select.isAlive)
            {
                Destroy(prefab);
                yield break;
            }
        }
        Destroy(prefab);
    }
    IEnumerator transference(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[8]);
        prefab.transform.position = cardData.targetPos;
        while (manager.flow.Ap[cardData.charaIndex] != 0)
        {
            manager.flow.setAP(cardData.charaIndex, -1);
            manager.flow.Ap[cardData.target[0].GetComponent<CharacterFunc>().ChrNumber]++;
            yield return null;
        }
        Destroy(prefab);
    }
    IEnumerator chain_reaction(CardData cardData)
    {
       

        GameObject temp = manager.InstantiateP(MageEffect[9]);
        temp.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
        yield return new WaitForSeconds(0.1f);
        BattleTileFunction[] btf = new BattleTileFunction[manager.enemy.EnemyF.Length];
        int t = 0;
        for (int i = 0; i < manager.enemy.EnemyF.Length; i++)
        {
            if (manager.enemy.EnemyF[i].GetAlive())
            {
                Vector3Int tempV = manager.enemy.EnemyF[i].GetEnemyXZIndex();
                int distance = manager.battleMap.Distance(cardData.targetXZ, tempV);
                if (distance <= 2 && distance != 0)
                {
                    btf[t] = manager.battleMap.XZtoTile(manager.enemy.EnemyF[i].GetEnemyXZIndex());
                    t++;
                }
            }
        }
        GameObject[] tempP = new GameObject[t];
        if (t != 0)
        {
            StartCoroutine(manager.camera.Shake(1, t * 0.5f));
            for (int i = 0; i < t; i++)
            {
                tempP[i] = manager.InstantiateP(MageEffect[9]);
                tempP[i].transform.position = btf[i].transform.position;
                btf[i].onObj.GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                manager.flow.setAP(cardData.charaIndex, 1);
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return new WaitForSeconds(1);
        Destroy(temp);
        for (int i = 0; i < t; i++)
        {
            Destroy(tempP[i]);
        }
    }
    IEnumerator trail_of_fire(CardData cardData)
    {
        Vector3Int delta = manager.battleMap.XZtoHex(cardData.targetXZ) - manager.battleMap.XZtoHex(cardData.activeXZ);
        int distance = manager.battleMap.Distance(cardData.targetXZ, cardData.activeXZ);
        delta = new Vector3Int(delta.x / distance, delta.y / distance, delta.z / distance);
        GameObject[] prefab = new GameObject[3];
        BattleTileFunction[] btf = new BattleTileFunction[3];
        int tryN = 0;
        for (int i = 0; i < 3; i++)
        {
            try
            {
                Vector3Int v3 = manager.battleMap.HexToXZ(manager.battleMap.XZtoHex(cardData.targetXZ) + delta * i);
                btf[tryN] = manager.battleMap.XZtoTile(v3);
                prefab[tryN] = manager.InstantiateP(MageEffect[10]);
                prefab[tryN].transform.position = btf[i].transform.position;
                tryN++;
            }
            catch { }
        }
        StartCoroutine(AfterFire(prefab, btf, tryN));
        yield return new WaitForSeconds(1);
    }
    IEnumerator AfterFire(GameObject[] prefab, BattleTileFunction[] btf, int N)
    {
        float t = 0;
        float[] cool = new float[N];
        while (t < 30)
        {
            for (int i = 0; i < N; i++)
            {
                if (btf[i].onTileObj == Target.ENEMY || btf[i].onTileObj == Target.FRIENDLY)
                {
                    cool[i] += Time.deltaTime;
                    if (cool[i] >= 5)
                    {
                        btf[i].onObj.GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
                        cool[i] = 0;
                        if (manager.flow.IsNoOrder() && btf[i].onObj.GetComponent<HpFunc>().Hp <= 0)
                        {
                            int myOrder = manager.flow.GetOrderNumber();
                            manager.flow.NextOrder();
                        }
                    }
                }
                else
                {
                    cool[i] = 0;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < N; i++)
        {
            Destroy(prefab[i]);
        }
    }
    IEnumerator gravity_field(CardData cardData) { yield return new WaitForSeconds(1); }
    IEnumerator cataclysm(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[11]);
        prefab.transform.position = cardData.targetPos;
        yield return new WaitForSeconds(1);
        StartCoroutine(AfterCataclysm(prefab, cardData));
    }
    IEnumerator AfterCataclysm(GameObject prefab, CardData cardData)
    {
        StartCoroutine(manager.camera.bip(13, 0.1f));
        yield return new WaitForSeconds(14);
        Destroy(prefab);
        GameObject prefab2 = manager.InstantiateP(MageEffect[12]);
        prefab2.transform.position = cardData.targetPos;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(manager.camera.Shake(0.5f, 4));
        BattleTileFunction[] btf = new BattleTileFunction[manager.enemy.EnemyF.Length];
        int t = 0;
        for (int i = 0; i < manager.enemy.EnemyF.Length; i++)
        {
            if (manager.enemy.EnemyF[i].GetAlive())
            {
                Vector3Int tempV = manager.enemy.EnemyF[i].GetEnemyXZIndex();
                int distance = manager.battleMap.Distance(cardData.targetXZ, tempV);
                if (distance <= 2)
                {
                    btf[t] = manager.battleMap.XZtoTile(manager.enemy.EnemyF[i].GetEnemyXZIndex());
                    t++;
                }
            }
        }
        for (int i = 0; i < t; i++)
        {
            DamageAdd[cardData.charaIndex] *= 2;
            btf[i].onObj.GetComponent<HpFunc>().SetHp(DamageCalc(-8, cardData.charaIndex), true);
            DamageAdd[cardData.charaIndex] /= 2;
            if (manager.flow.IsNoOrder() && btf[i].onObj.GetComponent<HpFunc>().Hp <= 0)
            {
                int myOrder = manager.flow.GetOrderNumber();
                manager.flow.NextOrder();
            }

        }
        yield return new WaitForSeconds(0.5f);
        Destroy(prefab2);

    }
    IEnumerator superconductor(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(MageEffect[13]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
        yield return new WaitForSeconds(1.5f);
        Destroy(prefab);
        GameObject prefab2 = manager.InstantiateP(MageEffect[14]);
        prefab2.transform.position = cardData.targetPos;
        StartCoroutine(AfterSuper(cardData, prefab2));
    }
    IEnumerator AfterSuper(CardData cardData, GameObject prefab)
    {
        yield return new WaitForSeconds(4);
        Destroy(prefab);
        BattleTileFunction[] btf = new BattleTileFunction[manager.enemy.EnemyF.Length];
        int t = 0;
        for (int i = 0; i < manager.enemy.EnemyF.Length; i++)
        {
            if (manager.enemy.EnemyF[i].GetAlive())
            {
                Vector3Int tempV = manager.enemy.EnemyF[i].GetEnemyXZIndex();
                int distance = manager.battleMap.Distance(cardData.targetXZ, tempV);
                if (distance <= 1)
                {
                    btf[t] = manager.battleMap.XZtoTile(manager.enemy.EnemyF[i].GetEnemyXZIndex());
                    t++;
                }
            }
        }
        GameObject[] tempP = new GameObject[t];
        StartCoroutine(manager.camera.Shake(1.5f, t * 0.5f));
        for (int i = 0; i < t; i++)
        {
            tempP[i] = manager.InstantiateP(MageEffect[13]);
            tempP[i].transform.position = btf[i].transform.position;
            btf[i].onObj.GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
            if (manager.flow.IsNoOrder() && btf[i].onObj.GetComponent<HpFunc>().Hp <= 0)
            {
                int myOrder = manager.flow.GetOrderNumber();
                manager.flow.NextOrder();
            }
        }
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < t; i++)
        {
            Destroy(tempP[i]);
        }
    }
    #endregion
    #region Guard

    IEnumerator ready(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[0]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().DamageAdd ++;
        yield return new WaitForSeconds(0.5f);
        Destroy(prefab);
        StartCoroutine(AfterReady(cardData.target[0].GetComponent<HpFunc>()));
    }
    IEnumerator AfterReady(HpFunc hpFunc)
    {
        float lastN = 0, nowN = 0;
        while (true)
        {
            nowN = hpFunc.Dp + hpFunc.Hp;
            if (lastN != nowN)
                break;
            lastN = hpFunc.Dp + hpFunc.Hp;
            yield return null;
        }
        hpFunc.DamageAdd--;
    }
    IEnumerator jab(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[1]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-2, cardData.charaIndex));
        StartCoroutine(manager.card[cardData.charaIndex].CardDraw());
        yield return new WaitForSeconds(1);
        Destroy(prefab);
    }
    IEnumerator strike(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[2]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-1, cardData.charaIndex));
        StartCoroutine(AfterStrike(cardData.target[0].GetComponent<EnemyInterface>()));
        StartCoroutine(manager.camera.Shake(1, 0.5f));
        yield return new WaitForSeconds(1);
        Destroy(prefab);
    }
    IEnumerator AfterStrike(EnemyInterface enemyInterface)
    {
        enemyInterface.SetMove(false);
        GameObject prefab = manager.InstantiateP(GuardEffect[3]);
        prefab.transform.position = enemyInterface.GetMono().transform.position;
        yield return new WaitForSeconds(15);
        Destroy(prefab);
        enemyInterface.SetMove(true);
    }
    IEnumerator accelerate(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[4]);
        prefab.transform.SetParent(manager.character.Playable[cardData.charaIndex].transform);
        prefab.transform.localPosition = Vector3.zero;
        yield return StartCoroutine(manager.character.Playable[cardData.charaIndex].moveAct(manager.battleMap.XZtoTile(cardData.activeXZ).transform.position, cardData.targetPos, cardData.targetXZ));
        StartCoroutine(manager.card[cardData.charaIndex].CardDraw());
        yield return new WaitForSeconds(1);
        Destroy(prefab);
    }
    IEnumerator PowerHit(CardData cardData)
    {
        Vector3Int delta = manager.battleMap.XZtoHex(cardData.targetXZ) - manager.battleMap.XZtoHex(cardData.activeXZ);
        BattleTileFunction btf = new BattleTileFunction();
        bool tryN = false;
        for (int i = 0; i < 2; i++)
        {
            try
            {
                Vector3Int v3 = manager.battleMap.HexToXZ(manager.battleMap.XZtoHex(cardData.targetXZ) + delta * (i + 1));
                if(manager.battleMap.XZtoTile(v3).onTileObj == Target.TILE)
                {
                    btf = manager.battleMap.XZtoTile(v3);
                    tryN = true;
                }
                if(manager.battleMap.XZtoTile(v3).onTileObj == Target.OBSTACLE)
                {
                    break;
                }
            }
            catch {
                break;
            }
        }

        GameObject prefab = manager.InstantiateP(GuardEffect[5]);
        prefab.transform.position = manager.battleMap.XZtoTile(cardData.activeXZ).transform.position;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-4, cardData.charaIndex));
        StartCoroutine(manager.camera.Shake(1, 1.5f));
        if (tryN)
        {
            StartCoroutine(cardData.target[0].GetComponent<EnemyInterface>().MoveAct(manager.battleMap.XZtoTile(cardData.targetXZ).transform.position, btf.transform.position, btf.tileXZIndex));
        }
        yield return new WaitForSeconds(0.3f);
        Destroy(prefab);
    }
    IEnumerator Rush(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[6]);
        prefab.transform.SetParent(manager.character.Playable[cardData.charaIndex].transform);
        prefab.transform.localPosition = Vector3.zero;
        int damage = manager.battleMap.Distance(cardData.activeXZ, cardData.targetXZ) + 1;
        yield return StartCoroutine(manager.character.Playable[cardData.charaIndex].moveAct(manager.battleMap.XZtoTile(cardData.activeXZ).transform.position, cardData.targetPos, cardData.targetXZ));
        Destroy(prefab);

        GameObject temp = manager.battleMap.FindTarget(cardData.targetXZ, Target.ENEMY);
        if (temp != null)
        {
            GameObject prefab2 = manager.InstantiateP(GuardEffect[7]);
            prefab2.transform.position = cardData.targetPos;
            temp.GetComponent<HpFunc>().SetHp(DamageCalc(-damage, cardData.charaIndex));
            StartCoroutine(manager.camera.Shake(0.7f, damage * 0.4f));
            yield return new WaitForSeconds(1);
            Destroy(prefab2);
        }

    }
    IEnumerator Protect(CardData cardData)
    {
        GameObject temp = manager.battleMap.FindTarget(cardData.targetXZ, Target.TILE);
        if (temp != null)
        {
            yield return StartCoroutine(manager.character.Playable[cardData.charaIndex].moveAct(manager.battleMap.XZtoTile(cardData.activeXZ).transform.position, temp.transform.position, temp.GetComponent<BattleTileFunction>().tileXZIndex));

            GameObject prefab1 = manager.InstantiateP(GuardEffect[8]);
            prefab1.transform.position = cardData.targetPos;
            GameObject prefab2 = manager.InstantiateP(GuardEffect[8]);
            prefab2.transform.position = temp.transform.position;
            manager.character.Playable[cardData.charaIndex].GetComponent<HpFunc>().SetDp(4);
            cardData.target[0].GetComponent<HpFunc>().SetDp(4);
            yield return new WaitForSeconds(1);
            Destroy(prefab1);
            Destroy(prefab2);
        }
    }
    IEnumerator RocketGrap(CardData cardData)
    {
        Vector3Int delta =  manager.battleMap.XZtoHex(cardData.activeXZ) - manager.battleMap.XZtoHex(cardData.targetXZ);
        int distance = manager.battleMap.Distance(cardData.targetXZ, cardData.activeXZ);
        delta = new Vector3Int(delta.x / distance, delta.y / distance, delta.z / distance);
        BattleTileFunction btf = new BattleTileFunction();
        bool tryN = false;
        for (int i = 0; i < 10; i++)
        {
            try
            {
                Vector3Int v3 = manager.battleMap.HexToXZ(manager.battleMap.XZtoHex(cardData.targetXZ) + delta * (i + 1));
                if (manager.battleMap.XZtoTile(v3).onTileObj == Target.TILE)
                {
                    btf = manager.battleMap.XZtoTile(v3);
                    tryN = true;
                }
                else if (manager.battleMap.XZtoTile(v3).onTileObj == Target.OBSTACLE)
                {
                    break;
                }
                if (manager.battleMap.Distance(cardData.activeXZ, v3) == 1)
                {
                    break;
                }
            }
            catch{ }
        }

        GameObject prefab = manager.InstantiateP(GuardEffect[9]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-1, cardData.charaIndex));
        yield return new WaitForSeconds(0.8f);
        Destroy(prefab);
        if (tryN)
        {
            yield return StartCoroutine(cardData.target[0].GetComponent<EnemyInterface>().MoveAct(manager.battleMap.XZtoTile(cardData.targetXZ).transform.position, btf.transform.position, btf.tileXZIndex));
        }
    }
    IEnumerator Counter(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[10]);
        prefab.transform.position = cardData.targetPos;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-3, cardData.charaIndex));
        yield return new WaitForSeconds(1);
        Destroy(prefab);
    }
    IEnumerator IronWall(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[11]);
        prefab.transform.position = manager.battleMap.XZtoTile(cardData.activeXZ).transform.position;
        manager.character.Playable[cardData.charaIndex].GetComponent<HpFunc>().SetDp(6);
        yield return new WaitForSeconds(1.5f);
        Destroy(prefab);
    }
    IEnumerator Excute(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[12]);
        prefab.transform.position = manager.battleMap.XZtoTile(cardData.activeXZ).transform.position;
        cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-3, cardData.charaIndex));
        yield return new WaitForSeconds(0.5f);
        Destroy(prefab);
        if (cardData.target[0].GetComponent<HpFunc>().Hp <= 8)
        {
            GameObject prefab2 = manager.InstantiateP(GuardEffect[14]);
            prefab2.transform.position = cardData.targetPos;
            yield return new WaitForSeconds(2f);
            Destroy(prefab2);

            GameObject prefab3 = manager.InstantiateP(GuardEffect[15]);
            prefab3.transform.position = cardData.targetPos;
            cardData.target[0].GetComponent<HpFunc>().SetHp(DamageCalc(-99, cardData.charaIndex), true);
            StartCoroutine(manager.camera.Shake(1, 4));
            yield return new WaitForSeconds(1);
            Destroy(prefab3);
        }
    }
    IEnumerator Unbreakable(CardData cardData)
    {
        StartCoroutine(AfterUnbreak(cardData));
        yield return new WaitForSeconds(1);
    }
    IEnumerator AfterUnbreak(CardData cardData)
    {
        GameObject prefab = manager.InstantiateP(GuardEffect[13]);
        prefab.transform.SetParent(manager.character.Playable[cardData.charaIndex].transform);
        prefab.transform.localPosition = Vector3.zero;
        HpFunc hpFunc = manager.character.Playable[cardData.charaIndex].GetComponent<HpFunc>();
        float lastN = 0, nowN = 0;
        int chance = 5;
        while (true)
        {
            nowN = hpFunc.Dp + hpFunc.Hp;
            if (lastN != nowN)
            {
                chance--;
                if (hpFunc.Hp <= 0)
                {
                    int heal = 1 - hpFunc.Hp;
                    hpFunc.SetHp(heal, true);
                }
                if (chance == 0)
                    break;
            }
            lastN = hpFunc.Dp + hpFunc.Hp;
            yield return null;
        }
        Destroy(prefab);
    }
    #endregion
}
