using UnityEngine;

// 밤이 시작되는 시점을 회전각 0으로 두고, 하루 주기마다 한 바퀴(360도) 회전합니다.
public class RotateDial : MonoBehaviour
{
    [Tooltip("하루 진행도를 가져올 GameTimeManager")]
    public GameTimeManager GameTimeSource;

    void Reset()
    {
        GameTimeSource = FindObjectOfType<GameTimeManager>();
    }

    void Update()
    {
        if (GameTimeSource == null)
            return;

        //하루의 진행도에 따라 회전 처리.
        // 0이라면 회전 0, 시계 방향으로 쭉 회전.
        float ProgressSinceNightStart = Mathf.Repeat(GameTimeSource.TimeOfDay - GameTimeSource.NightStart, 1f);
        transform.localRotation = Quaternion.Euler(0f, 0f, -ProgressSinceNightStart * 360f);
    }
}
