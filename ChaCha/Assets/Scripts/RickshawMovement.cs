using System.Collections;
using UnityEngine;

public class RickshawMovement : MonoBehaviour
{

    private Rigidbody rb;

    [SerializeField] private float torque = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(AddTorqueInstantly());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AddTorqueInstantly()
    {
        yield return new WaitForSeconds(3);

        rb.rotation = Quaternion.Euler(0, 100, 0);
    }
}
