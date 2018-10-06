using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderhead : MonoBehaviour
{
    // Enumerator for the states the thunderhead can be in
    public enum ThunderheadState { DEFAULT, HELD, LAUNCHED, STORMFRONT, CALLSTRIKE }
    private ThunderheadState curState = ThunderheadState.DEFAULT;

    // Flag indicating if the thunderhead is being held
    [HideInInspector] public bool IsHeld
    {
        get { return curState == ThunderheadState.HELD; }
        set { curState = (value) ? ThunderheadState.DEFAULT : curState; }
    }

    // Strength level of the thunderhead gale
    private float cloudLvl;
    public int CloudLvl
    {
        get { return Mathf.CeilToInt(cloudLvl); }
        set { cloudLvl = value; }
    }

    // Direction the thunderhead blows the wind
    private Vector3 galeVector = new Vector3(0, 0, 0);
    public Vector3 GaleVector
    {
        get { return galeVector; }
        set { galeVector = value; }
    }

    // Parameters of thunderhead when in STORMFRONT state
    private float stormFrontRange = -1f;
    [SerializeField] private float lightningAOE = 2.5f;

    // Parameters of thunderhead when in LAUNCHED state
    private Vector3 launchDirection = Vector2.zero;
    private float launchEffectRadius;
    [SerializeField] private float launchVelocity = 20f;

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject wizardObject;
    private LineRenderer areaOfEffectGUI;

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
        CloudLvl = 1;

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
        areaOfEffectGUI = GetComponent<LineRenderer>();
        if (!areaOfEffectGUI)
        {
            Debug.LogError("No Line Renderer attached to thunderhead", areaOfEffectGUI);
        }
        areaOfEffectGUI.enabled = false;

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
                areaOfEffectGUI.enabled = (wizardObject.GetComponent<Wizard>().CurMode == Wizard.Mode.GALE);
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
        GUI.Label(new Rect(labelPos, labelSize), CloudLvl.ToString());
    }

    // Destroy launched thunderhead when it is no longer visible
    private void OnBecameInvisible()
    {
        if (curState == ThunderheadState.LAUNCHED)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    // Convert gale strength level to some float value
    public float CloudStrength()
    {
        return CloudLvl; // TODO: figure out strength calculation
    }

    // Merge thunderheads
    // The thunderhead that is passed into the function is destroyed
    public void Merge(GameObject thunderheadToMerge)
    {
        Thunderhead thunderheadObj = thunderheadToMerge.GetComponent<Thunderhead>();
        if(thunderheadObj)
        {
            CloudLvl += thunderheadObj.CloudLvl;
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
        float angleIncrement = 360f / (areaOfEffectGUI.positionCount - 1);
        for (int pos = 0; pos < areaOfEffectGUI.positionCount; pos++)
        {
            // Calculate the values of position
            positionOnCircle = (Quaternion.Euler(0, 0, angleIncrement * pos) * new Vector2(0, 1)).normalized;
            positionOnCircle *= launchEffectRadius;
            positionNode.x = positionOnCircle.x;
            positionNode.z = positionOnCircle.y;

            areaOfEffectGUI.SetPosition(pos, positionNode);
        }

        areaOfEffectGUI.enabled = true;
    }

    // Change thunderhead to stormfront
    public void MakeStormFront(float newStormFrontRange)
    {
        curState = ThunderheadState.STORMFRONT;
        stormFrontRange = newStormFrontRange;

        // Set up the collision shape
        CapsuleCollider collisionShape = GetComponent<CapsuleCollider>();
        collisionShape.radius = newStormFrontRange;
        collisionShape.center = Vector3.zero;

        // Set the size of the stormfront range area indicator
        areaOfEffectGUI.enabled = false;
        areaOfEffectGUI.useWorldSpace = false;
        Vector3 positionNode = new Vector3(0, 0.1f - transform.position.y, 0);
        Vector2 positionOnCircle;
        float angleIncrement = 360f / (areaOfEffectGUI.positionCount - 1);
        for (int pos = 0; pos < areaOfEffectGUI.positionCount; pos++)
        {
            // Calculate the values of position
            positionOnCircle = (Quaternion.Euler(0, 0, angleIncrement * pos) * new Vector2(0, 1)).normalized;
            positionOnCircle *= newStormFrontRange;
            positionNode.x = positionOnCircle.x;
            positionNode.z = positionOnCircle.y;

            areaOfEffectGUI.SetPosition(pos, positionNode);
        }
    }

    // Call lightning at position
    internal IEnumerator CallLightning(Vector3 strikePos)
    {
        curState = ThunderheadState.CALLSTRIKE;

        // Set up the collision shape
        CapsuleCollider collisionShape = GetComponent<CapsuleCollider>();
        collisionShape.radius = lightningAOE;
        collisionShape.center = strikePos - transform.position;

        // Set the size of lightning AOE indicator
        areaOfEffectGUI.useWorldSpace = true;
        Vector3 positionNode = new Vector3(0, 0.1f - strikePos.y, 0);
        Vector2 positionOnCircle;
        float angleIncrement = 360f / (areaOfEffectGUI.positionCount - 1);
        for (int pos = 0; pos < areaOfEffectGUI.positionCount; pos++)
        {
            // Calculate the values of position
            positionOnCircle = (Quaternion.Euler(0, 0, angleIncrement * pos) * new Vector2(0, 1)).normalized;
            positionOnCircle *= lightningAOE;
            positionNode.x = strikePos.x + positionOnCircle.x;
            positionNode.z = strikePos.z + positionOnCircle.y;

            areaOfEffectGUI.SetPosition(pos, positionNode);
            yield return null;
        }
        areaOfEffectGUI.enabled = true;

        // Hold the lightning strike for a time
        yield return new WaitForSeconds(0.05f);
        MakeStormFront(stormFrontRange);
    }

    // Handle collisions
    public void OnTriggerEnter(Collider other)
    {
        switch (curState)
        {
            case ThunderheadState.LAUNCHED:
                if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
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

    // Handle collisions
    public void OnTriggerStay(Collider other)
    {
        switch (curState)
        {
            case ThunderheadState.STORMFRONT:
                if (wizardObject.GetComponent<Wizard>().CurMode == Wizard.Mode.GALE && other.CompareTag("Enemy"))
                {
                    Destroy(other.gameObject);
                }
                break;
            case ThunderheadState.CALLSTRIKE:
                if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
                {
                    Destroy(other.gameObject);
                }
                break;
            default:
                break;
        }
    }
}
