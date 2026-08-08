using UnityEngine;

// 1. ScriptableObject 상속으로 변경
public abstract class QuestObjective : ScriptableObject
{
    public string objectiveDescription;
    public int requiredAmount;
    public abstract bool IsCompleted(int currentAmount);
}