using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWizard : MonoBehaviour
{
    // State variables to manage input events
    private bool spawnPressed = false;
    private bool positioning = false;

    // Reference to game objects
    private GameObject railCenter;
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
        railCenter = GameObject.Find("Rail Center");
        float zPos = railCenter.transform.position.z + railCenter.GetComponent<TestCloudRail>().railRadius;
        float xPos = railCenter.transform.position.x;
        float yPos = railCenter.transform.position.y;
        transform.position = new Vector3(xPos, yPos, zPos);

        // Intialize curPos of wizard
        curPos = railCenter.GetComponent<TestCloudRail>().strtPos;
        rotRate = railCenter.GetComponent<TestCloudRail>().rotRate;
    }

    // Update is called once per frame
    void Update()
    {
        PositionAction();
        SummonAction();
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
                if (cloudManager.GetComponent<TestCloudManager>().ThunderheadAtPos(curPos))
                {
                    cloudManager.GetComponent<TestCloudManager>().DispelThunderhead(curPos);
                }
                // Summon a cloud at position
                else
                {
                    cloudManager.GetComponent<TestCloudManager>().SpawnThunderhead(curPos);
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
        float endAngle = railCenter.GetComponent<TestCloudRail>().PositionAngle(endPos);
        Quaternion strtRot = railCenter.transform.rotation;
        Quaternion endRot = Quaternion.Euler(new Vector3(0, endAngle, 0));

        for (float posTime = 0; posTime < rotRate; posTime += Time.deltaTime)
        {
            // Rotate the center of the rail to position the wizard
            if (railCenter != null)
            {
                railCenter.transform.rotation = Quaternion.Slerp(strtRot, endRot, posTime / rotRate);
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
