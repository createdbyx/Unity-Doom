using UnityEngine;

public class TextureAnimation : MonoBehaviour 
{
    public enum TextureType
    {
        Wall,
        Flat,
        Sprite
    }

    public TextureType textureType;

    public float frameTime = 1f;
    public string[] frames;
    private Texture[] _frames;
    public bool oscillates;
    public int direction = 1;

    MeshRenderer mr;
    MaterialPropertyBlock materialParameters;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        materialParameters = new MaterialPropertyBlock();
    }

    void Start()
    {
        if (frames.Length == 0)
            enabled = false;

        _frames = new Texture[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            if (textureType == TextureType.Wall)
                _frames[i] = TextureLoader.Instance.GetWallTexture(frames[i]);
            else if (textureType == TextureType.Flat)
                _frames[i] = TextureLoader.Instance.GetFlatTexture(frames[i]);
            else if (textureType == TextureType.Sprite)
                _frames[i] = TextureLoader.Instance.GetSpriteTexture(frames[i]);
        }
    }

    float time;
    int index = 0;

    void Update()
    {
        time += Time.deltaTime;
        if (time > frameTime)
        {
            index += direction;
            time = 0;

            if (index >= _frames.Length)
                if (oscillates)
                {
                    direction = -1;
                    index--;
                }
                else
                    index = 0;

            if (index < 0)
                if (oscillates)
                {
                    direction = 1;
                    index++;
                }
                else
                    index = _frames.Length - 1;

            mr.GetPropertyBlock(materialParameters);
            materialParameters.SetTexture("_MainTex", _frames[index]);
            mr.SetPropertyBlock(materialParameters);
        }
    }
}
