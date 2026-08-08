// 파일명: QuestData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Kill Objective", menuName = "Quest/Objectives/Kill")]
public class KillObjective : QuestObjective
{
    public string enemyID;

    public override bool IsCompleted(int currentAmount)
    {
        return currentAmount >= requiredAmount;
    }
}