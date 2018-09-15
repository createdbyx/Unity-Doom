using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

public class MapLoader : MonoBehaviour 
{
    public static MapLoader Instance;

    void Awake()
    {
        Instance = this;
    }

    public static bool IsSkyTexture(string textureName)
    {
        if (textureName == "F_SKY1")
            return true;

        return false;
    }

    public const int sizeDividor = 32;
    public const int flatUVdividor = 64 / sizeDividor; //all Doom flats are 64x64
    public const float _4units = 4f / sizeDividor;
    public const float _8units = 8f / sizeDividor;

    public static List<Vertex> vertices;
    public static List<Sector> sectors;
    public static List<Linedef> linedefs;
    public static List<Sidedef> sidedefs;
    public static List<Thing> things;

    public static Lump things_lump;
    public static Lump linedefs_lump;
    public static Lump sidedefs_lump;
    public static Lump vertexes_lump;
    public static Lump segs_lump;
    public static Lump ssectors_lump;
    public static Lump nodes_lump;
    public static Lump sectors_lump;
    public static Lump reject_lump;
    public static Lump blockmap_lump;

    public static int minX = int.MaxValue;
    public static int maxX = int.MinValue;
    public static int minY = int.MaxValue;
    public static int maxY = int.MinValue;
    public static int minZ = int.MaxValue;
    public static int maxZ = int.MinValue;

    public bool Load(string mapName)
    {
        if (WadLoader.lumps.Count == 0)
        {
            Debug.LogError("MapLoader: Load: WadLoader.lumps == 0");
            return false;
        }

        //lumps
        {
            int i = 0;
            foreach (Lump l in WadLoader.lumps)
            {
                if (l.lumpName.Equals(mapName))
                    goto found;

                i++;
            }

            Debug.Log("MapLoader: Load: Could not find map name \"" + mapName + "\"");
            return false;

        found:
            things_lump = WadLoader.lumps[++i];
            linedefs_lump = WadLoader.lumps[++i];
            sidedefs_lump = WadLoader.lumps[++i];
            vertexes_lump = WadLoader.lumps[++i];
            segs_lump = WadLoader.lumps[++i];
            ssectors_lump = WadLoader.lumps[++i];
            nodes_lump = WadLoader.lumps[++i];
            sectors_lump = WadLoader.lumps[++i];
            reject_lump = WadLoader.lumps[++i];
            blockmap_lump = WadLoader.lumps[++i];
        }

        //things
        {
            int num = things_lump.data.Length / 10;
            things = new List<Thing>(num);

            for (int i = 0, n = 0; i < num; i++)
            {
                short x = (short)(things_lump.data[n++] | (short)things_lump.data[n++] << 8);
                short y = (short)(things_lump.data[n++] | (short)things_lump.data[n++] << 8);
                int facing = (int)(things_lump.data[n++] | (int)things_lump.data[n++] << 8);
                int thingtype = (int)things_lump.data[n++] | ((int)things_lump.data[n++]) << 8;
                int flags = (int)things_lump.data[n++] | ((int)things_lump.data[n++]) << 8;

                things.Add(new Thing(x, y, facing, thingtype, flags));
            }
        }

        //vertices
        {
            int num = vertexes_lump.data.Length / 4;
            vertices = new List<Vertex>(num);

            for (int i = 0, n = 0; i < num; i++)
            {
                short x = (short)(vertexes_lump.data[n++] | (short)vertexes_lump.data[n++] << 8);
                short y = (short)(vertexes_lump.data[n++] | (short)vertexes_lump.data[n++] << 8);

                vertices.Add(new Vertex(x, y));

                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }
        }

        //sectors
        {
            int num = sectors_lump.data.Length / 26;
            sectors = new List<Sector>(num);

            for (int i = 0, n = 0; i < num; i++)
            {
                short hfloor = (short)(sectors_lump.data[n++] | (short)sectors_lump.data[n++] << 8);
                short hceil = (short)(sectors_lump.data[n++] | (short)sectors_lump.data[n++] << 8);

                string tfloor = Encoding.ASCII.GetString(new byte[]
                {
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++]
                }).TrimEnd('\0').ToUpper();

                string tceil = Encoding.ASCII.GetString(new byte[]
                {
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++],
                    sectors_lump.data[n++]
                }).TrimEnd('\0').ToUpper();

                int bright = sectors_lump.data[n++] | ((int)sectors_lump.data[n++]) << 8;
                int special = sectors_lump.data[n++] | ((int)sectors_lump.data[n++]) << 8;
                int tag = sectors_lump.data[n++] | ((int)sectors_lump.data[n++]) << 8;

                sectors.Add(new Sector(hfloor, hceil, tfloor, tceil, special, tag, bright));

                if (hfloor < minZ) minZ = hfloor;
                if (hceil > maxZ) maxZ = hceil;
            }
        }

        //sidedefs
        {
            int num = sidedefs_lump.data.Length / 30;
            sidedefs = new List<Sidedef>(num);

            for (int i = 0, n = 0; i < num; i++)
            {
                short offsetx = (short)(sidedefs_lump.data[n++] | (short)sidedefs_lump.data[n++] << 8);
                short offsety = (short)(sidedefs_lump.data[n++] | (short)sidedefs_lump.data[n++] << 8);

                string thigh = Encoding.ASCII.GetString(new byte[]
                {
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++]
                }).TrimEnd('\0').ToUpper();

                string tlow = Encoding.ASCII.GetString(new byte[]
                {
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++]
                }).TrimEnd('\0').ToUpper();

                string tmid = Encoding.ASCII.GetString(new byte[]
                {
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++],
                    sidedefs_lump.data[n++]
                }).TrimEnd('\0').ToUpper();

                int sector = (int)(sidedefs_lump.data[n++] | (int)sidedefs_lump.data[n++] << 8);

                sidedefs.Add(new Sidedef(sectors[sector], offsetx, offsety, thigh, tlow, tmid));
            }
        }

        //linedefs
        {
            int num = linedefs_lump.data.Length / 14;
            linedefs = new List<Linedef>(num);

            for (int i = 0, n = 0; i < num; i++)
            {
                int v1 = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;
                int v2 = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;
                int flags = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;
                int action = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;
                int tag = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;
                int s1 = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;
                int s2 = linedefs_lump.data[n++] | ((int)linedefs_lump.data[n++]) << 8;

                Linedef line = new Linedef(vertices[v1], vertices[v2], flags, action, tag);
                linedefs.Add(line);

                if (s1 != ushort.MaxValue)
                    sidedefs[s1].SetLine(line, true);

                if (s2 != ushort.MaxValue)
                    sidedefs[s2].SetLine(line, false);
            }
        }

        //SKY FIX
        {
            foreach (Linedef l in linedefs)
            {
                if (l.Back == null)
                    continue;

                if (IsSkyTexture(l.Front.Sector.ceilingTexture))
                    if (IsSkyTexture(l.Back.Sector.ceilingTexture))
                    {
                        l.Front.tHigh = "F_SKY1";
                        l.Back.tHigh = "F_SKY1";
                    }

                if (IsSkyTexture(l.Front.Sector.floorTexture))
                    if (IsSkyTexture(l.Back.Sector.floorTexture))
                    {
                        l.Front.tLow = "F_SKY1";
                        l.Back.tLow = "F_SKY1";
                    }
            }
        }

        //modify geometry to accomodate expected changes
        foreach (Linedef l in linedefs)
        {
            if (l.lineType == 0)
                continue;

            switch (l.lineType)
            {
                default:
                    break;

                case 1:
                case 26: //keycard doors
                case 27:
                case 28:
                    {
                        if (l.Back != null)
                            if (l.Back.Sector.maximumCeilingHeight == l.Back.Sector.ceilingHeight
                                || l.Front.Sector.ceilingHeight - _4units < l.Back.Sector.maximumCeilingHeight)
                                l.Back.Sector.maximumCeilingHeight = l.Front.Sector.ceilingHeight - _4units;
                    }
                    break;

                case 36:
                    {
                        if (!Sector.TaggedSectors.ContainsKey(l.lineTag))
                            break;

                        foreach (Sector sector in Sector.TaggedSectors[l.lineTag])
                            foreach (Sidedef s in sector.Sidedefs)
                                if (s.Line.Front.Sector.floorHeight +_8units < sector.minimumFloorHeight)
                                    sector.minimumFloorHeight = s.Line.Front.Sector.floorHeight + _8units;
                    }
                    break;

                case 88:
                    {
                        if (!Sector.TaggedSectors.ContainsKey(l.lineTag))
                            break;

                        foreach (Sector sector in Sector.TaggedSectors[l.lineTag])
                            foreach (Sidedef s in sector.Sidedefs)
                                if (s.Line.Front.Sector.floorHeight < sector.minimumFloorHeight)
                                    sector.minimumFloorHeight = s.Line.Front.Sector.floorHeight;
                    }
                    break;
            }

        }

        Debug.Log("Loaded map \"" + mapName + "\"");

        return true;
    }

    public void ApplyLinedefBehavior()
    {
        Transform holder = new GameObject("DynamicMeshes").transform;
        holder.transform.SetParent(transform);

        foreach (Linedef l in linedefs)
        {
            if (l.lineType == 0)
                continue;

            switch (l.lineType)
            {
                default:
                    break;

                case 1:
                case 26: //keycard doors
                case 27:
                case 28:
                    {
                        if (l.TopFrontObject == null)
                            break;

                        l.Back.Sector.ceilingObject.transform.SetParent(holder);
                        Door1LinedefController script = l.TopFrontObject.AddComponent<Door1LinedefController>();
                        script.Init(l);
                    }
                    break;

                case 36:
                    {
                        if (!Sector.TaggedSectors.ContainsKey(l.lineTag))
                            break;

                        List<Floor36Controller> linked = new List<Floor36Controller>();
                        foreach (Sector sector in Sector.TaggedSectors[l.lineTag])
                        {
                            sector.floorObject.transform.SetParent(holder);

                            Floor36Controller script = sector.floorObject.GetComponent<Floor36Controller>();
                            if (script == null)
                            {
                                script = sector.floorObject.AddComponent<Floor36Controller>();
                                script.Init(sector);
                            }
                            linked.Add(script);
                        }

                        BoxCollider mc = Mesher.Instance.CreateLineCollider(
                            l,
                            Mathf.Max(l.Front.Sector.floorHeight, l.Back.Sector.floorHeight),
                            Mathf.Min(l.Front.Sector.ceilingHeight, l.Back.Sector.ceilingHeight),
                            "Tag_" + l.lineTag + "_trigger",
                            holder
                            );

                        if (mc == null)
                            break;

                        mc.isTrigger = true;
                        LineTrigger lt = mc.gameObject.AddComponent<LineTrigger>();
                        lt.TriggerAction = (c) =>
                        {
                            if (c.gameObject != WadLoader.Instance.PlayerObject)
                                return;

                            foreach (Floor36Controller lc in linked)
                                if (lc.CurrentState == Floor36Controller.State.AtTop)
                                    lc.CurrentState = Floor36Controller.State.Lowering;
                        };
                    }
                    break;

                case 48:
                    {
                        foreach (GameObject g in l.gameObjects)
                            if (g != null)
                                g.AddComponent<ScrollLeftAnimation>();
                    }
                    break;

                case 88:
                    {
                        if (!Sector.TaggedSectors.ContainsKey(l.lineTag))
                            break;

                        List<Platform88Controller> linked = new List<Platform88Controller>(); 
                        foreach (Sector sector in Sector.TaggedSectors[l.lineTag])
                        {
                        	// hacky handling for a scene reload - I don't actually understand why we're getting here *after* we've cleared the linedefs it's looping through
							if (sector.floorObject == null) return;

                            sector.floorObject.transform.SetParent(holder);

                            Platform88Controller script = sector.floorObject.GetComponent<Platform88Controller>();
                            if (script == null)
                            {
                                script = sector.floorObject.AddComponent<Platform88Controller>();
                                script.Init(sector);
                            }

                            linked.Add(script);
                        }

                        BoxCollider mc = Mesher.Instance.CreateLineCollider(
                            l,
                            Mathf.Max(l.Front.Sector.floorHeight, l.Back.Sector.floorHeight),
                            Mathf.Min(l.Front.Sector.ceilingHeight, l.Back.Sector.ceilingHeight),
                            "Tag_" + l.lineTag + "_trigger",
                            holder
                            );

                        if (mc == null)
                            break;

                        mc.isTrigger = true;
                        LineTrigger lt = mc.gameObject.AddComponent<LineTrigger>();
                        lt.TriggerAction = (c) => 
                        {
                            if (c.gameObject != WadLoader.Instance.PlayerObject)
                                return;

                            foreach (Platform88Controller lc in linked)
                            {
                                if (lc.CurrentState == Platform88Controller.State.AtTop)
                                {
                                    lc.CurrentState = Platform88Controller.State.Lowering;
                                    lc.waitTime = 4f;
                                }
                            }
                        };
                    }
                    break;
            }
        }
    }
}
