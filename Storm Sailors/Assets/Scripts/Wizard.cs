using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode { WIND, STORM }

public class Wizard : MonoBehaviour
{
    // State variables to manage input events
    private bool positioning = false;
    private Mode curMode = Mode.WIND;

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject cloudManager;

    // Wizard movement variable
    private Vector2 curPos = new Vector2(0, 0);
    private Vector2 destinationPos = new Vector2(0, 0);
    private float rotRate;

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
        curPos = compassCenter.GetComponent<CompassCenter>().strtPos;
        rotRate = compassCenter.GetComponent<CompassCenter>().rotRate;
    }

    // Update is called once per frame
    void Update()
    {
        bool changeMode = Input.GetButtonDown("Change Mode");
        switch (curMode)
        {
            case Mode.WIND:
                // Change mode
                if (changeMode)
                {
                    ShiftMode();
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
                if (changeMode)
                {
                    ShiftMode();
                }
                // Handle storm mode actions
                else
                {

                }
                break;

            default:
                break;
        }
    }

    // Shift wizard between various modes
    private void ShiftMode()
    {
        switch (curMode)
        {
            case Mode.WIND:
                if (!positioning)
                {
                    curMode = Mode.STORM;
                }
                break;
            case Mode.STORM:
                curMode = Mode.WIND;
                break;
            default:
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
            destinationPos = new Vector2(posVect.x, posVect.y);
            positioning = true;
        }

        // Reposition wizard
        if (positioning)
        {
            StartCoroutine(Position(destinationPos));
        }
    }

    // Summon or dispel cloud at wizard position
    private void SummonAction()
    {
        if (Input.GetButtonDown("Summon") && !positioning)
        {
            // Dispel cloud at position
            if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curPos))
            {
                cloudManager.GetComponent<CloudManager>().DispelThunderhead(curPos);
            }
            // Summon a cloud at position
            else
            {
                cloudManager.GetComponent<CloudManager>().SpawnThunderhead(curPos);
            }
        }
    }

    // Reposition wizard
    IEnumerator Position(Vector2 endPos)
    {
        // Set up the start and end rotations
        Quaternion strtRot = compassCenter.GetComponent<CompassCenter>().PositionRot(curPos);
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

        // update the current wizard position
        curPos = endPos;

        // Reset timer and state flags
        positioning = false;
        yield return null;
    }
}
