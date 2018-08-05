using UnityEngine;

public class BlinkRandom : MonoBehaviour 
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
    float random;

    void Update()
    {
        time += Time.deltaTime;

        if (time >= random)
        {
            time = 0;
            flip = !flip;

            l.intensity = flip ? onIntensity : offIntensity;
            random = Random.Range(.2f, .8f);
        }
    }
}
