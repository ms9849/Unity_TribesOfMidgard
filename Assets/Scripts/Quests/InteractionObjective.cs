// 파일명: QuestData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Interaction Objective", menuName = "Quest/Objectives/Interaction")]
public class InteractionObjective : QuestObjective
{
    public string interactionID;

    public override bool IsCompleted(int currentAmount)
    {
        return currentAmount >= requiredAmount;
    }
}