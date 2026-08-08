// 파일명: QuestData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea] public string description;
    
    [SerializeReference] 
    public List<QuestObjective> objectives = new List<QuestObjective>();
    
    public int goldReward;
}