using System.Collections;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    #region PROPERTIES
    
    [SerializeField] private Submenu initMenu;
    
    private Submenu _currentlyActiveMenu;
    private Submenu _previouslyActiveMenu;
    private Submenu[] _submenus;

    private bool _uiInputActivated;
    
    #endregion
    
    private void Awake()
    {
        _submenus = FindObjectsOfType<Submenu>().Where(submenu => submenu.gameObject.activeSelf && submenu.enabled).ToArray();
        foreach (var submenu in _submenus)
        {
            submenu.Close();
        }

        if (initMenu)
        {
            initMenu.Open();
            _currentlyActiveMenu = initMenu;
            ActivateUiInput();
        }
    }

    public void CloseMenu()
    {
        if (_currentlyActiveMenu)
        {
            _currentlyActiveMenu.Close();
        }

        _previouslyActiveMenu = _currentlyActiveMenu;
        _currentlyActiveMenu = null;
        StartCoroutine(DeactivateUiInputWithDelay());
    }
    
    public void SwitchToMenu(Submenu menu)
    {
        if (menu == _currentlyActiveMenu) return;

        if (menu)
        {
            if (_currentlyActiveMenu) _currentlyActiveMenu.Close(); 
            menu.Open();
            StartCoroutine(ActivateUiInputWithDelay());
        }
        else
        {
            CloseMenu();
        }

        _previouslyActiveMenu = _currentlyActiveMenu;
        _currentlyActiveMenu = menu;
    }

    public void SwitchToParentMenu()
    {
        if (_currentlyActiveMenu && _currentlyActiveMenu.GetParentMenu())
        {
            _currentlyActiveMenu.Close();
            _previouslyActiveMenu = _currentlyActiveMenu;
            _currentlyActiveMenu  = _currentlyActiveMenu.GetParentMenu();
            _currentlyActiveMenu.Open();
        }
        else
        {
            CloseMenu();
        }
    }
    
    public void SwitchToPreviousMenu()
    {
        if (_previouslyActiveMenu)
        {
            _currentlyActiveMenu.Close();
            _previouslyActiveMenu.Open();
            _currentlyActiveMenu = _previouslyActiveMenu;
            _previouslyActiveMenu = null;
        }
        else if (_currentlyActiveMenu && _currentlyActiveMenu.GetParentMenu())
        {
                _currentlyActiveMenu.Close();
                _previouslyActiveMenu = _currentlyActiveMenu;
                _currentlyActiveMenu  = _currentlyActiveMenu.GetParentMenu();
                _currentlyActiveMenu.Open();
        }
        else
        {
            CloseMenu();
        }
    }

    public void Quit()
    {
	    Application.Quit();
    }

    private IEnumerator ActivateUiInputWithDelay()
    {
        yield return null;
        ActivateUiInput();
    }

    private IEnumerator DeactivateUiInputWithDelay()
    {
        yield return null;
        DeactivateUiInput();
    }
    
    public void ActivateUiInput()
    {
        if (!_uiInputActivated && GameManager.Instance && GameManager.Instance.playerInput && GameManager.Instance.playerInput.inputIsActive)
        {
            //print("Activated UI Input");
            GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");
            _uiInputActivated = true;
        }
    }

    public void DeactivateUiInput()
    {
        if (_uiInputActivated && GameManager.Instance && GameManager.Instance.playerInput && GameManager.Instance.playerInput.inputIsActive)
        {
            //print("Activated Player Input");
            GameManager.Instance.playerInput.SwitchCurrentActionMap("PlayerInput");
            _uiInputActivated = false;
        }
    }
}
