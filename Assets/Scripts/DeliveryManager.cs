using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Recipe[]   _recipes;
    [SerializeField] private Customer[] _gaffs;
     
    [Header("UI")]
    [SerializeField] private Image _potionImage;
    [SerializeField] private Image[] _potionElements;

    private Recipe _currentRecipe;
    public  Recipe CurrentRecipe => _currentRecipe;

    public void RequestPotion()
    {
        if (_currentRecipe)
        {
            Debug.Log("Still need to deliver potion, can't request new one.");
            return;
        }

        _currentRecipe = _recipes[Random.Range(0, _recipes.Length)];

        _gaffs[Random.Range(0, _gaffs.Length)].Activate(this);

        //_potionImage.sprite = potion.sprite;
        for (int i = 0; i < _potionElements.Length; ++i)
        {
            _potionElements[i].color = ElementUtils.ElementToColour(_currentRecipe.Elements[i]);
        }
    }

    public void DeliverPotion()
    {
        _currentRecipe = null;

        RequestPotion();
    }

    private void Start()
    {
        RequestPotion();
    }
}
