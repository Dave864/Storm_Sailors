using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWizard : MonoBehaviour
{
    // State variables to manage input events
    private bool spawnPressed = false;

    // Reference to game objects
    private GameObject railCenter;
    private GameObject cloudManager;

    // Wizard's current position
    private Vector2 curPos = new Vector2(0, 0);

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Summon"))
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

    // Update position
    public void SetPosition(Vector2 newPos)
    {
        curPos = newPos;
    }
}
