using UnityEngine;

public class ScrollLeftAnimation : MonoBehaviour
{
    MeshRenderer mr;

    MaterialPropertyBlock materialParameters;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        materialParameters = new MaterialPropertyBlock();
    }

    float offset = 0;

    void Update()
    {
        offset += Time.deltaTime * .25f;
        offset %= 1;

        mr.GetPropertyBlock(materialParameters);
        materialParameters.SetVector("_MainTex_ST", new Vector4(1, 1, offset, 0));
        mr.SetPropertyBlock(materialParameters);
    }
}
