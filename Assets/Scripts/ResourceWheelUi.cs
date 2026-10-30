using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResourceWheelUi : MonoBehaviour
{
    [SerializeField] private ResourceWheelSO _resources;

    private Inventory _inventory;

    private Transform _uiParent;
    private bool _isActive = false;

    public void Initialize(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void Activate()
    {
        _uiParent.gameObject.SetActive(true);
        _isActive = true;
    }

    public void Deactivate()
    {
        _uiParent.gameObject.SetActive(false);
        _isActive = false;
    }

    void Start()
    {
        _uiParent = transform.GetChild(0);

        for (int i = 0; i < _resources.resources.Length; ++i)
        {
            var a = _uiParent.GetChild(i);
            var b = a.GetChild(0).GetComponent<Image>();
            var c = _resources.resources[i].image;
            b.sprite = c;


        }

        Deactivate();
    }

    void Update()
    {
        if (!_isActive) { return; }

        int button = 0;

        button = (Gamepad.current?.buttonWest.ReadValue() ?? 0.0f)  >= 0.5f ? 1 : button;
        button = (Gamepad.current?.buttonEast.ReadValue() ?? 0.0f)  >= 0.5f ? 2 : button;
        button = (Gamepad.current?.buttonNorth.ReadValue() ?? 0.0f) >= 0.5f ? 3 : button;
        button = (Gamepad.current?.buttonSouth.ReadValue() ?? 0.0f) >= 0.5f ? 4 : button;

        if (button > 0)
        {
            _inventory.UseIngredient(_resources.resources[button - 1]);
        }
    }
}
