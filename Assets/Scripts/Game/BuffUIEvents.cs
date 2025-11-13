using System;

public static class BuffUIEvents
{
    // [변경가능] 시작 토스트: label(문구), seconds(선택: 남은시간 표기용)
    public static event Action<string, float?> OnBuffStarted;
    // [변경가능] 종료 토스트: label(문구)
    public static event Action<string> OnBuffEnded;

    // 호출 헬퍼
    public static void RaiseStarted(string label, float? seconds = null)
        => OnBuffStarted?.Invoke(label, seconds);

    public static void RaiseEnded(string label)
        => OnBuffEnded?.Invoke(label);
}
