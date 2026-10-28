using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SetUpCar : MonoBehaviour
{
    [SerializeField]
    private GameObject[] CarPrefabs;
    private SplineAnimate carAnimate;
    private bool reverse;

    //When Car Prefab is dragged into the scene, set it up
    private void Awake()
    {
        foreach(GameObject car in CarPrefabs)
        {
            car.SetActive(false);
        }
        reverse = Random.value < 0.5;
        carAnimate = GetComponent<SplineAnimate>();

        //Assign Spline to Car
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

        //Set Random Start Point
        carAnimate.StartOffset = Random.Range(0.01f, 0.99f);

        //Set Random Visuals
        CarPrefabs[Random.Range(0, CarPrefabs.Length - 1)].SetActive(true);
    }
   
}
