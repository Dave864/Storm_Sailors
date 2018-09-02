using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode { GALE = 0, STORM = 1 }

public class Wizard : MonoBehaviour
{
    // State variables to manage input events
    private bool shifting = false;

    private Mode curMode = Mode.GALE;
    public int CurMode
    {
        get { return (int)curMode; }
    }

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
        switch (curMode)
        {
            case Mode.GALE:
                // Don't shift modes if wizard is changing compass position
                if (!GetComponent<GaleMode>().Positioning)
                {
                    // Calculate the start and end positions of the mode shift
                    windPos = transform.localPosition;
                    stormPos = new Vector3(0, 0, compassCenter.transform.position.y - compassCenter.GetComponent<CompassCenter>().stormHeight);
                    stormRot = shipObject.transform.rotation;
                    curCompassRot = transform.rotation;

                    // Move wizard to storm mode position
                    for (float shiftTime = 0; shiftTime < shiftRate; shiftTime += Time.deltaTime)
                    {
                        transform.localPosition = Vector3.Slerp(windPos, stormPos, shiftTime / shiftRate);
                        transform.rotation = Quaternion.Slerp(curCompassRot, stormRot, shiftTime / shiftRate);
                        yield return null;
                    }
                    transform.localPosition = stormPos;
                    transform.rotation = stormRot;
                    curMode = Mode.STORM;
                }
                shifting = false;
                yield return null;
                break;
            
            case Mode.STORM:
                // Calculate the start and end positions of the mode shift
                stormPos = transform.localPosition;
                windPos = new Vector3(0, compassCenter.GetComponent<CompassCenter>().compassRadius, 0);
                stormRot = transform.rotation;

                // Move wizard to wind mode position
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
