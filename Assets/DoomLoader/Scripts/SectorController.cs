using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SectorController : MonoBehaviour
{
    public Sector sector;

    public List<ThingController> StaticThings = new List<ThingController>();
    public LinkedList<ThingController> DynamicThings = new LinkedList<ThingController>();

    MeshRenderer mr;

    public void Init()
    {
        mr = GetComponent<MeshRenderer>();

        if (sector.specialType == 0 && sector.tag == 0)
        {
            enabled = false;
            return;
        }

        //sector types are only applied on floors (which in turn can control the ceiling)
        if (sector.floorObject == gameObject && sector.specialType > 0)
            switch (sector.specialType)
            {
                case 1: //blink random
                    {
                        float random = UnityEngine.Random.Range(.2f, .8f);
                        float time = 0;
                        bool flip = false;
                        float original = sector.brightness;
                        float neighbor = original;
                        foreach (Sidedef s in sector.Sidedefs)
                            if (s.Other != null)
                                if (s.Other.Sector.brightness < neighbor)
                                    neighbor = s.Other.Sector.brightness;

                        parameters = new object[5] { time, flip, original, neighbor, random };
                    }
                    UpdateActions.Add(new Action(() =>
                    {
                        float time = (float)parameters[0];
                        time += Time.deltaTime;

                        if (time >= (float)parameters[4])
                        {
                            bool flip = (bool)parameters[1];
                            time = 0;
                            flip = !flip;

                            ChangeBrightness(flip ? (float)parameters[2] : (float)parameters[3]);
                            parameters[1] = flip;

                            parameters[4] = UnityEngine.Random.Range(.2f, .8f);
                        }
                        parameters[0] = time;
                    }));
                    break;

                case 17: //flicker randomly
                    {
                        float random = UnityEngine.Random.Range(.05f, .15f);
                        float time = 0;
                        bool flip = false;
                        float original = sector.brightness;
                        float neighbor = original;
                        foreach (Sidedef s in sector.Sidedefs)
                            if (s.Other != null)
                                if (s.Other.Sector.brightness < neighbor)
                                    neighbor = s.Other.Sector.brightness;

                        parameters = new object[5] { time, flip, original, neighbor, random };
                    }
                    UpdateActions.Add(new Action(() =>
                    {
                        float time = (float)parameters[0];
                        time += Time.deltaTime;

                        if (time >= (float)parameters[4])
                        {
                            bool flip = (bool)parameters[1];
                            time = 0;
                            flip = !flip;

                            ChangeBrightness(flip ? (float)parameters[2] : (float)parameters[3]);
                            parameters[1] = flip;

                            parameters[4] = UnityEngine.Random.Range(.05f, .15f);
                        }
                        parameters[0] = time;
                    }));
                    break;

                case 8: //oscillates
                    UpdateActions.Add(new Action(() =>
                    {
                        ChangeBrightness(Mathf.Sin(Time.time * 4) * .25f + .75f);
                    }));
                    break;

                case 2: //blink 0.5second
                    {
                        float time = UnityEngine.Random.Range(0, .5f);
                        bool flip = false;
                        float original = sector.brightness;
                        float neighbor = original;
                        foreach (Sidedef s in sector.Sidedefs)
                            if (s.Other != null)
                                if (s.Other.Sector.brightness < neighbor)
                                    neighbor = s.Other.Sector.brightness;

                        parameters = new object[4] { time, flip, original, neighbor };
                    }
                    UpdateActions.Add(new Action(() =>
                    {
                        float time = (float)parameters[0];
                        time += Time.deltaTime;

                        if (time >= .5f)
                        {
                            bool flip = (bool)parameters[1];
                            time = 0;
                            flip = !flip;

                            ChangeBrightness(flip ? (float)parameters[2] : (float)parameters[3]);
                            parameters[1] = flip;
                        }
                        parameters[0] = time;
                    }));
                    break;
                case 12: //blink 0.5second synchronized
                    {
                        bool flip = false;
                        float original = sector.brightness;
                        float neighbor = original;
                        foreach (Sidedef s in sector.Sidedefs)
                            if (s.Other != null)
                                if (s.Other.Sector.brightness < neighbor)
                                    neighbor = s.Other.Sector.brightness;

                        parameters = new object[3] { original, neighbor, flip };
                    }
                    UpdateActions.Add(new Action(() =>
                    {
                        bool flip = Time.time % 1f > .5f;
                        if (flip != (bool)parameters[2])
                        {
                            parameters[2] = flip;
                            ChangeBrightness(flip ? (float)parameters[0] : (float)parameters[1]);
                        }
                    }));
                    break;
                case 3: //blink 1.0second
                    {
                        float time = UnityEngine.Random.Range(0, 1f);
                        bool flip = false;
                        float original = sector.brightness;
                        float neighbor = original;
                        foreach (Sidedef s in sector.Sidedefs)
                            if (s.Other != null)
                                if (s.Other.Sector.brightness < neighbor)
                                    neighbor = s.Other.Sector.brightness;

                        parameters = new object[4] { time, flip, original, neighbor };
                    }
                    UpdateActions.Add(new Action(() =>
                    {
                        float time = (float)parameters[0];
                        time += Time.deltaTime;

                        if (time >= 1f)
                        {
                            bool flip = (bool)parameters[1];
                            time = 0;
                            flip = !flip;

                            ChangeBrightness(flip ? (float)parameters[2] : (float)parameters[3]);
                            parameters[1] = flip;
                        }
                        parameters[0] = time;
                    }));
                    break;
                case 13: //blink 1.0second synchronized
                    {
                        bool flip = false;
                        float original = sector.brightness;
                        float neighbor = original;
                        foreach (Sidedef s in sector.Sidedefs)
                            if (s.Other != null)
                                if (s.Other.Sector.brightness < neighbor)
                                    neighbor = s.Other.Sector.brightness;

                        parameters = new object[3] { original, neighbor, flip };
                    }
                    UpdateActions.Add(new Action(() =>
                    {
                        bool flip = Time.time % 2f > 1f;
                        if (flip != (bool)parameters[2])
                        {
                            parameters[2] = flip;
                            ChangeBrightness(flip ? (float)parameters[0] : (float)parameters[1]);
                        }
                    }));
                    break;

                default:
                    //Debug.Log("Unknown sector type: " + sector.specialType);
                    break;
            }
    }

    object[] parameters = new object[0];
    List<Action> UpdateActions = new List<Action>();

    void Update()
    {
        foreach (Action action in UpdateActions)
            action.Invoke();
    }

    public void ChangeBrightness(float value)
    {
        MaterialPropertyBlock materialProperties = new MaterialPropertyBlock();
        mr.GetPropertyBlock(materialProperties);
        materialProperties.SetColor("_Color", Color.white * value);
        mr.SetPropertyBlock(materialProperties);

        if (sector.ceilingObject == gameObject)
            return;

        sector.ceilingObject.GetComponent<SectorController>().ChangeBrightness(value);

        foreach (ThingController tc in StaticThings)
            tc.SetBrightness(value);

        foreach (ThingController tc in DynamicThings)
            tc.SetBrightness(value);

        foreach(Sidedef s in sector.Sidedefs)
            if (s.gameObject != null)
            {
                MeshRenderer mr = s.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    MaterialPropertyBlock properties = new MaterialPropertyBlock();
                    mr.GetPropertyBlock(properties);
                    properties.SetColor("_Color", Color.white * value);
                    mr.SetPropertyBlock(properties);
                }
            }
    }
}
