using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void FixedUpdate()
    {
        transform.Rotate(0.0f, _speed, 0.0f);
    }
}
