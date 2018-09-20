using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipObject : MonoBehaviour
{
    // State variables
    private bool shipIsRotating = false;
    private bool noGale = true;

    // Movement parameters
    [HideInInspector] public int baseSpeed = 100;       // The base movement speed of the chosen ship
    [HideInInspector] public float slowTime = 1.0f;     // The time it takes for the chosen ship to come to a stop
    private float turnRate = 45.0f;                     // The turning rate in deg / sec

    // The current direction vector of the ship
    public Vector3 CurHeading { get; set; }

    // Reference to Cloud Manager, which handles calculating the gale vector
    private GameObject cloudManager;

    // Use this for initialization
    void Start()
    {
        // Get movement parameters of chosen ship
        GameObject selectedShip = gameObject.transform.GetChild(0).gameObject;
        if (selectedShip)
        {
            baseSpeed = selectedShip.GetComponent<Ship>().baseSpeed;
            slowTime = selectedShip.GetComponent<Ship>().slowTime;
            turnRate = selectedShip.GetComponent<Ship>().turnRate;
        }
        else
        {
            Debug.LogError("No ship selected for ShipObject", selectedShip);
        }

        // Get reference to cloud manager object
        cloudManager = GameObject.Find("Cloud Manager");
        if(!cloudManager)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

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
        noGale = (galeDir == Vector3.zero);
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
            // Stop rotating if there is no gale
            if (noGale)
            {
                curTime = rotTime;
            }
            yield return null;
        }
        CurHeading = transform.forward.normalized;

        shipIsRotating = false;
        yield return null;
    }
}
