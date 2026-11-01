using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResourceWheelUi : MonoBehaviour
{
    [SerializeField]
    readonly Color m_activeIconColour = new Color(0xFF,0xFF,0xFF);
    [SerializeField]
    readonly Color m_inactiveIconColor = new Color(0x64,0x64,0x64);
    [SerializeField] 
    private ResourceWheelSO _resources;

    private struct ResourceDisplay
    {
        public Image icon;
        public TextMeshProUGUI textBox;
    }

    private ResourceDisplay[] m_resourceDisplays = new ResourceDisplay[4];

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
        JankyImageInitialisation();
        Deactivate();
    }

    void Update()
    {
        if (!_isActive) { return; }

        int button = 0;

        button = (Gamepad.current?.buttonWest.ReadValue()  ?? (Input.GetKey(KeyCode.J) ? 1.0f : 0.0f)) >= 0.5f ? 1 : button;
        button = (Gamepad.current?.buttonEast.ReadValue()  ?? (Input.GetKey(KeyCode.L) ? 1.0f : 0.0f)) >= 0.5f ? 2 : button;
        button = (Gamepad.current?.buttonNorth.ReadValue() ?? (Input.GetKey(KeyCode.I) ? 1.0f : 0.0f)) >= 0.5f ? 3 : button;
        button = (Gamepad.current?.buttonSouth.ReadValue() ?? (Input.GetKey(KeyCode.K) ? 1.0f : 0.0f)) >= 0.5f ? 4 : button;

        if (button > 0)
        {
            _inventory.UseIngredient(_resources.resources[button - 1]);
        }
    }

    // Moved gross code here to only execute once. - M@
    void JankyImageInitialisation()
    {
        _uiParent = transform.GetChild(0);

        for (int i = 0; i < _resources.resources.Length; ++i)
        {
            var root = _uiParent.GetChild(i + 1);

            Sprite icon = _resources.resources[i].image;
            m_resourceDisplays[i].icon = root.GetChild(0).GetComponent<Image>();
            m_resourceDisplays[i].icon.sprite = icon;
            m_resourceDisplays[i].textBox = root.GetChild(3).GetComponent<TextMeshProUGUI>();
            m_resourceDisplays[i].textBox.text = _inventory.GetResourceCount(_resources.resources[i]).ToString();

            root.GetChild(1).GetComponent<Image>().color = ElementUtils.ElementToColour(_resources.resources[i].Elements[0]);
            root.GetChild(2).GetComponent<Image>().color = ElementUtils.ElementToColour(_resources.resources[i].Elements[1]);
        }

        _inventory.IngredientCountChanged += UpdateIndicators;
    }

    // Called directly by inventory - M@
    void UpdateIndicators()
    {
        for (int i = 0; i < m_resourceDisplays.Length; ++i)
        {
            int resourceCount = _inventory.GetResourceCount(_resources.resources[i]);
            m_resourceDisplays[i].textBox.text = resourceCount.ToString();
            m_resourceDisplays[i].icon.color = resourceCount > 0 ? m_activeIconColour : m_inactiveIconColor;
        }
    }
}
