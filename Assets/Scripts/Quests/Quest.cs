// 파일명: QuestInstance.cs
using System.Collections.Generic;

public enum QuestState { NotStarted, Active, Completed, Failed }

public class Quest
{
    public QuestData Data { get; private set; }
    public QuestState State { get; set; }
    
    // 각 목표별 현재 달성 수치
    public Dictionary<QuestObjective, int> currentProgress;

    public Quest(QuestData data)
    {
        Data = data;
        State = QuestState.Active;
        currentProgress = new Dictionary<QuestObjective, int>();
        
        // 진행도 0으로 초기화
        foreach (var obj in data.objectives)
        {
            currentProgress.Add(obj, 0);
        }
    }
}