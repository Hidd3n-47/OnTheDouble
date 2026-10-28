using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Transform hud;

    [SerializeField] private Button[] buttons;
    [SerializeField] private float lStickCooldown = 0.5f;
    [SerializeField] private float lStickDeadZone = 0.2f;

    private float lStickCooldownTimer = 0.0f;
    private int selectedItem = 0;

    public void EnableMenu()
    {
        transform.gameObject.SetActive(true);
        hud.gameObject.SetActive(false);

        Time.timeScale = 0.0f;
    }

    public void DisableMenu()
    {
        Time.timeScale = 1.0f;
        hud.gameObject.SetActive(true);
        transform.gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    private void Update()
    {
        if(Gamepad.current?.circleButton.wasPressedThisFrame == true)
        {
            DisableMenu();
            return;
        }

        if (Gamepad.current?.xButton.wasPressedThisFrame == true)
        {
            DisableMenu();
            buttons[selectedItem].onClick?.Invoke();
            return;
        }

        float verticalMovement  = Gamepad.current?.leftStick.y.value ?? 0.0f;
        bool noVerticalMovement = Mathf.Approximately(verticalMovement, 0.0f);

        if (!noVerticalMovement && Mathf.Approximately(lStickCooldownTimer, 0.0f) && Mathf.Abs(verticalMovement) > lStickDeadZone)
        {
            selectedItem = (selectedItem - 1 * Math.Sign(verticalMovement)) % buttons.Length;
            lStickCooldownTimer = lStickCooldown;
        }

        if (Gamepad.current?.dpad.down.wasPressedThisFrame == true)
        {
            selectedItem = (selectedItem + 1) % buttons.Length;
        }
        else if (Gamepad.current?.dpad.up.wasPressedThisFrame == true)
        {
            selectedItem = (selectedItem - 1) % buttons.Length;
        }

        if (selectedItem < 0)
        {
            selectedItem += buttons.Length;
        }

        lStickCooldownTimer = Mathf.Max(lStickCooldownTimer - Time.unscaledDeltaTime, 0.0f);

        if (noVerticalMovement)
        {
            lStickCooldownTimer = 0.0f;
        }

        buttons[selectedItem].Select();
    }
}
