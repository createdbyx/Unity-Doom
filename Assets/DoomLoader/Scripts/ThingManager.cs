using UnityEngine;
using System.Collections.Generic;

public class ThingManager : MonoBehaviour 
{
    public static ThingManager Instance;

    public List<ThingController> ThingPrefabs = new List<ThingController>();

    private Dictionary<int, GameObject> _things = new Dictionary<int, GameObject>();

	private List<ThingController> activeThings = new List<ThingController>();

    void Awake()
    {
        Instance = this;

        foreach (ThingController tc in ThingPrefabs)
            _things.Add(tc.thingID, tc.gameObject);
    }

    public void CreateThings(bool deathmatch)
    {
        GameObject holder = new GameObject("MapThings");
        holder.transform.SetParent(transform);

        foreach(Thing t in MapLoader.things)
        {
            if (!deathmatch)
                if ((t.flags & (1 << 4)) != 0)
                    continue;

            if (!_things.ContainsKey(t.thingType))
            {
                Debug.Log("Unknown thing type (" + t.thingType + ")");
                continue;
            }

            GameObject thingObject = Instantiate(_things[t.thingType]);
            thingObject.transform.SetParent(holder.transform);

            ThingController tc = thingObject.GetComponent<ThingController>();
            if (tc != null)
            {
                tc.thing = t;
                tc.Init();
                activeThings.Add(tc);
            }
        }
    }
}
