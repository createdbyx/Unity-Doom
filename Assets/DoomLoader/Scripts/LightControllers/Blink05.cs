using UnityEngine;

public class Blink05 : MonoBehaviour 
{
    Light l;
    public float offIntensity = 0f;
    public float onIntensity = 1f;

    void Awake()
    {
        l = GetComponent<Light>();
    }

    float time;
    bool flip;

    void Update()
    {
        time += Time.deltaTime;

        if (time >= .5f)
        {
            time = 0;
            flip = !flip;

            l.intensity = flip ? onIntensity : offIntensity;
        }
    }
}
