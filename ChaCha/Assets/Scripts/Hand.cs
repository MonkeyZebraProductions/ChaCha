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
    private InputAction grabAction,leftShiftAction,rightShiftAction;

    [Header("Controls")]
    [SerializeField]
    private string grabActionName = "Left Grab";
    [SerializeField]
    private string actionMap = "Left Hand";

    [Header("Bouncing")]
    [SerializeField]
    private float forceStrength = 1.0f;
    [SerializeField] private float AxisRatio = 5;
    [SerializeField] private float AngleSarpness = 0.3f;

    [Header("Grab")]
    [SerializeField]
    private float GrabGracePeriod = 1f;
    [SerializeField]
    private float grabAngle = 45f;
    [SerializeField]
    private List<char> additionalButtons;
    private List<char> currentAdditionalButtons;
    [SerializeField]
    private float minGrabBindingDelay = 10f;
    [SerializeField]
    private float maxGrabBindingDelay = 15f;
    [SerializeField]
    private float HitFill = 0.025f;
    private char firstModifier;
    private char secondModifier;
    private float? grabButtonPressed;

    private string initialGrabBind;

    public bool Grabbed;
    private bool _gripRelease;

    private int numHits;
    private bool _flailing;


    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI GrabControlText;
    [SerializeField]
    private Slider HoldMeter;
    [SerializeField]
    private TextMeshProUGUI ShiftText;
    [SerializeField]
    private float FillSpeed = 1f;
    [SerializeField]
    private bool leftShift; 

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
        playerInput = transform.parent.GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        playerInput.actions.Disable();
        playerInput.actions.FindActionMap(actionMap).Enable();

        grabAction = playerInput.actions[grabActionName];
        leftShiftAction = playerInput.actions["Shift Left"];
        rightShiftAction = playerInput.actions["Shift Right"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(AddAdditionalBinding());
        GrabControlText.text = grabAction.GetBindingDisplayString(1);
        initialGrabBind = grabAction.GetBindingDisplayString(1);
        rickShawTransform = transform.parent;
        currentAdditionalButtons = additionalButtons;
        firstModifier = currentAdditionalButtons[Random.Range(0, additionalButtons.Count - 1)];
        additionalButtons.Remove(firstModifier);
        secondModifier = currentAdditionalButtons[Random.Range(0, additionalButtons.Count - 1)];
    }

    // Update is called once per frame
    void Update()
    {
        
        float rotationDirection = Vector3.SignedAngle(rickShawTransform.forward,virtualCameraTransform.forward,Vector3.up)*2f;
        if(Grabbed)
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
            
           if(leftShift)
           {
                ShiftText.enabled = rotationDirection < -grabAngle + 10;
           }
           else
           {
                ShiftText.enabled = rotationDirection > grabAngle - 10;
           }

            if (_gripRelease)
            {
                HoldMeter.value += Time.deltaTime * FillSpeed / 4;  
            }

            if(HoldMeter.value >= HoldMeter.maxValue)
            {
                ReleaseGrip();
            }
        }

    }

    private void FixedUpdate()
    {
         transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y,-0.65f);
        if(Grabbed )
        {
            rb.linearVelocity=Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if(Time.time-grabButtonPressed <GrabGracePeriod && other.gameObject.layer == 6)
        {
            Grabbed = true;
            _flailing = false;
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
            //if (rb.linearVelocity.magnitude < forceStrength)
            //{
            //    rb.linearVelocity = rb.linearVelocity.normalized * forceStrength;
            //}

            //if (rb.linearVelocity.magnitude > forceStrength)
            //{
            //    rb.linearVelocity = rb.linearVelocity.normalized * forceStrength;
            //}

            foreach (ContactPoint contactPoint in collision.contacts)
            {
                Vector2 hitNormal = contactPoint.normal;
                float xVelocity;
                float yVelocity;
                if (Mathf.Abs(hitNormal.x) <= 0.1f)
                {
                    if (rb.linearVelocity.x < 0.0f)
                    {
                        xVelocity = -1.0f;
                    }
                    else
                    {
                        xVelocity = 1.0f;
                    }
                    rb.linearVelocity = new Vector2(xVelocity, hitNormal.y * AxisRatio).normalized * forceStrength;
                    
                }
                if (Mathf.Abs(hitNormal.y) <= 0.1f)
                {
                    if (rb.linearVelocity.y < 0.0f)
                    {
                        yVelocity = -1.0f;
                    }
                    else
                    {
                        yVelocity = 1.0f;
                    }
                    rb.linearVelocity = new Vector2(hitNormal.x * AxisRatio, yVelocity).normalized * forceStrength;
                }
                    Debug.Log(hitNormal);

            }
            HoldMeter.value += HitFill * numHits;
            
            numHits++;
        }
        //else
        //{
        //    rb.linearVelocity = Vector3.zero;
        //    Debug.Log("Don't MOVE!");
        //}
        //AddGrabBinding();
    }

    public void AddGrabBindings()
    {
        StartCoroutine(AddAdditionalBinding());
    }

    IEnumerator AddAdditionalBinding()
    {
        yield return new WaitForSeconds(Random.Range(minGrabBindingDelay,maxGrabBindingDelay));
        AddOneModifierGrabBinding(); 
        yield return new WaitForSeconds(Random.Range(minGrabBindingDelay, maxGrabBindingDelay));
        grabAction.ChangeCompositeBinding("OneModifier").Erase();
        AddTwoModifierGrabBinding(initialGrabBind,firstModifier,secondModifier);
    }

    IEnumerator AddRandomBinding()
    {
        if(currentAdditionalButtons.Count<0)
        {
            currentAdditionalButtons = additionalButtons;
        }
        int randomInt= Random.Range(0, 2);
        char newBinding = currentAdditionalButtons[Random.Range(0, additionalButtons.Count - 1)];
        switch (randomInt)
        {
            case 0:
                initialGrabBind = newBinding.ToString();
                break;
            case 1:
                firstModifier = newBinding;
                break;
            case 2: 
                secondModifier = newBinding; 
                break;
        }
        currentAdditionalButtons.Remove(newBinding);
        yield return new WaitForSeconds(Random.Range(minGrabBindingDelay, maxGrabBindingDelay));
        grabAction.ChangeCompositeBinding("TwoModifiers").Erase();
        AddTwoModifierGrabBinding(initialGrabBind, firstModifier, secondModifier);
    }

    void AddOneModifierGrabBinding()
    {
        grabAction.ChangeBindingWithGroup("Keyboard&Mouse").Erase();
        grabAction.AddCompositeBinding("OneModifier").With("Binding", "<Keyboard>/" + initialGrabBind, groups:"Keyboard&Mouse")
                                                     .With("Modifier", "<Keyboard>/" + firstModifier, groups: "Keyboard&Mouse");
        GrabControlText.text = grabAction.GetBindingDisplayString(1);
        
    }
    void AddTwoModifierGrabBinding(string binding, char modifier1, char modifier2)
    {
        grabAction.AddCompositeBinding("TwoModifiers").With("Binding", "<Keyboard>/" + binding, groups: "Keyboard&Mouse")
                                                     .With("Modifier1", "<Keyboard>/" + modifier1, groups: "Keyboard&Mouse")
                                                     .With("Modifier2", "<Keyboard>/" + modifier2, groups: "Keyboard&Mouse");
        GrabControlText.text = grabAction.GetBindingDisplayString(1);
        StartCoroutine(AddRandomBinding());
    }

    void GrabVoid()
    {
        grabButtonPressed = Time.time;
    }

    void GrabCancel()
    {
        if(Grabbed)
        {
            _gripRelease = true;
        }
    }

    void ReleaseGrip()
    {
        _gripRelease = false;
        Grabbed = false;
        rb.linearVelocity = Random.insideUnitCircle.normalized * forceStrength * 2f;
        _flailing = true;
        numHits = 0;
        HoldMeter.value = 0f;
    }

    void OnEnable()
    {

        playerInput.actions.Enable();
        grabAction.started += context => GrabVoid();
        grabAction.canceled += context => GrabCancel();
    }

    void OnDisable()
    {
        playerInput.actions.Disable();
        grabAction.started -= context => GrabVoid();
        grabAction.canceled -= context => GrabCancel();
    }
}
