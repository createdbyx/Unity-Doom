using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ThingController : MonoBehaviour 
{
    public string thingName;
    public int thingID;
    public string spriteName;
    public Thing thing;
    public bool alwaysBright;
    public bool dynamic;

    [HideInInspector]
    public Material material;

    MaterialPropertyBlock materialProperties;

    private MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();

        materialProperties = new MaterialPropertyBlock();
    }

    public void Init()
    {
        transform.rotation = Quaternion.Euler(0, thing.facing, 0);

        float x = thing.posX;
        float z = thing.posY;

        Vector3 origin = new Vector3(x, (float)MapLoader.maxZ / MapLoader.sizeDividor, z);
        //Vector3 target = new Vector3(x, (float)MapLoader.minZ / MapLoader.sizeDividor, z);

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit))
        {
            transform.position = hit.point;

            SectorController sc = hit.collider.gameObject.GetComponent<SectorController>();
            if (sc != null)
            {
                if (mr != null)
                {
                    mr.GetPropertyBlock(materialProperties);
                    materialProperties.SetFloat("_Brightness", alwaysBright ? 1f : sc.sector.brightness);
                    mr.SetPropertyBlock(materialProperties);
                }

                if (dynamic)
                    sc.DynamicThings.AddFirst(this);
                else
                    sc.StaticThings.Add(this);
            }
        }
        else
            Destroy(gameObject);

        if (!string.IsNullOrEmpty(spriteName))
        {
            Texture tex = TextureLoader.Instance.GetSpriteTexture(spriteName);
            //materialProperties.SetFloat("_ScaleX", (float)tex.width / MapLoader.sizeDividor);
            //materialProperties.SetFloat("_ScaleY", (float)tex.height / MapLoader.sizeDividor);
            Mesh mesh = Mesher.Instance.CreateBillboardMesh((float)tex.width / MapLoader.sizeDividor, (float)tex.height / MapLoader.sizeDividor, .5f, 0);
            GetComponent<MeshFilter>().mesh = mesh;

            SetTexture(tex);
            SetSpriteDirection(1);
        }
    }

    public void SetTexture(Texture input) {
		materialProperties.SetTexture("_MainTex", input);
        mr.SetPropertyBlock(materialProperties);
    }

    public void SetHeight (float input) {
		materialProperties.SetFloat("_ScaleY", input);
	}

	public void SetWidth(float input) {
		materialProperties.SetFloat("_ScaleX", input);
	}

    public void SetSpriteDirection (int input) {
		if (input > 0) {
			transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y,1);
			materialProperties.SetFloat("_ScaleX", 1);
      		mr.SetPropertyBlock(materialProperties);
		} else {
			transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y,-1);
			materialProperties.SetFloat("_ScaleX", -1);
      		mr.SetPropertyBlock(materialProperties);
		}
	}

    public void SetBrightness(float value)
    {
        if (alwaysBright)
            return;

        if (mr == null)
            return;

        mr.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_Brightness", value);
        mr.SetPropertyBlock(materialProperties);
    }
}
