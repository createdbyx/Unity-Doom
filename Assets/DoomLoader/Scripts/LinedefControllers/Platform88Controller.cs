using UnityEngine;

public class Platform88Controller : MonoBehaviour 
{
    public void Init(Sector sector)
    {
        targetHeight = currentHeight = originalHeight = sector.floorHeight;

        foreach (Sidedef s in sector.Sidedefs)
        {
            if (s.Line.Back == null)
                continue;

            if (s.Line.Back.Sector == sector)
            {
                if (s.Line.BotFrontObject != null)
                    s.Line.BotFrontObject.transform.SetParent(transform);

                if (s.Line.Front.Sector.floorHeight < targetHeight)
                    targetHeight = s.Line.Front.Sector.floorHeight;
            }
        }
    }

    public float speed = 2;

    public float originalHeight;
    public float targetHeight;
    public float currentHeight;

    public enum State
    {
        AtTop,
        AtBottom,
        Lowering,
        Rising
    }

    public State CurrentState;

    public float waitTime;

    void Update()
    {
        switch (CurrentState)
        {
            default:
            case State.AtTop:
                break;

            case State.Lowering:
                currentHeight -= Time.deltaTime * speed;
                if (currentHeight < targetHeight)
                {
                    currentHeight = targetHeight;
                    CurrentState = State.AtBottom;
                }
                transform.position = new Vector3(transform.position.x, currentHeight - originalHeight, transform.position.z);
                break;

            case State.AtBottom:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                    CurrentState = State.Rising;
                break;

            case State.Rising:
                currentHeight += Time.deltaTime * speed;
                if (currentHeight > originalHeight)
                {
                    currentHeight = originalHeight;
                    CurrentState = State.AtTop;
                }
                transform.position = new Vector3(transform.position.x, currentHeight - originalHeight, transform.position.z);
                break;
        }
    }
}
