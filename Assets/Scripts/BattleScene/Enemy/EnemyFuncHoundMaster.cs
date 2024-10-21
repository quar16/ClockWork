using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFuncHoundMaster : MonoBehaviour, EnemyInterface
{
    public ref bool GetAlive() { return ref isAlive; }
    public ref AllManager GetManager() { return ref manager; }
    public ref HpFunc GethpFunc() { return ref hpFunc; }
    public ref Vector3Int GetEnemyXZIndex() { return ref EnemyXZIndex; }
    public MonoBehaviour GetMono() { return this; }

    public AllManager manager;
    public HpFunc hpFunc;

    public Break deathPref;
    public GameObject model;
    public GameObject deathEffect;

    public GameObject[] NextMoveUIs;

    public Vector3Int EnemyXZIndex;

    public GameObject[] effect;
    public struct EnemyData
    {
        public string name;//몬스터 이름
        public string text;//몬스터 설명

        public int Hp;//체력
        public int def;//방어력

        public int[] term;//각 스킬을 쓴 뒤의 후딜
        public int[] coolTime;//각 스킬의 재사용 쿨타임
        public int[] realcool;

        public int attackRange;//평타 거리
        public int attackDamage;//평타 공격력

        public int moveIndex;
        public int moveSpeed;
    }
    public EnemyData enemyData = new EnemyData
    {
        name = "하운드 마스터",//몬스터 이름
        text = "하운드와 바르게스트를 관리하는 관리인. 본체를 노리고 접근하면 무슨 일이 일어날 지 모른다.",//몬스터 설명

        Hp = 23,//초기 체력
        def = 10,//방어력

        term = new int[] { 1, 1, 10, 7, 7 },//각 스킬을 쓴 뒤의 후딜
        coolTime = new int[] { 25, 25, 15 },//각 스킬의 재사용 쿨타임
        realcool = new int[] {0,0,0},
        attackRange = 3,//평타 거리
        attackDamage = 2,//평타 공격력

        moveIndex = 3,
        moveSpeed = 2
    };
    bool canMove = true;
    public void SetMove(bool B)
    {
        canMove = B;
    }
    public int GetType()
    {
        return 2;
    }
    public void StartAI()
    {
        hpFunc.FirstSetHp(enemyData.Hp, enemyData.def);
        StartCoroutine(BattleAI());
        StartCoroutine(OneSecUpdate());
    }
    public IEnumerator OneSecUpdate()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 3; i++)
            {
                if (enemyData.realcool[i] > 0)
                    enemyData.realcool[i]--;
            }
        }
    }

    public IEnumerator BattleAI()
    {
        yield return new WaitForSeconds(Random.Range(3,5.0f));
        //게임 시작 첫 대기시간

        while (isAlive)
        {
            yield return new WaitForSeconds(Random.Range(2.0f, 5.5f));
            ActiveType activeType = SetActiveType();
            //다음 행동을 지정
            yield return new WaitForSeconds(enemyData.term[(int)activeType] / 2.0f);
            NextMoveUIs[(int)activeType].SetActive(true);
            //자신에게 다음 행동 표시, 혹은 저 함수에서 하려나?

            yield return new WaitForSeconds(1);
            //유저가 반응할 시간 1초

            yield return new WaitWhile(() => DataSave.Stop);
            yield return StartCoroutine(Active(activeType));
            //행동을 대기열에 등록하고 알아서 실행
            
            NextMoveUIs[(int)activeType].SetActive(false);
            yield return new WaitForSeconds(enemyData.term[(int)activeType] / 2.0f);
            //각 행동에 따른 후딜레이를 가짐
        }
    }

    public ActiveType SetActiveType()
    {
        int index = 4;

        if (FindPlayer(enemyData.attackRange))
        {
            index = 3;
        }

        if (isSkill1() && enemyData.realcool[0] == 0)
            index = 0;
        if (isSkill2() && enemyData.realcool[1] == 0)
            index = 1;
        if (isSkill3() && enemyData.realcool[2] == 0)
            index = 2;

        return (ActiveType)index;
    }


    bool FindPlayer(int range)
    {
        CharacterFunc[] cf = manager.character.Playable;
        for (int i = 0; i < cf.Length; i++)
        {
            if (cf[i].isAlive)
                if (manager.battleMap.Distance(EnemyXZIndex, cf[i].playerXZIndex) <= range)
                {
                    return true;
                }
        }
        return false;
    }
    CharacterFunc FindPlayerS(int range)
    {
        CharacterFunc[] cf = manager.character.Playable;
        for (int i = 0; i < cf.Length; i++)
        {
            if (cf[i].isAlive)
                if (manager.battleMap.Distance(EnemyXZIndex, cf[i].playerXZIndex) <= range)
                {
                    return cf[i];
                }
        }
        return null;
    }

    IEnumerator Active(ActiveType activeType)
    {
        switch (activeType)
        {
            case ActiveType.SkILL1:
                yield return StartCoroutine(Skill1());
                break;
            case ActiveType.SkILL2:
                yield return StartCoroutine(Skill2());
                break;
            case ActiveType.SkILL3:
                yield return StartCoroutine(Skill3());
                break;
            case ActiveType.ATTACK:
                yield return StartCoroutine(Attack());
                break;
            case ActiveType.MOVE:
                yield return StartCoroutine(MoveProcess());
                break;
        }
    }


    #region MOVE
    Vector3Int targetPos;

    IEnumerator MoveProcess()
    {
        if (!canMove)
            yield break;
        Vector3 start = manager.battleMap.XZtoTile(EnemyXZIndex).transform.position;

        Vector3Int[] innerRange = new Vector3Int[63];
        int n = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                try
                {
                    Vector3Int target = manager.battleMap.tiles[i, j].tileXZIndex;
                    int distance = manager.battleMap.HexDistance(EnemyXZIndex, target);
                    if (manager.battleMap.tiles[i, j].onTileObj == EffectManager.Target.TILE)
                        if (distance <= enemyData.moveSpeed)
                        {
                            innerRange[n] = target;
                            n++;
                        }
                }
                catch
                {
                }
            }
        }
        int gap = 20;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (manager.character.Playable[j].isAlive)
                    if (Mathf.Abs(enemyData.moveIndex - manager.battleMap.HexDistance(innerRange[i], manager.character.Playable[j].playerXZIndex)) <= gap)
                    {
                        targetPos = innerRange[i];

                        gap = Mathf.Abs(enemyData.moveIndex - manager.battleMap.HexDistance(innerRange[i], manager.character.Playable[j].playerXZIndex));

                    }
            }
        }

        BattleTileFunction btf = manager.battleMap.XZtoTile(EnemyXZIndex);
        btf.onTileObj = EffectManager.Target.TILE;
        btf.onObj = null;
        EnemyXZIndex = targetPos;
        btf = manager.battleMap.manager.battleMap.XZtoTile(EnemyXZIndex);
        btf.onTileObj = EffectManager.Target.ENEMY;
        btf.onObj = gameObject;
        Vector3 end = manager.battleMap.XZtoTile(EnemyXZIndex).transform.position;



        yield return StartCoroutine(Move(start, end));
    }

    public GameObject endPref;
    IEnumerator Move(Vector3 startPosition, Vector3 target)
    {
        GameObject temp = manager.InstantiateP(endPref);
        temp.transform.position = target;
        LineEnd lineEnd = temp.GetComponent<LineEnd>();
        lineEnd.end = target;
        lineEnd.manager = manager;
        lineEnd.enemyFunc = this;
        lineEnd.setStart();
        int myOrder = manager.flow.GetOrderNumber();
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.MOVE, 5);
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);
        if (isAlive)
        {
            float L = 0;
            while (L <= 1)
            {
                L += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition + Vector3.up * 0.75f, target + Vector3.up * 0.75f, L);
                yield return null;
            }

        }
        manager.flow.NextOrder();
        lineEnd.SetEnd();
        Destroy(OIC);
    }
    public IEnumerator MoveAct(Vector3 startPosition, Vector3 target, Vector3Int targetXZ)
    {
        if (!canMove)
            yield break;
        if (isAlive)
        {
            BattleTileFunction btf = manager.battleMap.XZtoTile(EnemyXZIndex);
            btf.onTileObj = EffectManager.Target.TILE;
            btf.onObj = null;
            EnemyXZIndex = targetXZ;
            btf = manager.battleMap.XZtoTile(EnemyXZIndex);
            btf.onTileObj = EffectManager.Target.ENEMY;
            btf.onObj = gameObject;

            float L = 0;
            float speed = 6;
            while (L <= 1)
            {
                L += speed * Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition + Vector3.up * 0.75f, target + Vector3.up * 0.75f, L);
                yield return null;
            }
        }
    }
    #endregion

    #region ATTACK
    public GameObject attackEnd;
    IEnumerator Attack()
    {
        CharacterFunc cf = FindPlayerS(enemyData.attackRange);
        if (cf != null)
        {
            GameObject temp = manager.InstantiateP(attackEnd);
            Vector3 target = manager.battleMap.XZtoTile(cf.playerXZIndex).transform.position;
            temp.transform.position = target;
            LineEnd lineEnd = temp.GetComponent<LineEnd>();
            lineEnd.end = target;
            lineEnd.manager = manager;
            lineEnd.enemyFunc = this;
            lineEnd.setStart();
            int myOrder = manager.flow.GetOrderNumber();
            GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.ATTACK, 5);
            yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);

            if (isAlive)
            {
                GameObject prefab = manager.InstantiateP(effect[0]);
                prefab.transform.position = target;
                prefab.transform.position += new Vector3(0, 4.5f, 0);
                yield return new WaitForSeconds(0.3f);
                cf.GetComponent<HpFunc>().SetHp(-enemyData.attackDamage);
                yield return new WaitForSeconds(0.3f);
                Destroy(prefab);

            }
            manager.flow.NextOrder();
            lineEnd.SetEnd();
            Destroy(OIC);
        }
    }
    #endregion

    #region SKILL
    int tempH = 0;
    int tempB = 0;
    public bool isSkill1()
    {
        int houndN = 0;
        for(int i = 0; i < manager.enemy.EnemyF.Length; i++)
        {
            if(manager.enemy.EnemyF[i].GetAlive()&& manager.enemy.EnemyF[i].GetType() == 0)
            {
                houndN++;
            }
        }
        houndN += tempH;

        if (houndN < 2)
        {
            return true;
        }
        return false;
    }
    public bool isSkill2()
    {
        int bargN = 0;
        for (int i = 0; i < manager.enemy.EnemyF.Length; i++)
        {
            if (manager.enemy.EnemyF[i].GetAlive() && manager.enemy.EnemyF[i].GetType() == 1)
            {
                bargN++;
            }
        }
        bargN += tempB;
        if (bargN < 1)
        {
            return true;
        }
        return false;
    }
    public bool isSkill3()
    {
        if (FindPlayer(1))
            return true;

        return false;
    }
    bool firstSummon = true;
    public IEnumerator Skill1()
    {
        tempH++;
        int myOrder = manager.flow.GetOrderNumber();//자신의 오더 순서를 받는다.
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.SKILL, 5); //오더 UI를 소환하고 가져온다.
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);//자신의 오더 순서까지 기다림
        GameObject btf = manager.battleMap.FindTarget(EnemyXZIndex, EffectManager.Target.TILE);
        if (isAlive && btf != null)
        {
            Vector3Int v3 = btf.GetComponent<BattleTileFunction>().tileXZIndex;
            GameObject temp = manager.InstantiateP(effect[1]);
            temp.transform.position = manager.battleMap.XZtoTile(v3).transform.position;
            yield return new WaitForSeconds(0.8f);
            Destroy(temp);
            GameObject temp2 = manager.InstantiateP(effect[2]);
            temp2.transform.position = manager.battleMap.XZtoTile(v3).transform.position;
            manager.enemy.CreateEnemy(0, v3);
            yield return new WaitForSeconds(1.2f);
            Destroy(temp2);
            manager.enemy.EnemyF[manager.enemy.EnemyF.Length - 1].StartAI();
            enemyData.realcool[0] = enemyData.coolTime[0];
            if (firstSummon)
            {
                firstSummon = false;
                enemyData.realcool[0] = 0;
            }
        }
        tempH--;
        manager.flow.NextOrder();//다음 순서의 오더를 부른다.
        Destroy(OIC);//자신의 처리가 끝나면 오더 ui를 파괴

    }
    public IEnumerator Skill2()
    {
        tempB++;
        int myOrder = manager.flow.GetOrderNumber();//자신의 오더 순서를 받는다.
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.SKILL, 5); //오더 UI를 소환하고 가져온다.
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);//자신의 오더 순서까지 기다림
        GameObject btf = manager.battleMap.FindTarget(EnemyXZIndex, EffectManager.Target.TILE);
        if (isAlive && btf != null)
        {
            Vector3Int v3 = btf.GetComponent<BattleTileFunction>().tileXZIndex;
            GameObject temp = manager.InstantiateP(effect[1]);
            temp.transform.position = manager.battleMap.XZtoTile(v3).transform.position;
            yield return new WaitForSeconds(0.8f);
            Destroy(temp);
            GameObject temp2 = manager.InstantiateP(effect[2]);
            temp2.transform.position = manager.battleMap.XZtoTile(v3).transform.position;
            manager.enemy.CreateEnemy(1, v3);
            yield return new WaitForSeconds(1.2f);
            Destroy(temp2);
            manager.enemy.EnemyF[manager.enemy.EnemyF.Length - 1].StartAI();
            enemyData.realcool[1] = enemyData.coolTime[1];
        }
        tempB--;
        manager.flow.NextOrder();//다음 순서의 오더를 부른다.
        Destroy(OIC);//자신의 처리가 끝나면 오더 ui를 파괴
    }
    public IEnumerator Skill3()
    {
        CharacterFunc cf = FindPlayerS(1);
        if (cf != null)
        {
            GameObject temp = manager.InstantiateP(attackEnd);
            Vector3 target = manager.battleMap.XZtoTile(cf.playerXZIndex).transform.position;
            temp.transform.position = target;
            LineEnd lineEnd = temp.GetComponent<LineEnd>();
            lineEnd.end = target;
            lineEnd.manager = manager;
            lineEnd.enemyFunc = this;
            lineEnd.setStart();
            int myOrder = manager.flow.GetOrderNumber();
            GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.SKILL, 4);
            yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);
            yield return new WaitForSeconds(0.5f);
            if (isAlive)
            {
                GameObject prefab = manager.InstantiateP(effect[3]);
                prefab.transform.position = target;
                yield return new WaitForSeconds(0.3f);
                cf.GetComponent<HpFunc>().SetHp(-10);
                StartCoroutine(manager.camera.Shake(0.7f));
                yield return new WaitForSeconds(2.5f);
                Destroy(prefab);
                enemyData.realcool[2] = enemyData.coolTime[2];
            }
            manager.flow.NextOrder();
            lineEnd.SetEnd();
            Destroy(OIC);
        }
    }

    #endregion

    public bool isAlive = true;
    public void Death()
    {
        isAlive = false;
        gameObject.layer = 2;
        BattleTileFunction btf = manager.battleMap.XZtoTile(EnemyXZIndex);
        btf.onTileObj = EffectManager.Target.TILE;
        btf.onObj = null;

        model.SetActive(false);
        deathPref.gameObject.SetActive(true);
        StartCoroutine(manager.camera.Shake(1, 0.3f));
        GameObject temp = manager.InstantiateP(deathEffect);
        temp.transform.position = gameObject.transform.position;
        deathPref.StartBreak();
    }
}
