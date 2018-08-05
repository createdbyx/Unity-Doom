using UnityEngine;

public class Floor36Controller : MonoBehaviour 
{
    public void Init(Sector sector)
    {
        targetHeight = currentHeight = originalHeight = sector.floorHeight;

        foreach (Sidedef s in sector.Sidedefs)
        {
            if (s.Line.Back == null)
                continue;

            if (s.Line.Front.Sector == sector)
                if (s.Line.BotBackObject != null)
                    s.Line.BotBackObject.transform.SetParent(transform);

            if (s.Line.Back.Sector == sector)
            {
                if (s.Line.BotFrontObject != null)
                    s.Line.BotFrontObject.transform.SetParent(transform);

                if (targetHeight == sector.floorHeight)
                    targetHeight = s.Line.Front.Sector.floorHeight + MapLoader._8units;

                if (s.Line.Front.Sector.floorHeight + MapLoader._8units > targetHeight)
                    targetHeight = s.Line.Front.Sector.floorHeight + MapLoader._8units;
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
    }

    public State CurrentState = State.AtTop;

    void Update()
    {
        switch (CurrentState)
        {
            default:
            case State.AtTop:
            case State.AtBottom:
                break;

            case State.Lowering:
                currentHeight -= Time.deltaTime * speed;
                if (currentHeight < targetHeight)
                {
                    currentHeight = targetHeight;
                    CurrentState = State.AtBottom;
                    enabled = false;
                }
                transform.position = new Vector3(transform.position.x, currentHeight - originalHeight, transform.position.z);
                break;
        }
    }
}
