using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    // Variables for managing spawned clouds
    public int maxCloudCnt = 3;
    private int curGaleCloudCnt = 0;
    public int CurGaleCloudCnt { get { return curGaleCloudCnt; } }
    public float dipVal = 0.5f;
    public GameObject thunderheadPrefab;

    // Vector resulting from combined gale vectors of thunderheads
    public Vector3 CombinedGaleVector { get; set; }

    // Containers for keeping track of spawned thunderheads
    private Dictionary<Vector2, GameObject> thunderheadPos = new Dictionary<Vector2, GameObject>();
    public GameObject StormCloudRef { get; set; }

    // List of keys for thunderheadPos container
    public List<Vector2> CardinalPos { get; private set; }

    // Reference to other game objects
    private GameObject compassCenter;
    private GameObject wizardObject;

    // Use this for initialization
    void Start()
    {
        // Initialize combined gale vector
        CombinedGaleVector = new Vector3(0, 0, 0);

        // Establish reference to compass center
        compassCenter = GameObject.Find("Compass Center");
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Establish reference to wizard
        wizardObject = GameObject.Find("Wizard Object");
        if (wizardObject == null)
        {
            Debug.LogError("Wizard Object object not found", wizardObject);
        }

        // Initialize thunderheadPos
        thunderheadPos.Add(new Vector2(0, 1), null);    // position N
        thunderheadPos.Add(new Vector2(1, 1), null);    // position NE
        thunderheadPos.Add(new Vector2(1, 0), null);    // position E
        thunderheadPos.Add(new Vector2(1, -1), null);   // position SE
        thunderheadPos.Add(new Vector2(0, -1), null);   // position S
        thunderheadPos.Add(new Vector2(-1, -1), null);  // position SW
        thunderheadPos.Add(new Vector2(-1, 0), null);   // position W
        thunderheadPos.Add(new Vector2(-1, 1), null);   // position NW

        // Initialize list of keys
        CardinalPos = new List<Vector2>(thunderheadPos.Keys);

        // Initialize the storm cloud reference
        StormCloudRef = null;
    }

    // Return whether a thunderhead is at pos
    public bool IsThunderheadAtPos(Vector2 pos)
    {
        if (thunderheadPos.ContainsKey(pos))
        {
            return thunderheadPos[pos] != null;
        }
        return false;
    }

    // Get the thunderhead in gale mode to move to storm mode
    public GameObject GetThunderheadFromGale(Vector2 pos, bool toStorm = false)
    {
        if (IsThunderheadAtPos(pos))
        {
            if (toStorm)
            {
                CombinedGaleVector -= thunderheadPos[pos].GetComponent<Thunderhead>().GaleVector;
                GameObject galeCloud = thunderheadPos[pos];
                thunderheadPos[pos] = null;
                return galeCloud;
            }
            else
            {
                return thunderheadPos[pos];
            }
        }
        else
        {
            return null;
        }
    }

    // Summon a new thunderhead
    public void SpawnThunderhead(Vector2 cardinalPos = default(Vector2))
    {
        Vector3 wizardPos = wizardObject.transform.position;
        GameObject newThunderhead;
        switch (wizardObject.GetComponent<Wizard>().CurMode)
        {
            // Gale mode
            case Wizard.Mode.GALE:
                if (curGaleCloudCnt < maxCloudCnt && thunderheadPos.ContainsKey(cardinalPos) && thunderheadPos[cardinalPos] == null)
                {
                    // Instantiate gale thunderhead at position of wizard
                    newThunderhead = Instantiate(thunderheadPrefab, wizardPos, Quaternion.identity, transform);

                    // Rotate thunderhead so it looks at the center of the cloud rail
                    newThunderhead.transform.LookAt(compassCenter.transform, Vector3.up);

                    // Lower the thunderhead by the dip value
                    newThunderhead.transform.position = new Vector3(wizardPos.x, wizardPos.y - dipVal, wizardPos.z);

                    // Add thunderhead to container
                    thunderheadPos[cardinalPos] = newThunderhead;
                    curGaleCloudCnt++;

                    // Update combined gale vector
                    CombinedGaleVector += newThunderhead.GetComponent<Thunderhead>().GaleVector;
                }
                break;

            // Storm mode
            case Wizard.Mode.STORM:
                // Instantiate storm thunderhead at position of wizard
                StormCloudRef = Instantiate(thunderheadPrefab, wizardPos, Quaternion.identity, transform);                
                break;

            default:
                break;
        }
    }

    // Pick up or place thunderheads
    public GameObject MoveThunderHead(Vector2 cardinalPos, ref GameObject heldCloud)
    {
        switch (wizardObject.GetComponent<Wizard>().CurMode)
        {
            // Gale Mode
            case Wizard.Mode.GALE:
                if (!heldCloud)
                {
                    // Pick up cloud at cardinal position
                    if (IsThunderheadAtPos(cardinalPos))
                    {
                        GameObject cloudToGrab = thunderheadPos[cardinalPos];

                        // Update combined gale vector
                        CombinedGaleVector -= cloudToGrab.GetComponent<Thunderhead>().GaleVector;

                        // Raise the thunderhead by the dip value
                        Vector3 newPos = cloudToGrab.transform.position;
                        cloudToGrab.transform.position = new Vector3(newPos.x, newPos.y + dipVal, newPos.z);

                        // Remove the thunderhead from the container
                        thunderheadPos[cardinalPos] = null;
                        cloudToGrab.GetComponent<Thunderhead>().IsHeld = true;
                        cloudToGrab.transform.parent = wizardObject.transform;
                        return cloudToGrab;
                    }
                    return null;
                }
                else
                {
                    // Merge held cloud into cloud at position
                    if (IsThunderheadAtPos(cardinalPos))
                    {
                        thunderheadPos[cardinalPos].GetComponent<Thunderhead>().Merge(heldCloud);
                        heldCloud = null;
                        curGaleCloudCnt--;

                        // Dispel cloud if new level exceeds overload level
                        if (wizardObject.GetComponent<GaleMode>().CloudLevelOverload < thunderheadPos[cardinalPos].GetComponent<Thunderhead>().GaleLvl)
                        {
                            DispelThunderhead(cardinalPos);
                        }
                    }
                    // Place cloud at position
                    else
                    {
                        // Put held cloud back into thunderhead container
                        heldCloud.transform.parent = transform;
                        thunderheadPos[cardinalPos] = heldCloud;
                        GameObject placedCloud = thunderheadPos[cardinalPos];
                        curGaleCloudCnt--;
                        
                        // Clear the wizard's held cloud object
                        heldCloud = null;
                        placedCloud.GetComponent<Thunderhead>().IsHeld = false;

                        // Update the gale vector of the newly positioned thunderhead
                        placedCloud.transform.LookAt(compassCenter.transform.position, Vector3.up);
                        placedCloud.GetComponent<Thunderhead>().GaleVector = compassCenter.transform.position - placedCloud.transform.position;

                        // Update combined gale vector
                        CombinedGaleVector += placedCloud.GetComponent<Thunderhead>().GaleVector;

                        // Lower the thunderhead by the dip value
                        Vector3 newPos = placedCloud.transform.position;
                        placedCloud.transform.position = new Vector3(newPos.x, newPos.y - dipVal, newPos.z);
                    }
                }
                break;
            
            // Storm Mode
            case Wizard.Mode.STORM:
                // Make held cloud the storm cloud
                if (!StormCloudRef)
                {
                    StormCloudRef = heldCloud;
                    StormCloudRef.transform.parent = transform;
                }
                // Merge held cloud into storm cloud
                else
                {
                    StormCloudRef.GetComponent<Thunderhead>().Merge(heldCloud);

                    // If the new gale level goes above the overload level, the storm cloud "explodes"
                    if (wizardObject.GetComponent<StormMode>().StormLevelOverload < StormCloudRef.GetComponent<Thunderhead>().GaleLvl)
                    {
                        DispelThunderhead();
                    }
                }
                curGaleCloudCnt--;
                heldCloud = null;
                break;
            default:
                break;
        }
        return null;
    }

    // Dispel a thunderhead
    public void DispelThunderhead(Vector2 cardinalPos = default(Vector2))
    {
        switch (wizardObject.GetComponent<Wizard>().CurMode)
        {
            // Gale mode
            case Wizard.Mode.GALE:
                if (curGaleCloudCnt > 0 && thunderheadPos.ContainsKey(cardinalPos))
                {
                    // Update combined gale vector
                    CombinedGaleVector -= thunderheadPos[cardinalPos].GetComponent<Thunderhead>().GaleVector;

                    // Dispel thunderhead
                    Destroy(thunderheadPos[cardinalPos]);
                    thunderheadPos[cardinalPos] = null;
                    curGaleCloudCnt--;
                }
                break;

            // Storm mode
            case Wizard.Mode.STORM:
                // Dispel storm thunderhead
                Destroy(StormCloudRef);
                break;

            default:
                break;
        }
    }

    // Dispel all spawned thunderheads
    public void DispelAll()
    {
        CombinedGaleVector = Vector3.zero;
        curGaleCloudCnt = 0;

        // Dispel all thunderclouds
        for (int i = 0; i < CardinalPos.Count; i++)
        {
            Destroy(thunderheadPos[CardinalPos[i]]);
            thunderheadPos[CardinalPos[i]] = null;
        }
    }
}
