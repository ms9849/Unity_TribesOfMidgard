using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static Action<string> OnEnemyKilled;
    public static Action<INTERACTION_TYPE> OnInteractioned;
    public static Action<string> OnItemCreated;

    [SerializeField]
    private TextMeshProUGUI QuestTitle;
    [SerializeField]
    private TextMeshProUGUI QuestDescription;

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

        UpdateQuestUI();
    } 

    private void OnEnable()
    {
        OnEnemyKilled += HandleEnemyKilled;
        OnInteractioned += HandleInteractioned;     
    }

    private void OnDisable()
    {
        OnEnemyKilled -= HandleEnemyKilled;
        OnInteractioned -= HandleInteractioned;
    }

    public void AcceptQuest(QuestData data)
    {
        activeQuests.Add(new Quest(data));
        Debug.Log($"퀘스트 수락됨: {data.questName}");
        UpdateQuestUI();
    }

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
                        quest.currentProgress[objective]++;
                        
                        Debug.Log($"{quest.Data.questName} 진행도 업데이트...");
                        
                        UpdateQuestUI();

                        if (killObjective.IsCompleted(quest.currentProgress[objective]))
                        {
                            Debug.Log($"목표 달성: {killObjective.objectiveDescription}");
                            CheckQuestCompletion(quest);
                        }
                    }
                }
            }
        }
    }

    private void HandleInteractioned(INTERACTION_TYPE interactionType)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.State != QuestState.Active) continue;

            foreach (var objective in quest.Data.objectives)
            {
                if (objective is InteractionObjective interactObjective)
                {
                    if (interactObjective.interactionType == interactionType)
                    {
                        quest.currentProgress[objective]++;
                        
                        Debug.Log($"{quest.Data.questName} 진행도 업데이트: " + 
                                  $"{quest.currentProgress[objective]} / {interactObjective.requiredAmount}");

                        UpdateQuestUI();

                        if (interactObjective.IsCompleted(quest.currentProgress[objective]))
                        {
                            Debug.Log($"목표 달성: {interactObjective.objectiveDescription}");
                            CheckQuestCompletion(quest);
                        }
                    }
                }
            }
        }
    }

    private void CheckQuestCompletion(Quest quest)
    {
        bool isAllCompleted = true;
        foreach (var objective in quest.Data.objectives)
        {
            if (!objective.IsCompleted(quest.currentProgress[objective]))
            {
                isAllCompleted = false;
                break;
            }
        }

        if (isAllCompleted)
        {
            quest.State = QuestState.Completed;
            Debug.Log($"퀘스트 완료!: {quest.Data.questName}");
            
            if (quest.Data.nextQuest != null)
            {
                AcceptQuest(quest.Data.nextQuest);
            }
            else 
            {
                UpdateQuestUI();
            }
        }
    }

    public void UpdateQuestUI()
    {
        Quest currentQuest = activeQuests.Find(q => q.State == QuestState.Active);

        if (currentQuest != null)
        {
            if (QuestTitle != null) QuestTitle.text = currentQuest.Data.questName;
            
            if (QuestDescription != null) 
            {
                // 기본 퀘스트 설명 입력
                string fullDescription = currentQuest.Data.description + "\n\n";

                // 현재 퀘스트의 모든 목표를 순회하며 진척도를 문자열에 덧붙임
                foreach (var objective in currentQuest.Data.objectives)
                {
                    int currentProgress = currentQuest.currentProgress[objective];
                    int requiredAmount = objective.requiredAmount;
                    
                    fullDescription += $"- {objective.objectiveDescription} : {currentProgress} / {requiredAmount}\n";
                }

                // 완성된 텍스트를 UI에 적용
                QuestDescription.text = fullDescription;
            }
        }
        else
        {
            if (QuestTitle != null) QuestTitle.text = "퀘스트 없음";
            if (QuestDescription != null) QuestDescription.text = "현재 진행 중인 퀘스트가 없습니다.";
        }
    }
}