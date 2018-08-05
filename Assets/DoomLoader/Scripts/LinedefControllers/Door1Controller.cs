using UnityEngine;

public class Door1LinedefController : MonoBehaviour, PokeableLinedef
{
    public Door1SectorController sectorController;

    public void Poke(GameObject caller)
    {
        if (sectorController.CurrentState != Door1SectorController.State.Open &&
            sectorController.CurrentState != Door1SectorController.State.Opening)
        {
            sectorController.CurrentState = Door1SectorController.State.Opening;
            sectorController.waitTime = 4;
        }
    }

    public void Init(Linedef linedef)
    {
        if (linedef.Back == null)
            return;

        sectorController = linedef.Back.Sector.ceilingObject.GetComponent<Door1SectorController>();
        if (sectorController == null)
        {
            sectorController = linedef.Back.Sector.ceilingObject.AddComponent<Door1SectorController>();
            sectorController.originalHeight = linedef.Back.Sector.ceilingHeight;
            sectorController.currentHeight = sectorController.originalHeight;
            sectorController.targetHeight = linedef.Front.Sector.ceilingHeight - MapLoader._4units;
        }
        else
        {
            if (sectorController.targetHeight > linedef.Front.Sector.ceilingHeight - MapLoader._4units)
                sectorController.targetHeight = linedef.Front.Sector.ceilingHeight - MapLoader._4units;
        }

        transform.SetParent(linedef.Back.Sector.ceilingObject.transform);
    }
}

public class Door1SectorController : MonoBehaviour
{
    public float speed = 2;

    public float originalHeight;
    public float targetHeight;
    public float currentHeight;

    public enum State
    {
        Closed,
        Open,
        Opening,
        Closing
    }

    public State CurrentState = State.Closed;

    public float waitTime;

    void Update()
    {
        switch (CurrentState)
        {
            default:
            case State.Closed:
                break;

            case State.Opening:
                currentHeight += Time.deltaTime * speed;
                if (currentHeight > targetHeight)
                {
                    currentHeight = targetHeight;
                    CurrentState = State.Open;
                }
                transform.position = new Vector3(transform.position.x, currentHeight - originalHeight, transform.position.z);
                break;

            case State.Open:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                    CurrentState = State.Closing;
                break;

            case State.Closing:
                currentHeight -= Time.deltaTime * speed;
                if (currentHeight < originalHeight)
                {
                    currentHeight = originalHeight;
                    CurrentState = State.Closed;
                }
                transform.position = new Vector3(transform.position.x, currentHeight - originalHeight, transform.position.z);
                break;
        }
    }
}