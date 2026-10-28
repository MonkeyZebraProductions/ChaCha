using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent AwakeEvent;
    public UnityEvent StartEvent;
    public UnityEvent DangerEvent;
    public UnityEvent RegrabEvent;

    [SerializeField]
    Hand LeftHand;
    [SerializeField]
    Hand RightHand;

    bool _gameStarted,_inDanger = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AwakeEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(LeftHand != null && RightHand != null)
        {
            //If Both Hands are grabbed, start the game
            if (!_gameStarted && LeftHand.Grabbed && RightHand.Grabbed)
            {
                StartEvent?.Invoke();
                _gameStarted = true;
            }

            //If neither hand is grabbed, tell the player they are in danger
            if(_gameStarted && !_inDanger && !LeftHand.Grabbed && !RightHand.Grabbed)
            {
                DangerEvent?.Invoke();
                _inDanger = true;
            }

            //When both hands are grabbed and the player is in danger, take them out of danger
            if (_gameStarted && _inDanger && (LeftHand.Grabbed || RightHand.Grabbed))
            {
                RegrabEvent?.Invoke();
                _inDanger = false;
            }
        }
    }
}
