using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShip : MonoBehaviour
{
    // State variables
    private bool shipIsRotating = false;

    // Movement parameters
    public int baseSpeed = 100;
    public float uTurnTime = 1.0f;  // The time for the ship to turn 180 degrees in seconds; TODO: min value in editor (post jam)
    private float turnRate;         // The turning rate in deg / sec

    // The direction the ship is currently facing
    private Vector3 curHeading;

    // Reference to Cloud Manager, which handles calculating the gale vector
    private GameObject cloudManager;

	// Use this for initialization
	void Start () {
        turnRate = 180.0f / uTurnTime;
        cloudManager = GameObject.Find("Cloud Manager");
        curHeading = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 galeDir = cloudManager.GetComponent<TestCloudManager>().CombinedGaleVector;
        if (!shipIsRotating && galeDir.normalized != curHeading.normalized)
        {
            StartCoroutine(RotateShip(galeDir));
        }
	}

    IEnumerator RotateShip(Vector3 galeDir)
    {
        shipIsRotating = true;
        // Get angle between galeDir and ship's current heading
        float headingDiff = Vector3.Angle(transform.forward, galeDir);

        // Construct start and end rotations
        Quaternion endRotation = Quaternion.Euler(new Vector3(0, headingDiff, 0));
        Quaternion strtRotation = transform.rotation;

        // Calculate time it will take to rotate ship
        float rotTime = headingDiff * turnRate;

        // Rotate ship to new rotation
        for (float curTime = 0; curTime < rotTime; curTime += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(strtRotation, endRotation, curTime / rotTime);
            yield return null;
        }
        shipIsRotating = false;
        yield return null;
    }
}
