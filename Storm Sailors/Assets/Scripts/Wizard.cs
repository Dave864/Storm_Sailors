using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    // State variables to manage input events
    private bool spawnPressed = false;
    private bool positioning = false;

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

        // Position the wizard at the radius of the cloud rail
        compassCenter = GameObject.Find("Compass Center");
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
        PositionAction();
        SummonAction();

        // Dispel all clouds
        if (Input.GetButton("Dispel All"))
        {
            cloudManager.GetComponent<CloudManager>().DispelAll();
        }
    }

    // Move wizard to new position
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
        if (Input.GetButton("Summon") && !positioning)
        {
            if (!spawnPressed)
            {
                spawnPressed = true;
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
        else
        {
            spawnPressed = false;
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
