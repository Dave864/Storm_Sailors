using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWizard : MonoBehaviour
{
    // Reference to game objects
    private GameObject railCenter;
    private GameObject cloudManager;

    // Wizard's current position
    private Vector2 position = new Vector2(0, 0);

	// Use this for initialization
	void Awake()
    {
        // Get reference to cloud manager
        cloudManager = GameObject.Find("Cloud Manager");

        // Position the wizard at the radius of the cloud rail
        railCenter = GameObject.Find("Rail Center");
        float newXPos = railCenter.transform.position.x;
        float newYPos = railCenter.GetComponent<TestCloudRail>().railRadius;
        float newZPos = railCenter.transform.position.z;
        transform.position = new Vector3(newXPos, newYPos, newZPos);
	}
	
	// Update is called once per frame
	void Update()
    {
        int summonInput = Mathf.RoundToInt(Input.GetAxisRaw("Summon"));
        if (summonInput > 0)
        {
            // Dispel cloud at position
            if (cloudManager.GetComponent<TestCloudManager>().ThunderheadAtPos(position))
            {
                cloudManager.GetComponent<TestCloudManager>().DispelThunderhead(position);
            }
            // Summon a cloud at position
            else
            {
                cloudManager.GetComponent<TestCloudManager>().SpawnThunderhead(position);
            }
        }
	}

    // Update position
    public void SetPosition(Vector2 newPos)
    {
        position = newPos;
    }
}
