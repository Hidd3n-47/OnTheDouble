using UnityEngine;

public class TestingMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotSpeed = 2.5f;

    public Transform camera;

    private Vector3 offset;

    private void Start()
    {
        offset = camera.position - transform.position;
    }

    void Update()
    {
        Vector2 movement = Vector2.zero;

        movement.x += (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.A) ? 1.0f : 0.0f);
        movement.y += (Input.GetKey(KeyCode.W) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.S) ? 1.0f : 0.0f);

        transform.Rotate(0.0f, movement.x * Time.deltaTime * rotSpeed, 0.0f, Space.Self);

        transform.position +=Time.deltaTime * speed * transform.forward * movement.y;
        camera.position = transform.position + offset;
        //camera.position += new Vector3(0.0f, 0.0f, movement.y) * Time.deltaTime * speed;
    }
}
