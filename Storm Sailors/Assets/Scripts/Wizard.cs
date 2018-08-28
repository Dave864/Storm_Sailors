using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode { WIND, STORM }

public class Wizard : MonoBehaviour
{
    // State variables to manage input events
    private bool positioning = false;
    private bool shifting = false;
    private Mode curMode = Mode.WIND;

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject cloudManager;

    // Wizard movement variables
    private Vector2 curCompassPos = new Vector2(0, 0);
    private Vector2 destinationCompassPos = new Vector2(0, 0);
    private Quaternion wizardCompassRot = new Quaternion();
    private float rotRate;
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

        // Position the wizard at the radius of the cloud rail
        float zPos = compassCenter.transform.position.z + compassCenter.GetComponent<CompassCenter>().compassRadius;
        float xPos = compassCenter.transform.position.x;
        float yPos = compassCenter.transform.position.y;
        transform.position = new Vector3(xPos, yPos, zPos);

        // Rotate wizard towards the center of the compass
        transform.LookAt(compassCenter.transform, Vector3.forward);

        // Intialize curPos of wizard
        curCompassPos = compassCenter.GetComponent<CompassCenter>().strtPos;
        rotRate = compassCenter.GetComponent<CompassCenter>().rotRate;
        shiftRate = compassCenter.GetComponent<CompassCenter>().shiftRate;
    }

    // Update is called once per frame
    void Update()
    {
        bool changeMode = Input.GetButtonDown("Change Mode");
        switch (curMode)
        {
            case Mode.WIND:
                // Change mode
                if (changeMode && !shifting)
                {
                    StartCoroutine(ShiftMode());
                }
                // Handle wind mode actions
                else
                {
                    PositionAction();
                    SummonAction();

                    // Dispel all clouds
                    if (Input.GetButton("Dispel All"))
                    {
                        cloudManager.GetComponent<CloudManager>().DispelAll();
                    }
                }
                break;

            case Mode.STORM:
                // Change mode
                if (changeMode && !shifting)
                {
                    StartCoroutine(ShiftMode());
                }
                // Handle storm mode actions
                else
                {
                    //transform.Rotate(Vector3.up * 30 * Time.deltaTime);
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
        float shiftRate = compassCenter.GetComponent<CompassCenter>().shiftRate;
        Quaternion stormPos;
        Quaternion stormRot;
        Quaternion windPos;
        switch (curMode)
        {
            case Mode.WIND:
                // Don't shift modes if wizard is changing compass position
                if (!positioning)
                {
                    // Move wizard to storm mode position
                    stormPos = Quaternion.LookRotation(compassCenter.transform.up, Vector3.up);
                    windPos = compassCenter.GetComponent<CompassCenter>().PositionRot(curCompassPos);
                    stormRot = Quaternion.Euler(180f, 0, 0);
                    wizardCompassRot = transform.rotation;
                    for (float shiftTime = 0; shiftTime < shiftRate; shiftTime += Time.deltaTime)
                    {
                        compassCenter.transform.rotation = Quaternion.Slerp(windPos, stormPos, shiftTime / shiftRate);
                        transform.rotation = Quaternion.Slerp(wizardCompassRot, stormRot, shiftTime / shiftRate);
                        yield return null;
                    }

                    // Finalize move to account for loop not quite moving the wizard fully
                    compassCenter.transform.rotation = stormPos;
                    transform.rotation = stormRot;
                    curMode = Mode.STORM;
                }
                shifting = false;
                yield return null;
                break;
            
            case Mode.STORM:
                // Move wizard to wind mode position
                stormPos = compassCenter.transform.rotation;
                windPos = compassCenter.GetComponent<CompassCenter>().PositionRot(curCompassPos);
                stormRot = transform.rotation;
                for (float shiftTime = 0; shiftTime < shiftRate; shiftTime += Time.deltaTime)
                {
                    compassCenter.transform.rotation = Quaternion.Slerp(stormPos, windPos, shiftTime / shiftRate);
                    transform.rotation = Quaternion.Slerp(stormRot, wizardCompassRot, shiftTime / shiftRate);
                    yield return null;
                }

                // Finalize move to account for loop not quite moving the wizard fully
                compassCenter.transform.rotation = windPos;
                transform.rotation = wizardCompassRot;
                curMode = Mode.WIND;
                shifting = false;
                yield return null;
                break;
            
            default:
                shifting = false;
                yield return null;
                break;
        }
    }

    // Move wizard to new compass position
    private void PositionAction()
    {
        // Construct position vector from input
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 posVect = new Vector2(hInput, vInput);

        // Set up the start and end positions if not already repositioning
        if (!positioning && posVect != new Vector2(0, 0))
        {
            destinationCompassPos = new Vector2(posVect.x, posVect.y);
            positioning = true;
        }

        // Reposition wizard
        if (positioning)
        {
            StartCoroutine(Position(destinationCompassPos));
        }
    }

    // Summon or dispel cloud at wizard position
    private void SummonAction()
    {
        if (Input.GetButtonDown("Summon") && !positioning)
        {
            // Dispel cloud at position
            if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curCompassPos))
            {
                cloudManager.GetComponent<CloudManager>().DispelThunderhead(curCompassPos);
            }

            // Summon a cloud at position
            else
            {
                cloudManager.GetComponent<CloudManager>().SpawnThunderhead(curCompassPos);
            }
        }
    }

    // Reposition wizard
    IEnumerator Position(Vector2 endPos)
    {
        // Set up the start and end rotations
        Quaternion strtRot = compassCenter.GetComponent<CompassCenter>().PositionRot(curCompassPos);
        Quaternion endRot = compassCenter.GetComponent<CompassCenter>().PositionRot(endPos);

        for (float posTime = 0; posTime < rotRate; posTime += Time.deltaTime)
        {
            // Rotate the center of the rail to position the wizard
            if (compassCenter != null)
            {
                compassCenter.transform.rotation = Quaternion.Slerp(strtRot, endRot, posTime / rotRate);
            }
            yield return null;
        }

        // Finalize rotation to account for for loop not quite rotating all the way
        compassCenter.transform.rotation = endRot;

        // update the current wizard position
        curCompassPos = endPos;

        // Reset timer and state flags
        positioning = false;
        yield return null;
    }
}
