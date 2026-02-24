using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailState : MonoBehaviour
{
    [SerializeField]
    private Image FailScreen;
    [SerializeField]
    private Sprite[] FailSprites;
    [SerializeField]
    private AudioSource[] Sounds;
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool sucessScreen;
    private void Start()
    {
        if (!sucessScreen)
        {
            StartCoroutine(FailSequence());
        }
    }
    private void Update()
    {
        if(Keyboard.current.rKey.isPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator FailSequence()
    {
        Sounds[0].Play();
        Sounds[1].Play();
        yield return new WaitForSeconds(delay);
        Sounds[2].Play();
        FailScreen.sprite = FailSprites[1];
        yield return new WaitForSeconds(delay);
        FailScreen.sprite = FailSprites[2];
        Sounds[3].Play();
        Sounds[4].Play();
    }
}
