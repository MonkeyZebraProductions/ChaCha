using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

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

            if(_gameStarted && !_inDanger && !LeftHand.Grabbed && !RightHand.Grabbed)
            {
                DangerEvent?.Invoke();
                _inDanger = true;
            }

            if (_gameStarted && _inDanger && (LeftHand.Grabbed || RightHand.Grabbed))
            {
                RegrabEvent?.Invoke();
                _inDanger = false;
            }
        }
    }

    public void GameFail()
    {

    }
}
