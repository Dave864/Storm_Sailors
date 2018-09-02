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
    private GameObject shipObject;

    // Wizard movement variables
    private Vector2 curCompassPos = new Vector2(0, 0);
    private Quaternion curCompassRot = new Quaternion();
    private Vector2 destinationCompassPos = new Vector2(0, 0);
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

        // Intialize curPos of wizard
        curCompassPos = compassCenter.GetComponent<CompassCenter>().strtPos;
        curCompassRot = transform.rotation;
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
                    // Face the wizard in the direction of the mouse
                    if (Input.mousePresent)
                    {
                        // Translate the mouse position to in game world position
                        Vector2 mousePos = Input.mousePosition;
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

                        // Raycast the mouse position find the apparent mouse position on the sea
                        Vector3 mouseSeaPosition = new Vector3();
                        RaycastHit mouseRayHit;
                        if (Physics.Raycast(mouseWorldPosition, Camera.main.transform.forward, out mouseRayHit))
                        {
                            mouseSeaPosition = new Vector3(mouseRayHit.point.x, transform.position.y, mouseRayHit.point.z);
                        }
                        else
                        {
                            mouseSeaPosition = mouseWorldPosition;
                        }
                        
                        // Rotate the wizard in the direction of the mouse point on the sea
                        Vector3 direction = mouseSeaPosition - transform.position;
                        transform.forward = direction;
                    }
                    // Do other stuff
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
            case Mode.WIND:
                // Don't shift modes if wizard is changing compass position
                if (!positioning)
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
