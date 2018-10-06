using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    // Enumerators for the modes the wizard can be in
    public enum Mode { GALE = 0, STORM = 1 }
    private Mode curMode = Mode.GALE;
    public Mode CurMode { get { return curMode; } }

    // State variables to manage input events
    private bool shifting = false;
    
    // Reference to held cloud
    [HideInInspector] public GameObject heldCloud;

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject shipObject;

    // Wizard movement variables
    private Quaternion curCompassRot = new Quaternion();
    private float shiftRate;

    // Use this for initialization
    void Awake()
    {
        // Get reference to cloud manager
        cloudManager = GameObject.Find("Cloud Manager");
        if (cloudManager == null)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Get reference to compass center
        compassCenter = GameObject.Find("Compass Center");
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Get reference to ship object
        shipObject = GameObject.Find("Ship Object");
        if (shipObject == null)
        {
            Debug.LogError("Ship Object object not found", shipObject);
        }

        // Position the wizard at the radius of the cloud rail
        transform.localPosition = new Vector3(0, compassCenter.GetComponent<CompassCenter>().compassRadius, 0);

        // Rotate wizard towards the center of the compass
        transform.LookAt(compassCenter.transform);

        // Initialize held cloud to be null
        heldCloud = null;

        // Intialize rotation and mode shift time of wizard
        curCompassRot = transform.rotation;
        shiftRate = compassCenter.GetComponent<CompassCenter>().shiftRate;
    }

    // Update is called once per frame
    void Update()
    {
        bool changeMode = Input.GetButtonDown("Change Mode");
        switch (curMode)
        {
            case Mode.GALE:
                // Change mode
                if (changeMode && !shifting)
                {
                    StartCoroutine(ShiftMode());
                }
                break;

            case Mode.STORM:
                // Change mode
                if (changeMode && !shifting)
                {
                    StartCoroutine(ShiftMode());
                }
                break;

            default:
                break;
        }
    }

    // Shift wizard between various modes
    IEnumerator ShiftMode()
    {
        shifting = true;
        Vector3 stormPos;
        Vector3 windPos;
        Quaternion stormRot;
        Quaternion thunderheadRot;
        switch (curMode)
        {
            // Shift from gale mode to storm mode
            case Mode.GALE:
                // Do not shift if wizard is performing an action
                if (GetComponent<GaleMode>().CurAction != GaleMode.Action.GRAB && GetComponent<GaleMode>().CurAction != GaleMode.Action.DEFAULT)
                {
                    shifting = false;
                    yield break;
                }

                // Calculate the start and end positions of the mode shift
                windPos = transform.localPosition;
                stormPos = new Vector3(0, 0, compassCenter.transform.position.y - compassCenter.GetComponent<CompassCenter>().stormHeight);
                stormRot = shipObject.transform.rotation;
                curCompassRot = transform.rotation;

                // Get the start rotation of the held thunderhead
                thunderheadRot = (heldCloud != null) ? heldCloud.transform.rotation : Quaternion.identity;
                if (heldCloud)
                {
                    heldCloud.GetComponent<Thunderhead>().IsHeld = false;
                }

                // Move wizard to storm mode position
                for (float shiftTime = 0; shiftTime < shiftRate; shiftTime += Time.deltaTime)
                {
                    transform.localPosition = Vector3.Slerp(windPos, stormPos, shiftTime / shiftRate);
                    transform.rotation = Quaternion.Slerp(curCompassRot, stormRot, shiftTime / shiftRate);

                    // Rotate held cloud
                    if (heldCloud)
                    {
                        heldCloud.transform.rotation = Quaternion.Slerp(thunderheadRot, stormRot, shiftTime / shiftRate);
                    }
                    yield return null;
                }
                transform.localPosition = stormPos;
                transform.rotation = stormRot;
                curMode = Mode.STORM;

                // Merge held cloud into storm cloud
                if (heldCloud)
                {
                    cloudManager.GetComponent<CloudManager>().MoveThunderHead(Vector2.zero, ref heldCloud);
                }

                shifting = false;
                yield return null;
                break;
            
            // Shift from storm mode to gale mode
            case Mode.STORM:
                // Do not shift if wizard is performing an action
                if (GetComponent<StormMode>().CurAction != StormMode.Action.DEFAULT)
                {
                    shifting = false;
                    yield break;
                }

                // Calculate the start and end positions of the mode shift
                stormPos = transform.localPosition;
                windPos = new Vector3(0, compassCenter.GetComponent<CompassCenter>().compassRadius, 0);
                stormRot = transform.rotation;

                // Dispel storm cloud if cloud is not at a sustainable level
                if (cloudManager.GetComponent<CloudManager>().StormCloudRef)
                {
                    int stormLvl = cloudManager.GetComponent<CloudManager>().StormCloudRef.GetComponent<Thunderhead>().CloudLvl;
                    if (stormLvl < GetComponent<StormMode>().StormLevelSustainable)
                    {
                        cloudManager.GetComponent<CloudManager>().DispelThunderhead();
                    }
                }

                // Move wizard to gale mode position
                for (float shiftTime = 0; shiftTime < shiftRate; shiftTime += Time.deltaTime)
                {
                    transform.localPosition = Vector3.Slerp(stormPos, windPos, shiftTime / shiftRate);
                    transform.rotation = Quaternion.Slerp(stormRot, curCompassRot, shiftTime / shiftRate);
                    yield return null;
                }
                transform.localPosition = windPos;
                transform.rotation = curCompassRot;
                curMode = Mode.GALE;
                shifting = false;
                yield return null;
                break;
            
            default:
                shifting = false;
                yield return null;
                break;
        }
    }
}
