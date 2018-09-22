using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

public class WadLoader : MonoBehaviour 
{
    public static WadLoader Instance;

    void Awake()
    {
        Instance = this;
    }

    public Color ambientLightColor = Color.white;
    public bool deathmatch;
    public static List<Lump> lumps = new List<Lump>();
    public string autoloadWad = "Doom1.WAD";
    public int autoLoadEpisode = 1;
    public int currentMission = 1;
    public GameObject PlayerObject;

    void Start() {
		if (!LoadWad(autoloadWad))
            return;

		TextureLoader.Instance.LoadAndBuildAll();
		LoadMap();
		Invoke("SetPlayerDisabled", 0.1f);
    }

    public bool LoadMap() {
		return LoadMap(autoLoadEpisode, currentMission);
    }

    public bool LoadMap(int episode, int mission)
    {
		if (MapLoader.vertices != null && MapLoader.vertices.Count > 0) Doom.UnloadCurrentMap();

        Shader.SetGlobalColor("_AMBIENTLIGHT", ambientLightColor);

		autoLoadEpisode = episode;
		currentMission = mission;

		string map = "E" + autoLoadEpisode.ToString() + "M" + currentMission.ToString();

        if (string.IsNullOrEmpty(autoloadWad))
            return false;

		if (MapLoader.Instance.Load(map))  {
            Mesher.Instance.CreateMeshes();

            MapLoader.Instance.ApplyLinedefBehavior();

            ThingManager.Instance.CreateThings(deathmatch);

            if (PlayerStart.PlayerStarts[0] == null)
                Debug.LogError("PlayerStart1 == null");
            else
            {
                PlayerObject.transform.position = PlayerStart.PlayerStarts[0].transform.position;
                PlayerObject.transform.rotation = PlayerStart.PlayerStarts[0].transform.rotation;
            }

			Doom.isLoaded = true;
            //PlayerObject.GetComponent<AudioSource>().clip = SoundLoader.LoadSound("DSPISTOL");
            //PlayerObject.GetComponent<AudioSource>().Play();
        }
        return Doom.isLoaded;
	}

	public void SetCurrentMission(int input) {
		currentMission = input;
	}

	public void SetAutoLoadEpisode(int input) {
		autoLoadEpisode = input;
	}

    public void ReloadScene() {
    	Doom.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    public bool LoadWad(string file)
    {
        string path = Application.streamingAssetsPath + "/" + file;
        if (!File.Exists(path))
        {
            Debug.LogError("WadLoader: Load: File \"" + file + "\" does not exist!");
            return false;
        }

        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        BinaryReader reader = new BinaryReader(stream, Encoding.ASCII);

        if (file.Length < 4)
        {
            reader.Close();
            stream.Close();
            Debug.LogError("WadLoader: Load: WAD length < 4!");
            return false;
        }

        try
        {
            stream.Seek(0, SeekOrigin.Begin);

            bool isiwad = (Encoding.ASCII.GetString(reader.ReadBytes(4)) == "IWAD"); //other option is "PWAD"
            if (isiwad) { }

            int numlumps = reader.ReadInt32();
            int lumpsoffset = reader.ReadInt32();

            stream.Seek(lumpsoffset, SeekOrigin.Begin);

            lumps.Clear();

            for (int i = 0; i < numlumps; i++)
            {
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32();
                string name = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd('\0').ToUpper();

                lumps.Add(new Lump(offset, length, name));
            }

            //load the whole wad into memory
            long bytes = 0;
            foreach(Lump l in lumps)
            {
                l.data = new byte[l.length];
                stream.Seek(l.offset, SeekOrigin.Begin);
                stream.Read(l.data, 0, l.length);
                bytes += l.length;
            }

            Debug.Log("Loaded WAD \"" + file + "\" (" + bytes + " bytes in lumps)");
        }
        catch(Exception e)
        {
            Debug.LogError("WadLoader: Load: Reader exception!");
            Debug.LogError(e);

            reader.Close();
            stream.Close();
            return false;
        }

        reader.Close();
        stream.Close();
        return true;
	}

    private void SetPlayerDisabled() {
		Doom.player.SetInputEnabled(false);
    }
}
