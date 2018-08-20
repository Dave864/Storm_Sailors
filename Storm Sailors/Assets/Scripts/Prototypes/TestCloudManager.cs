using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCloudManager : MonoBehaviour {
    // Variables for managing spawned clouds
    public int maxCloudCnt = 3;
    private int curCloudCnt = 0;
    public float dipVal = 0.5f; // TODO: put a limit on how big this can be in editor (after jam)
    public GameObject thunderheadPrefab;

    // Vector resulting from combined gale vectors of thunderheads
    [HideInInspector] public Vector3 CombinedGaleVector { get; set; }

    // Container for keeping track of spawned thunderheads
    private Dictionary<Vector2, GameObject> thunderheadPos = new Dictionary<Vector2, GameObject>();

    // Reference to other game objects
    private GameObject railCenter;
    private GameObject wizard;

	// Use this for initialization
	void Start()
    {
        // Keep the cloud max count from exceeding the number of positions
        // TODO: placeholder until editor scripts are made (after jam)
        if (maxCloudCnt > 8)
        {
            maxCloudCnt = 8;
        }

        // Initialize combined gale vector
        CombinedGaleVector = new Vector3(0, 0, 0);

        // Establish reference to game objects
        railCenter = GameObject.Find("Cloud Rail");
        wizard = GameObject.Find("Wizard Entity");

        // Initialize thunderheadPos
        thunderheadPos.Add(new Vector2(0, 1), null);    // position N
        thunderheadPos.Add(new Vector2(1, 1), null);    // position NE
        thunderheadPos.Add(new Vector2(1, 0), null);    // position E
        thunderheadPos.Add(new Vector2(1, -1), null);   // position SE
        thunderheadPos.Add(new Vector2(0, -1), null);   // position S
        thunderheadPos.Add(new Vector2(-1, -1), null);  // position SW
        thunderheadPos.Add(new Vector2(-1, 0), null);   // position W
        thunderheadPos.Add(new Vector2(-1, 1), null);   // position NW
    }

    // Return whether a thunderhead is at pos
    public bool ThunderheadAtPos(Vector2 pos)
    {
        if (thunderheadPos.ContainsKey(pos))
        {
            return thunderheadPos[pos] != null;
        }
        return false;
    }

    // Summon a new thunderhead at pos
    public void SpawnThunderhead(Vector2 cardinalPos)
    {
        if (curCloudCnt < maxCloudCnt)
        {
            if (thunderheadPos.ContainsKey(cardinalPos))
            {
                if (thunderheadPos[cardinalPos] == null)
                {
                    Vector3 wizardPos = wizard.transform.position;

                    // Instantiate thunderhead at position of wizard
                    GameObject newThunderhead = Instantiate(thunderheadPrefab, wizard.transform.position, Quaternion.identity, transform);
                    newThunderhead.transform.position = new Vector3(wizard.transform.position.x, wizard.transform.position.y - dipVal, wizard.transform.position.z);

                    // Rotate thunderhead so it looks at the center of the cloud rail
                    newThunderhead.transform.LookAt(railCenter.transform, Vector3.up);

                    // Add thunderhead to container
                    thunderheadPos[cardinalPos] = newThunderhead;
                    curCloudCnt++;

                    // Update combined gale vector
                    CombinedGaleVector += newThunderhead.GetComponent<TestThunderhead>().GaleVector;
                }
            }
        }
    }

    // Dispel a thunderhead at pos
    public void DispelThunderhead(Vector2 cardinalPos)
    {
        if (curCloudCnt > 0)
        {
            if (thunderheadPos.ContainsKey(cardinalPos))
            {
                // Update combined gale vector
                CombinedGaleVector -= thunderheadPos[cardinalPos].GetComponent<TestThunderhead>().GaleVector;
                // Dispel thunderhead
                Destroy(thunderheadPos[cardinalPos]);
                thunderheadPos[cardinalPos] = null;
                curCloudCnt--;
            }
        }
    }
}
