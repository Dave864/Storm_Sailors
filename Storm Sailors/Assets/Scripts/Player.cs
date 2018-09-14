using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start area reference
    public Vector3 startPos = new Vector3(0, 0, 0);

    // References to components of player
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject wizardObject;
    private GameObject ship;

    // Movement parameters
    private readonly int defaultSpeed = 500;        // The default base speed of the ship
    private readonly float defaultRotRate = 0.1f;   // The default time it takes to change wizard position in sec
    private int baseSpeed;                          // The used base speed of the ship

    // Use this for initialization
    void Start()
    {
        // Establish reference to compass center
        compassCenter = gameObject.transform.Find("Compass/Compass Center").gameObject;
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Establish reference to wizard
        wizardObject = gameObject.transform.Find("Compass/Compass Center/Wizard Object").gameObject;
        if (wizardObject == null)
        {
            Debug.LogError("Wizard object not found", wizardObject);
        }

        // Establish reference to ship
        ship = gameObject.transform.Find("Ship Object").gameObject;
        if (ship == null)
        {
            Debug.LogError("Ship object not found", ship);
        }

        // Establish reference to cloud manager
        cloudManager = gameObject.transform.Find("Compass/Cloud Manager").gameObject;
        if (cloudManager == null)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Initialize the movement parameters
        baseSpeed = (ship == null) ? defaultSpeed : ship.GetComponent<ShipObject>().baseSpeed;

        // Set initial position of player
        transform.position = new Vector3(startPos.x, transform.position.y, startPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Quit qame
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }

        // Move the player entity based off of the heading of the ship
        transform.position += ship.GetComponent<ShipObject>().CurHeading * baseSpeed * Time.deltaTime;
    }
}
