using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    public float time;             // 소환될 시간
    public GameObject monsterPrefab; // 소환할 몬스터 프리팹. 풀링으로 해야하지만.. 이번 프로젝트에선 그 부분을 고려하지 않았으니.
}

//해야할 일
/*
1. 풀링을 적용할 생각이 있다면 풀링.
2. 위그드라실 정보 들고있게끔 처리하기. 
*/

public class Portal : MonoBehaviour
{
[Header("Monster Spawn Data")]
    [SerializeField]
    private List<SpawnData> MonsterSpawnList = new List<SpawnData>();
    private Queue<SpawnData> MonsterSpawnQueue = new Queue<SpawnData>();    
    float AccTime = 0.0f;

    void Start()
    {
        foreach (SpawnData data in MonsterSpawnList)
        {
            MonsterSpawnQueue.Enqueue(data);
        }

        if (GameTimeManager.Instance != null)
        {
            //밤에만 동작하게 처리해.
            GameTimeManager.Instance.OnNightStart += SetPortalOn;
            GameTimeManager.Instance.OnDayStart += SetPortalOff;
        }

        gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnNightStart -= SetPortalOn;
            GameTimeManager.Instance.OnDayStart -= SetPortalOff;
        }
    }

    void Update()
    {
        AccTime += Time.deltaTime;
        SpawnMonsters();
    }

    void SpawnMonsters()
    {
        if(MonsterSpawnQueue.Count > 0 && AccTime >= MonsterSpawnQueue.Peek().time)
        {
            SpawnData monsterData = MonsterSpawnQueue.Dequeue();
            Instantiate(monsterData.monsterPrefab, transform.position, transform.rotation);
            AccTime = 0.0f;
        }
    }
    
    private void SetPortalOn()
    {
        gameObject.SetActive(true);
    }
    private void SetPortalOff()
    {
        gameObject.SetActive(false);
    }
}
