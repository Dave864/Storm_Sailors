using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWizard : MonoBehaviour
{
    private GameObject railCenter;

	// Use this for initialization
	void Awake ()
    {
        // Position the wizard at the radius of the cloud rail
        railCenter = GameObject.Find("Rail Center");
        float newXPos = railCenter.transform.position.x;
        float newYPos = railCenter.transform.position.y + 0.2f;
        float newZPos = railCenter.GetComponent<TestCloudRail>().railRadius;
        transform.position = new Vector3(newXPos, newYPos, newZPos);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
