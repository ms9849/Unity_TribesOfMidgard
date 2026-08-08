// 파일명: GameEvents.cs
using UnityEngine;
using System;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    // 몬스터 처치.
    public static Action<string> OnEnemyKilled;
    // 상호작용.
    public static Action<string> OnInteractioned;
    // 아이템 제작
    public static Action<string> OnItemCreated;

    public static QuestManager Instance { get; private set; }

    public List<Quest> activeQuests = new List<Quest>();

    [Header("게임 시작 시 자동 수락할 퀘스트 목록")]
    public List<QuestData> startingQuests;
    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (startingQuests != null)
        {
            foreach (var questData in startingQuests)
            {
                AcceptQuest(questData);
            }
        }
    } 

    private void OnEnable()
    {
// 선언한 모든 이벤트를 구독
        OnEnemyKilled += HandleEnemyKilled;
        OnInteractioned += HandleInteractioned;     
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위한 이벤트 구독 해제
        OnEnemyKilled -= HandleEnemyKilled;
    }

    // 퀘스트 수락 시 호출할 메서드 (예시)
    public void AcceptQuest(QuestData data)
    {
        activeQuests.Add(new Quest(data));
        Debug.Log($"퀘스트 수락됨: {data.questName}");
    }

    // 몬스터 사망 이벤트 수신 시 처리 로직
    private void HandleEnemyKilled(string killedEnemyID)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.State != QuestState.Active) continue;

            foreach (var objective in quest.Data.objectives)
            {
                if (objective is KillObjective killObjective)
                {
                    if (killObjective.enemyID == killedEnemyID)
                    {
                        // 진척도 증가
                        quest.currentProgress[objective]++;
                        
                        Debug.Log($"{quest.Data.questName} 진행도 업데이트: " + 
                                  $"{quest.currentProgress[objective]} / {killObjective.requiredAmount}");

                        // 목표 달성 체크
                        if (killObjective.IsCompleted(quest.currentProgress[objective]))
                        {
                            Debug.Log($"목표 달성: {killObjective.objectiveDescription}");
                            // TODO: 모든 목표가 달성되었는지 확인 후 quest.State를 Completed로 변경
                        }
                    }
                }
            }
        }
    }
    // 2. 상호작용 이벤트 처리 (새로 추가)
    private void HandleInteractioned(string objectID)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.State != QuestState.Active) continue;

            foreach (var objective in quest.Data.objectives)
            {
                if (objective is InteractionObjective interactObjective)
                {
                    if (interactObjective.interactionID == objectID)
                    {
                        // 진척도 증가
                        quest.currentProgress[objective]++;
                        
                        Debug.Log($"{quest.Data.questName} 진행도 업데이트: " + 
                                  $"{quest.currentProgress[objective]} / {interactObjective.requiredAmount}");

                        // 목표 달성 체크
                        if (interactObjective.IsCompleted(quest.currentProgress[objective]))
                        {
                            Debug.Log($"목표 달성: {interactObjective.objectiveDescription}");
                            // TODO: 모든 목표가 달성되었는지 확인 후 quest.State를 Completed로 변경
                        }
                    }
                }
            }
        }
    }
}