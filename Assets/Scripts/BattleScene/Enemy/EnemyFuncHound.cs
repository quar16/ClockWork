using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFuncHound : MonoBehaviour, EnemyInterface
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
        public bool[] isSkill;//스킬의 발동 가능 여부

        public int attackRange;//평타 거리
        public int attackDamage;//평타 공격력

        public int moveIndex;
        public int moveSpeed;
    }
    public EnemyData enemyData = new EnemyData
    {
        name = "하운드",//몬스터 이름
        text = "적을 추격하기 위한 사냥개",//몬스터 설명

        Hp = 6,//초기 체력
        def = 0,//방어력

        term = new int[] { 8, 10, 10, 5, 7 },//각 스킬을 쓴 뒤의 후딜
        isSkill = new bool[] { false, false, false },//스킬의 발동 여부
        coolTime = new int[] { 30, 10, 20 },//각 스킬의 재사용 쿨타임

        attackRange = 2,//평타 거리
        attackDamage = 1,//평타 공격력

        moveIndex = 1,
        moveSpeed = 2
    };
    bool canMove = true;
    public void SetMove(bool B)
    {
        canMove = B;
    }
    public int GetType()
    {
        return 0;
    }
    public void StartAI()
    {
        hpFunc.FirstSetHp(enemyData.Hp, enemyData.def);
        StartCoroutine(BattleAI());
        StartCoroutine(OneSecUpdate());
    }
    public IEnumerator OneSecUpdate()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            if (enemyData.coolTime[i] > 0)
                enemyData.coolTime[i]--;
        }
    }

    public IEnumerator BattleAI()
    {
        yield return new WaitForSeconds(Random.Range(3, 5.0f));
        //게임 시작 첫 대기시간

        while (isAlive)
        {
            yield return new WaitForSeconds(Random.Range(2.0f, 5.5f));
            ActiveType activeType = SetActiveType();
            //다음 행동을 지정
            NextMoveUIs[(int)activeType].SetActive(true);
            //자신에게 다음 행동 표시, 혹은 저 함수에서 하려나?

            yield return new WaitForSeconds(1);
            //유저가 반응할 시간 1초

            yield return new WaitWhile(() => DataSave.Stop);
            yield return StartCoroutine(Active(activeType));
            //행동을 대기열에 등록하고 알아서 실행

            yield return new WaitForSeconds(enemyData.term[(int)activeType] / 2.0f);
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

        for (int i = 0; i < 3; i++)
        {
            if (enemyData.isSkill[i] && enemyData.coolTime[i] == 0)
            {
                index = i;
                break;
            }
        }

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
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.MOVE, 3);
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
            GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.ATTACK, 3);
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
    public IEnumerator Skill1()
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
            int myOrder = manager.flow.GetOrderNumber();//자신의 오더 순서를 받는다.
            GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.SKILL, 3); //오더 UI를 소환하고 가져온다.
            yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);//자신의 오더 순서까지 기다림
            if (isAlive)
            {

            }
            //주변 두칸의 모든 플레이어에게 2데미지

            manager.flow.NextOrder();//다음 순서의 오더를 부른다.
            Destroy(OIC);//자신의 처리가 끝나면 오더 ui를 파괴
        }
    }
    public IEnumerator Skill2()
    {
        Debug.Log("skill 2");
        yield return new WaitForSeconds(1);
        //주변 바닥중 한곳에 불을 피운다
        //불은 지나가는 대상에게 5의 피해를 준다
        //버프의 영향을 받음
    }
    public IEnumerator Skill3()
    {
        Debug.Log("skill 3");
        yield return new WaitForSeconds(1);
        //20초간 공격력 1증가
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
