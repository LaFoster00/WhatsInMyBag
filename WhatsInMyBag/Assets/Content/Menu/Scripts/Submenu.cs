using GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class Submenu : MonoBehaviour
{
    [SerializeField] private GameObject menuContent;

    [SerializeField] private Selectable initSelectable;
    
    private Submenu _parentMenu;
    private bool _active;

    private void Awake()
    {
        if (transform.parent)
        {
            _parentMenu = transform.parent.GetComponent<Submenu>();
        }
    }

    private void OnEnable()
    {
        GameEventManager.AddListener<InputDeviceChangedEvent>(OnInputDeviceChanged);
    }

    private void OnDisable()
    {
        GameEventManager.RemoveListener<InputDeviceChangedEvent>(OnInputDeviceChanged);
    }

    private void OnInputDeviceChanged(InputDeviceChangedEvent @event)
    {
        if (!_active) return;
        if (@event.Scheme != "KeyboardMouse")
        {
            initSelectable.Select();
        }
    }

    public void Open()
    {
        menuContent.SetActive(true);
        initSelectable.Select();
        _active = true;
    }
    
    public void Close()
    {
        menuContent.SetActive(false);
        _active = false;
    }

    public Submenu GetParentMenu()
    {
        return _parentMenu;
    }
}
