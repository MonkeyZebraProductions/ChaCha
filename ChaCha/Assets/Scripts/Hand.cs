using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Hand : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;
    private InputAction moveAction,grabAction;

    [Header("Controls")]
    [SerializeField]
    private string moveActionName = "LeftMove";
    [SerializeField]
    private string grabActionName = "Left Grab";

    [SerializeField]
    private float forceStrength = 1.0f;

    [Header("Grab")]
    [SerializeField]
    private float GrabGracePeriod = 1f;
    private float? grabButtonPressed;

    private string initialGrabBind;

    bool _grabbed;

    private int numHits;
    private bool _flailing;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI GrabControlText;

    [Header("Hand Visuals")]
    [SerializeField]
    private Transform handSprite;
    [SerializeField]
    private Transform virtualCameraTransform;
    private Transform rickShawTransform;

    [SerializeField]
    private float minRotateDelta;


    [SerializeField]
    private LayerMask RickshawLayer;

    private void Awake()
    {
        //Assigns variables at start of Runtime
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        moveAction = playerInput.actions[moveActionName];
        grabAction = playerInput.actions[grabActionName];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AddAdditionalBinding());
        GrabControlText.text = grabAction.GetBindingDisplayString(1);
        initialGrabBind = grabAction.GetBindingDisplayString(1);
        rickShawTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        float handZRotation = Quaternion.Angle(rickShawTransform.rotation,virtualCameraTransform.rotation);
        float rotationDirection = Vector3.SignedAngle(rickShawTransform.forward,virtualCameraTransform.forward,Vector3.up);
        Debug.Log(rotationDirection);
        if(_grabbed)
        {
            handSprite.localRotation = Quaternion.Euler(0f,0f,handZRotation*(rotationDirection<0? 1:-1));
        }
    }

    private void FixedUpdate()
    {
        if (moveAction != null && !_grabbed) 
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            Vector3 moveVector3 = new Vector3(moveInput.x, moveInput.y, 0);

            transform.localPosition+=moveVector3.normalized * Time.fixedDeltaTime;
            //rb.AddRelativeForce(moveVector3.normalized*forceStrength);
        }
         transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y,-0.65f);
        if(_grabbed )
        {
            rb.linearVelocity=Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if(Time.time-grabButtonPressed <GrabGracePeriod && other.gameObject.layer == 6)
        {
            _grabbed = true;
            grabButtonPressed = null;
            //rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_flailing)
        {
            foreach (ContactPoint contactPoint in collision.contacts)
            {
                Vector3 hitNormal = contactPoint.normal;
                //if (rb2D.linearVelocity.y < 0.0f)
                //{
                //    yVelocity = -1.0f;
                //}
                //else
                //{
                //    yVelocity = 1.0f;
                //}
                rb.linearVelocity = new Vector2(hitNormal.x, hitNormal.y).normalized * forceStrength * Mathf.Pow(0.8f, numHits);
                numHits++;
            }
            if (numHits > 5)
            {
                numHits = 0;
                _flailing = false;
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
        //AddGrabBinding();
    }

    IEnumerator AddAdditionalBinding()
    {
        yield return new WaitForSeconds(5f);
        AddGrabBinding();
        yield return new WaitForSeconds(0.1f);
        
    }

    void AddGrabBinding()
    {
        grabAction.ChangeBindingWithGroup("Keyboard&Mouse").Erase();
        grabAction.AddCompositeBinding("OneModifier").With("Binding", "<Keyboard>/" + initialGrabBind, groups:"Keyboard&Mouse")
                                                     .With("Modifier", "<Keyboard>/Z", groups: "Keyboard&Mouse");
        GrabControlText.text = grabAction.GetBindingDisplayString(4);
        
    }

    void GrabVoid()
    {
        grabButtonPressed = Time.time;
    }

    void GrabCancel()
    {
        _grabbed = false;
        rb.linearVelocity = Random.insideUnitCircle.normalized * forceStrength*4f;
        _flailing = true;
    }

    void OnEnable()
    {
        grabAction.started += context => GrabVoid();
        grabAction.canceled += context => GrabCancel();
        //playerInput.Enable();
    }

    void OnDisable()
    {
        grabAction.started -= context => GrabVoid();
        grabAction.canceled -= context => GrabCancel();
    }
}
