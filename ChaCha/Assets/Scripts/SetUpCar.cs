using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SetUpCar : MonoBehaviour
{
    [SerializeField]
    private GameObject[] CarPrefabs;
    private SplineAnimate carAnimate;
    private bool reverse;

    private void Awake()
    {
        foreach(GameObject car in CarPrefabs)
        {
            car.SetActive(false);
        }
        reverse = Random.value < 0.5;
        carAnimate = GetComponent<SplineAnimate>();
        SplineContainer[] containers = FindObjectsByType<SplineContainer>(FindObjectsSortMode.None);
        foreach (SplineContainer container in containers)
        {
            if(container.gameObject.name == "Spline Road" && !reverse)
            {
                carAnimate.Container = container;
            }

            if (container.gameObject.name == "Spline Road Reverse" && reverse)
            {
                carAnimate.Container = container;
            }
        }
        carAnimate.StartOffset = Random.Range(0.01f, 0.99f);
        CarPrefabs[Random.Range(0, CarPrefabs.Length - 1)].SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
