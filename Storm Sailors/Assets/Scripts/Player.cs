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
    private readonly int defaultSpeed = 500;       // The default base speed of the ship
    private int baseSpeed;                         // The used base speed of the ship
    private float curSpeed;                        // The current speed of the ship
    private readonly float defaultSlowTime = 1.0f; // The default deceleration rate of the shape
    private float slowTime;                        // The rate the ship slows down when no wind is present
    private float curSlowTime;                     // The current amount of time spent slowing down
    private Vector3 lastHeading;                   // The vector of the last heading created by gale clouds

    // Use this for initialization
    void Start()
    {
        // Establish reference to compass center
        compassCenter = gameObject.transform.Find("Compass/Compass Center").gameObject;
        if (!compassCenter)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Establish reference to wizard
        wizardObject = gameObject.transform.Find("Compass/Compass Center/Wizard Object").gameObject;
        if (!wizardObject)
        {
            Debug.LogError("Wizard object not found", wizardObject);
        }

        // Establish reference to ship
        ship = gameObject.transform.Find("Ship Object").gameObject;
        if (!ship)
        {
            Debug.LogError("Ship object not found", ship);
        }

        // Establish reference to cloud manager
        cloudManager = gameObject.transform.Find("Compass/Cloud Manager").gameObject;
        if (!cloudManager)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Initialize the movement parameters
        baseSpeed = (!ship) ? defaultSpeed : ship.GetComponent<ShipObject>().baseSpeed;
        curSpeed = 0;
        slowTime = (!ship) ? defaultSlowTime : ship.GetComponent<ShipObject>().slowTime;
        curSlowTime = 0;
        lastHeading = Vector3.zero;

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

        // Decelerate the player entity if there is no gale blowing
        if (ship.GetComponent<ShipObject>().CurHeading == Vector3.zero)
        {
            //curSpeed -= slowTime;
            //curSpeed = (curSpeed < 0) ? 0 : curSpeed;
            if (curSlowTime <= slowTime)
            {
                curSpeed = baseSpeed * ((slowTime - curSlowTime) / slowTime);
                curSlowTime += Time.deltaTime;
            }
            else
            {
                curSpeed = 0;
            }
        }
        // Move the player entity at full speed at the ship's current heading
        else
        {
            lastHeading = ship.GetComponent<ShipObject>().CurHeading;
            curSpeed = baseSpeed;
            curSlowTime = 0;
        }
        // Move the player entity
        transform.position += lastHeading * curSpeed * Time.deltaTime;
    }
}
