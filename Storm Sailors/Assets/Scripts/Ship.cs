using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    // State variables
    private bool shipIsRotating = false;

    // Movement parameters
    public int baseSpeed = 100;
    public float uTurnTime = 1.0f;  // The time for the ship to turn 180 degrees in seconds; TODO: min value in editor (post jam)
    private float turnRate;         // The turning rate in deg / sec

    // The current direction vector of the ship
    public Vector3 CurHeading { get; set; }

    // Reference to Cloud Manager, which handles calculating the gale vector
    private GameObject cloudManager;

    // Use this for initialization
    void Start()
    {
        turnRate = 180.0f / uTurnTime;
        cloudManager = GameObject.Find("Cloud Manager");
        CurHeading = transform.forward.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 galeDir = cloudManager.GetComponent<CloudManager>().CombinedGaleVector.normalized;
        // Set heading to zero if there is no gale
        if (galeDir == Vector3.zero)
        {
            CurHeading = Vector3.zero;
        }
        // Set heading to match galeDir
        if (!shipIsRotating && galeDir != CurHeading)
        {
            StartCoroutine(RotateShip(galeDir));
        }
    }

    IEnumerator RotateShip(Vector3 galeDir)
    {
        shipIsRotating = true;
        // Get angle between galeDir and ship's current heading
        float headingDiff = Vector3.SignedAngle(transform.forward, galeDir, Vector3.up);

        // Construct start and end rotations
        Quaternion strtRotation = transform.rotation;
        Vector3 endEulerRot = new Vector3(0, headingDiff, 0) + strtRotation.eulerAngles;
        Quaternion endRotation = Quaternion.Euler(endEulerRot);

        // Calculate time it will take to rotate ship
        float rotTime = Mathf.Abs(headingDiff) / turnRate;

        // Rotate ship to new rotation
        for (float curTime = 0; curTime < rotTime; curTime += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(strtRotation, endRotation, curTime / rotTime);
            CurHeading = transform.forward.normalized;
            yield return null;
        }
        CurHeading = transform.forward.normalized;

        shipIsRotating = false;
        yield return null;
    }

    private void OnTriggerEnter(Collider obj)
    {
        // Go back to start if ship hit an obstacle
        if (obj.gameObject.CompareTag("Obstacle"))
        {
            cloudManager.GetComponent<CloudManager>().DispelAll();
            Vector3 startPos = GetComponentInParent<Player>().StartPos;
            transform.parent.position = startPos;
        }
        // Exit the game when finish line is reached
        else if (obj.gameObject.CompareTag("Finish"))
        {
            Application.Quit();
        }
    }
}
