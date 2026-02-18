using UnityEngine;
using UnityEngine.Events;

public class SpeedChanger : MonoBehaviour
{
    public UnityEvent<float,float> OnChangeSpeed;
    [SerializeField]
    private float targetSpeed = 5f;
    [SerializeField]
    private float smoothTime = 2f;
    private void OnTriggerEnter(Collider other)
    {
        OnChangeSpeed?.Invoke(targetSpeed,smoothTime);
        Debug.Log("EventInvoked");
    }
}
