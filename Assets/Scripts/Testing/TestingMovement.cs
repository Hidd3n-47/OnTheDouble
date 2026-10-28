using UnityEngine;

public class TestingMovement : MonoBehaviour
{
    public float speed = 5.0f;

    void Update()
    {
        Vector2 movement = Vector2.zero;

        movement.x += (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.A) ? 1.0f : 0.0f);
        movement.y += (Input.GetKey(KeyCode.W) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.S) ? 1.0f : 0.0f);

        transform.position += new Vector3(movement.x, 0.0f, movement.y) * Time.deltaTime * speed;
    }
}
