using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public UnityEvent StartEvent;

    [SerializeField]
    Hand LeftHand;
    [SerializeField]
    Hand RightHand;

    bool _gameStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(LeftHand != null && RightHand != null)
        {
            if (!_gameStarted && LeftHand.Grabbed && RightHand.Grabbed)
            {
                StartEvent?.Invoke();
                _gameStarted = true;
            }
        }
    }
}
