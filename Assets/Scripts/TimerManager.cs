using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TimerManager : MonoBehaviour
{
    [FormerlySerializedAs("rso_Timer")]
    [SerializeField] private RSO_Timer rsoTimer;
    [SerializeField] private TextMeshProUGUI timerText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rsoTimer.Reset(60f * 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (rsoTimer == null || timerText == null) return;

        // decrement timer
        rsoTimer.TimeRemaining -= Time.deltaTime;
        // clamp TimeRemaining to non-negative value to avoid negative drift
        rsoTimer.TimeRemaining = Mathf.Max(0f, rsoTimer.TimeRemaining);
        
        // use clamped remaining time for display
        float remaining = rsoTimer.TimeRemaining;

        // compute minutes and seconds
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        // format as "00M 00S"
        timerText.text = string.Format($"{minutes}M {seconds}S");
    }
}
