using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class WarningMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TimerText;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject rickshaw;
    [SerializeField]
    private GameObject FailState;
    [SerializeField]
    private float StartTime = 3f;
    private float currentTime;

    private bool _timeStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeStarted)
        {
            if(currentTime > 0)
            {
                currentTime-=Time.deltaTime;
                TimerText.text = currentTime.ToString("0.00");
            }
            else
            {
                TimerText.text = "0";
                FailState.SetActive(true);
                Destroy(rickshaw);
            }
        }
    }

    public void StartTimer()
    {
        currentTime = StartTime;
        canvas.enabled = true;
        _timeStarted = true;
    }

    public void EndTimer()
    {
        _timeStarted = false;
        canvas.enabled = false;
        StartTime -= 0.1f;
    }
}
