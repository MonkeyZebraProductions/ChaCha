using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UI;

public class SpeedController : MonoBehaviour
{
    private bool changeSpeed;
    private float targetspeed,startTime,t,duration,currentSpeed,maxSpeed;
    private float elapsedTime;
    [SerializeField]
    private Slider DestinationMeter;
    //[SerializeField]
    //private SplineContainer rickshawSpline;
    //[SerializeField]
    //private float StartSpeed=20;
    private SplineAnimate rickshawSpline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rickshawSpline = GetComponent<SplineAnimate>();
        currentSpeed=rickshawSpline.MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(DestinationMeter != null)
        {
            DestinationMeter.value = rickshawSpline.NormalizedTime;
        }
        if (changeSpeed && (rickshawSpline != null))
        {
            if (Mathf.Abs(currentSpeed-targetspeed) >= 0.2f)
            {
                float t = (Time.time - startTime) / duration;
                currentSpeed = Mathf.Lerp(maxSpeed, targetspeed, t);
                UpdatePathSpeed(currentSpeed);
            }
            else
            {
                Debug.Log("ChangeSpeedOver");
                changeSpeed = false;
            }
        }
    }

    private void UpdatePathSpeed(float newSpeed)
    {
        float prevProgress = rickshawSpline.NormalizedTime;
        rickshawSpline.MaxSpeed = newSpeed;
        rickshawSpline.NormalizedTime = prevProgress;
    }
    public void SetSpeedChange(float inSpeed,float inTime)
    {
        targetspeed = inSpeed;
        duration = inTime;
        startTime = Time.time;
        maxSpeed = rickshawSpline.MaxSpeed;
        changeSpeed = true;
        Debug.Log("EventCalled");
    }
}
