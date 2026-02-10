using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;
    private InputAction moveAction,grabAction,leftShiftAction,rightShiftAction;

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
    [SerializeField]
    private float grabAngle = 45f;
    [SerializeField]
    private List<char> additionalButtons;
    private char firstModifier;
    private char secondModifier;
    private float? grabButtonPressed;

    private string initialGrabBind;

    bool _grabbed;
    private bool _gripRelease;

    private int numHits;
    private bool _flailing;


    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI GrabControlText;
    [SerializeField]
    private Slider HoldMeter;


    [SerializeField]
    private float FillSpeed = 1f;

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
        leftShiftAction = playerInput.actions["Shift Left"];
        rightShiftAction = playerInput.actions["Shift Right"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AddAdditionalBinding());
        GrabControlText.text = grabAction.GetBindingDisplayString(1);
        initialGrabBind = grabAction.GetBindingDisplayString(1);
        rickShawTransform = transform.parent;
        firstModifier = additionalButtons[Random.Range(0, additionalButtons.Count - 1)];
        additionalButtons.Remove(firstModifier);
        secondModifier = additionalButtons[Random.Range(0, additionalButtons.Count - 1)];
    }

    // Update is called once per frame
    void Update()
    {
        
        float rotationDirection = Vector3.SignedAngle(rickShawTransform.forward,virtualCameraTransform.forward,Vector3.up)*2f;
        if(_grabbed)
        {
            handSprite.localRotation = Quaternion.Euler(0f,0f, rotationDirection);

            if(Mathf.Abs(rotationDirection) >grabAngle)
            {
                if (rotationDirection > 0f && leftShiftAction.IsPressed() || rotationDirection < 0f && rightShiftAction.IsPressed())
                {

                }
                else if (!leftShiftAction.IsPressed() && !rightShiftAction.IsPressed())
                {
                    HoldMeter.value += Time.deltaTime * FillSpeed;  
                }
            }
            else if (Mathf.Abs(rotationDirection) > grabAngle/2 && (rotationDirection < 0f && leftShiftAction.IsPressed() || rotationDirection > 0f && rightShiftAction.IsPressed()))
            {
                HoldMeter.value += Time.deltaTime * FillSpeed;
                
            }
            if(_gripRelease)
            {
                HoldMeter.value += Time.deltaTime * FillSpeed/5;
            }

            if(HoldMeter.value >= HoldMeter.maxValue)
            {
                ReleaseGrip();
            }
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
            _gripRelease = false;
            handSprite.localRotation = Quaternion.identity;
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
        AddOneModifierGrabBinding(); 
        yield return new WaitForSeconds(6f);
        AddTwoModifierGrabBinding();
    }

    void AddOneModifierGrabBinding()
    {
        grabAction.ChangeBindingWithGroup("Keyboard&Mouse").Erase();
        grabAction.AddCompositeBinding("OneModifier").With("Binding", "<Keyboard>/" + initialGrabBind, groups:"Keyboard&Mouse")
                                                     .With("Modifier", "<Keyboard>/" + firstModifier, groups: "Keyboard&Mouse");
        GrabControlText.text = grabAction.GetBindingDisplayString(1);
        
    }
    void AddTwoModifierGrabBinding()
    {
        grabAction.ChangeCompositeBinding("OneModifier").Erase();
        grabAction.AddCompositeBinding("TwoModifiers").With("Binding", "<Keyboard>/" + initialGrabBind, groups: "Keyboard&Mouse")
                                                     .With("Modifier1", "<Keyboard>/" + firstModifier, groups: "Keyboard&Mouse")
                                                     .With("Modifier2", "<Keyboard>/" + secondModifier, groups: "Keyboard&Mouse");
        GrabControlText.text = grabAction.GetBindingDisplayString(1);

    }

    void GrabVoid()
    {
        grabButtonPressed = Time.time;
    }

    void GrabCancel()
    {
        if(_grabbed)
        {
            _gripRelease = true;
        }
    }

    void ReleaseGrip()
    {
        _gripRelease = false;
        _grabbed = false;
        rb.linearVelocity = Random.insideUnitCircle.normalized * forceStrength * 4f;
        _flailing = true;
        numHits = 0;
        HoldMeter.value = 0f;
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
