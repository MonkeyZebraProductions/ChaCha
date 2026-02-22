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
        SpeedController speedController = other.transform.root.gameObject.GetComponent<SpeedController>();
        if (speedController)
        {
            speedController.SetSpeedChange(targetSpeed, smoothTime);
            Debug.Log(other.transform.root.name);
        }
        OnChangeSpeed?.Invoke(targetSpeed,smoothTime);
        Debug.Log("EventInvoked");
        Destroy(gameObject);
    }
}
