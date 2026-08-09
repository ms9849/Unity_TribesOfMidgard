using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea] public string description;
    
    public List<QuestObjective> objectives = new List<QuestObjective>(); 
    
    public int goldReward;

    [Header("연계 퀘스트 설정")]
    [Tooltip("이 퀘스트 완료 시 자동으로 수락할 다음 퀘스트 (없으면 비워둠)")]
    public QuestData nextQuest; 
}