// 파일명: QuestData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Interaction Objective", menuName = "Quest/Objectives/Interaction")]
public class InteractionObjective : QuestObjective
{
    public INTERACTION_TYPE interactionType;

    public override bool IsCompleted(int currentAmount)
    {
        return currentAmount >= requiredAmount;
    }
}