using UnityEngine;
using System.Collections;

public class SpectatorCamera : MonoBehaviour
{
    public int Speed = 20;

    void Start()
    {

    }

    void Update()
    {
        int speed = 0;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed = Speed * 2;
        else
            speed = Speed;

        if (Input.GetKey(KeyCode.A))
            transform.Rotate(0, -5 * speed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.Semicolon))
            transform.position = transform.position + Camera.main.transform.right * -1 * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.O))
            transform.position = transform.position + Camera.main.transform.forward * -1 * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, 5 * speed * Time.deltaTime, 0);
        
        if (Input.GetKey(KeyCode.J))
            transform.position = transform.position + Camera.main.transform.right * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Comma))
            transform.position = transform.position + Camera.main.transform.forward * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Period))
            transform.position = transform.position + Camera.main.transform.up * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Quote))
            transform.position = transform.position + Camera.main.transform.up * -1 * speed * Time.deltaTime;

    }
}