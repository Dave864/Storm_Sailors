using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderhead : MonoBehaviour
{
    // Enumerator for the states the thunderhead can be in
    public enum ThunderheadState { DEFAULT, HELD, LAUNCHED, STORMFRONT }
    private ThunderheadState curState = ThunderheadState.DEFAULT;

    // Flag indicating if the thunderhead is being held
    [HideInInspector] public bool IsHeld
    {
        get { return curState == ThunderheadState.HELD; }
        set { curState = (value) ? ThunderheadState.DEFAULT : curState; }
    }

    // Strength level of the thunderhead gale
    public int GaleLvl { get; set; }

    // Direction the thunderhead blows the wind
    private Vector3 galeVector = new Vector3(0, 0, 0);
    public Vector3 GaleVector
    {
        get { return galeVector; }
        set { galeVector = value; }
    }

    // Parameters of thunderhead when in LAUNCHED state
    private Vector3 launchDirection = Vector2.zero;
    private float launchEffectRadius;
    [SerializeField] private float launchVelocity = 20;

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject wizardObject;
    private LineRenderer launchEffectArea;

    // Initializes the thuderhead
    private void Awake()
    {
        // Initialize gale vector
        compassCenter = GameObject.Find("Compass Center");
        if (!compassCenter)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }
        galeVector = compassCenter.transform.position - transform.position;
        GaleLvl = 1;

        // Set the launch effect radius
        CapsuleCollider collisionShape = GetComponent<CapsuleCollider>();
        if (collisionShape)
        {
            launchEffectRadius = collisionShape.radius;
        }
        else
        {
            Debug.LogError("No Capsule Collision Shape attached to thunderhead", collisionShape);
        }

        // Get the reference to the thunderhead's line renderer
        launchEffectArea = GetComponent<LineRenderer>();
        if (!launchEffectArea)
        {
            Debug.LogError("No Line Renderer attached to thunderhead", launchEffectArea);
        }
        launchEffectArea.enabled = false;

        // Get the reference to the wizard object
        wizardObject = GameObject.Find("Wizard Object");
        if (!wizardObject)
        {
            Debug.LogError("Wizard object not found");
        }
    }

    // Called once per frame
    private void Update()
    {
        switch (curState)
        {
            case ThunderheadState.HELD:
                transform.LookAt(compassCenter.transform.position, Vector3.up);
                break;
            case ThunderheadState.LAUNCHED:
                transform.position += launchDirection * launchVelocity * Time.deltaTime;
                break;
            case ThunderheadState.STORMFRONT:
                break;
            default:
                break;
        }
    }

    // Draw GUI elements
    private void OnGUI()
    {
        // Display label indicating cloud gale level
        Vector3 thunderheadScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 labelPos = new Vector2(thunderheadScreenPos.x, Camera.main.pixelHeight - thunderheadScreenPos.y);
        Vector2 labelSize = new Vector2(50, 50);
        GUI.Label(new Rect(labelPos, labelSize), GaleLvl.ToString());
    }

    // Destroy thunderhead when it is no longer visible
    private void OnBecameInvisible()
    {
        Destroy(gameObject, 0.2f);
    }

    // Convert gale strength level to some float value
    public float CloudStrength()
    {
        return GaleLvl; // TODO: figure out strength calculation
    }

    // Merge thunderheads
    // The thunderhead that is passed into the function is destroyed
    public void Merge(GameObject thunderheadToMerge)
    {
        Thunderhead thunderheadObj = thunderheadToMerge.GetComponent<Thunderhead>();
        if(thunderheadObj)
        {
            GaleLvl += thunderheadObj.GaleLvl;
            Destroy(thunderheadToMerge);
        }
    }

    // Launch thunderhead
    public void Launch(Vector3 launchVector)
    {
        curState = ThunderheadState.LAUNCHED;
        launchDirection = launchVector.normalized;

        // Set the size of the launch effect area indicator
        Vector3 positionNode = new Vector3(0, 0.1f - transform.position.y , 0);
        Vector2 positionOnCircle;
        float angleIncrement = 360f / (launchEffectArea.positionCount - 1);
        for (int pos = 0; pos < launchEffectArea.positionCount; pos++)
        {
            // Calculate the values of position
            positionOnCircle = (Quaternion.Euler(0, 0, angleIncrement * pos) * new Vector2(0, 1)).normalized;
            positionOnCircle *= launchEffectRadius;
            positionNode.x = positionOnCircle.x;
            positionNode.z = positionOnCircle.y;

            launchEffectArea.SetPosition(pos, positionNode);
        }

        launchEffectArea.enabled = true;
        transform.SetParent(null);
    }

    // Change thunderhead to stormfront
    public void StormFront(float stormFrontRange)
    {
        curState = ThunderheadState.STORMFRONT;
        transform.GetComponent<CapsuleCollider>().radius = stormFrontRange;
    }

    // Handle collisions
    public void OnTriggerEnter(Collider other)
    {
        switch (curState)
        {
            case ThunderheadState.LAUNCHED:
                if (other.CompareTag("Obstacle"))
                {
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
                break;
            case ThunderheadState.STORMFRONT:
                if (wizardObject.GetComponent<Wizard>().CurMode == Wizard.Mode.GALE && other.CompareTag("Enemy"))
                {
                    Destroy(other.gameObject);
                }
                break;
            default:
                break;
        }
    }
}
