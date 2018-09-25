using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StormMode : MonoBehaviour
{
    // Enumerator for the various actions in Storm mode
    public enum Action { DEFAULT, CHARGE, GATHER, LAUNCH }
    private Action curAction = Action.DEFAULT;

    // Storm mode timer variables
    [SerializeField] private Slider stormTimerSlider;        // UI Slider object to serve as timer
    [SerializeField] private AnimationCurve stormChargeMult; // Curve to determine the timer multiplier for charging storm cloud
    [SerializeField] private float stormSpawnTime = 0.5f;    // The time to spawn intial storm cloud
    [SerializeField] private float stormLaunchTime = 0.15f;  // The time to confirm a launch action
    [SerializeField] private float stormGatherTime = 0.5f;   // The time to gather spawned gale clouds
    private float curLaunchTime = 0;                         // The timer for confirming a launch action

    // Storm level thresholds
    [SerializeField] private int stormLevelSustainable = 3;
    [SerializeField] private int stormLevelOverload = 5;
    public int StormLevelSustainable { get { return stormLevelSustainable; } }
    public int StormLevelOverload { get { return stormLevelOverload; } }

    // Radius of sustained storm cloud range
    [SerializeField] private float stormFrontRange = 10f;

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject shipObject;

    // Use this for initialization
    private void Awake()
    {
        // Get reference to cloud manager
        cloudManager = GameObject.Find("Cloud Manager");
        if (!cloudManager)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Get reference to compass center
        compassCenter = GameObject.Find("Compass Center");
        if (!compassCenter)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Get reference to ship object
        shipObject = GameObject.Find("Ship Object");
        if (!shipObject)
        {
            Debug.LogError("Ship Object object not found", shipObject);
        }
    }

    // Use this for initialization
    private void Start()
    {
        // Intialize the storm timer to be invisible
        if (stormTimerSlider)
        {
            stormTimerSlider.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            Debug.LogError("No slider UI assigned for cloud timer slider", stormTimerSlider);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (GetComponent<Wizard>().CurMode == Wizard.Mode.STORM)
        {
            // Execute various Storm mode actions
            WizardFaceMouse();
            StormActions();
            if (Input.GetButtonDown("Dispel All") && curAction == Action.DEFAULT)
            {
                StartCoroutine(GatherGaleClouds());
            }
        }
        else if (GetComponent<Wizard>().CurMode == Wizard.Mode.GALE)
        {
            // Call lightning strikes on enemies in range
        }
    }

    // Rotate the wizard to face the position of the mouse
    private void WizardFaceMouse ()
    {
        if (Input.mousePresent)
        {
            // Translate the mouse position to in game world position
            Vector2 mousePos = Input.mousePosition;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

            // Raycast the mouse position find the apparent mouse position on the sea
            Vector3 mouseSeaPosition = new Vector3();
            RaycastHit mouseRayHit;
            int layerMask = 1 << 4;
            if (Physics.Raycast(mouseWorldPosition, Camera.main.transform.forward, out mouseRayHit, Mathf.Infinity, layerMask))
            {
                mouseSeaPosition = new Vector3(mouseRayHit.point.x, transform.position.y, mouseRayHit.point.z);
            }
            else
            {
                mouseSeaPosition = mouseWorldPosition;
            }

            // Rotate the wizard in the direction of the mouse point on the sea
            Vector3 direction = mouseSeaPosition - transform.position;
            transform.forward = direction;
        }
    }

    // Conduct various storm actions
    private void StormActions()
    {
        // Actions when the action button is held or pressed
        // Pressing the button launches the storm thunderhead
        // Holding the button charges the storm thunderhead
        if (Input.GetButton("Cloud Action") && (curAction == Action.DEFAULT || curAction == Action.LAUNCH))
        {
            // Begin checking for confirmation of launch action
            if (Input.GetButtonDown("Cloud Action"))
            {
                curAction = Action.LAUNCH;
                curLaunchTime = 0;
            }
            // Check for confirmation of launch action
            else if (curAction == Action.LAUNCH)
            {
                curLaunchTime += Time.deltaTime;
                curAction = (curLaunchTime <= stormLaunchTime) ? Action.LAUNCH : Action.DEFAULT;
            }
            // Execute charge action
            else
            {
                StartCoroutine(ChargeThunderhead());
            }
        }

        // Actions when the action button is released
        // The launch action will execute if the charge action has not started
        else if (Input.GetButtonUp("Cloud Action") && curAction == Action.LAUNCH)
        {
            GameObject stormCloud = cloudManager.GetComponent<CloudManager>().StormCloudRef;
            if (stormCloud)
            {
                // Launch storm cloud if it is not sustainable
                if (stormCloud.GetComponent<Thunderhead>().GaleLvl < stormLevelSustainable)
                {
                    cloudManager.GetComponent<CloudManager>().StormCloudRef = null;
                    stormCloud.GetComponent<Thunderhead>().Launch(transform.forward);
                }
                // Call a lightning strike at mouse position (TODO)
                else
                {

                }
            }
            curAction = Action.DEFAULT;
        }
    }

    // Charge up thunderheaed to create or strengthen a storm cloud
    IEnumerator ChargeThunderhead()
    {
        curAction = Action.CHARGE;
        bool actionActivated = false;
        float chargeTime;
        float curTime = 0;
        stormTimerSlider.GetComponent<CanvasGroup>().alpha = 1;

        // Check to see if the storm cloud is created
        if (!cloudManager.GetComponent<CloudManager>().StormCloudRef)
        {
            // Check for spawn action
            while (Input.GetButton("Cloud Action") && !actionActivated)
            {
                stormTimerSlider.value = curTime / stormSpawnTime;
                curTime += Time.deltaTime;
                if (curTime >= stormSpawnTime)
                {
                    actionActivated = true;
                }
                yield return null;
            }

            // Spawn the storm cloud
            if (actionActivated)
            {
                cloudManager.GetComponent<CloudManager>().SpawnThunderhead();
            }
        }

        // Check to see if the storm cloud is strengthened
        else
        {
            int stormLvl = cloudManager.GetComponent<CloudManager>().StormCloudRef.GetComponent<Thunderhead>().GaleLvl;
            chargeTime = stormSpawnTime;
            chargeTime *= (stormChargeMult != null) ? stormChargeMult.Evaluate(stormLvl + 1) : stormLvl + 1;

            // Check for charge action
            while (Input.GetButton("Cloud Action") && !actionActivated)
            {
                stormTimerSlider.value = curTime / chargeTime;
                curTime += Time.deltaTime;
                if (curTime >= chargeTime)
                {
                    actionActivated = true;
                }
                yield return null;
            }

            // Strengthen the storm cloud
            if (actionActivated)
            {
                GameObject stormCloud = cloudManager.GetComponent<CloudManager>().StormCloudRef;
                stormCloud.GetComponent<Thunderhead>().GaleLvl++;

                // Change the storm cloud to a storm front
                if (stormLvl + 1 == StormLevelSustainable)
                {
                    stormCloud.GetComponent<Thunderhead>().StormFront(stormFrontRange);
                }
                // Dispel storm cloud if overcharged (TEMPORARY)
                else if (stormLvl + 1 > StormLevelOverload)
                {
                    cloudManager.GetComponent<CloudManager>().DispelThunderhead();
                }
            }
        }

        // Reset the timer
        stormTimerSlider.value = 0;
        stormTimerSlider.GetComponent<CanvasGroup>().alpha = 0;
        curAction = Action.DEFAULT;
        yield return null;
    }

    // Gather all spawned gale clouds and merge them into the storm cloud
    IEnumerator GatherGaleClouds()
    {
        curAction = Action.GATHER;

        // Set up container to keep track of spawned gale clouds
        int posCount = cloudManager.GetComponent<CloudManager>().CardinalPos.Count;
        List<GameObject> spawnedGaleClouds =  new List<GameObject>();
        List<Vector3> galeCloudPositions = new List<Vector3>();

        // Get all of the spawned gale clouds
        for (int posIndex = 0; posIndex < posCount; posIndex++)
        {
            Vector2 curPos = cloudManager.GetComponent<CloudManager>().CardinalPos[posIndex];
            GameObject spawnedCloud = cloudManager.GetComponent<CloudManager>().GetThunderheadFromGale(curPos, true);
            if (spawnedCloud)
            {
                spawnedGaleClouds.Add(spawnedCloud);
                galeCloudPositions.Add(spawnedCloud.transform.localPosition);
            }
            yield return null;
        }

        // Move spawned gale clouds to storm cloud position
        Coroutine[] movingClouds = new Coroutine[spawnedGaleClouds.Count];
        for (int i = 0; i < spawnedGaleClouds.Count; i++)
        {
            movingClouds[i] = StartCoroutine(MoveGaleToStorm(spawnedGaleClouds[i], galeCloudPositions[i]));
        }

        // Wait for the gale clouds to finish moving
        for (int i = 0; i < movingClouds.Length; i++)
        {
            yield return movingClouds[i];
        }

        // Merge spawned gale clouds into storm cloud
        for (int i = 0; i < spawnedGaleClouds.Count; i++)
        {
            GameObject galeCloud = spawnedGaleClouds[i];
            cloudManager.GetComponent<CloudManager>().MoveThunderHead(Vector2.zero, ref galeCloud);
            yield return null;
        }

        // Change storm cloud to storm front if it is at a sustainable level
        GameObject stormCloud = cloudManager.GetComponent<CloudManager>().StormCloudRef;
        if (stormCloud && stormCloud.GetComponent<Thunderhead>().GaleLvl >= StormLevelSustainable)
        {
            stormCloud.GetComponent<Thunderhead>().StormFront(stormFrontRange);
        }
        curAction = Action.DEFAULT;
        yield return null;
    }

    // Move spawned gale cloud to storm cloud position
    IEnumerator MoveGaleToStorm (GameObject galeCloud, Vector3 startPos)
    {
        for (float curTime = 0; curTime < stormGatherTime; curTime += Time.deltaTime)
        {
            galeCloud.transform.localPosition = Vector3.Slerp(startPos, transform.localPosition, curTime / stormGatherTime);
            yield return null;
        }
        galeCloud.transform.localPosition = transform.localPosition;
        yield return null;
    }
}
