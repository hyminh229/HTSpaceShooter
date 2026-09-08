using UnityEngine;

public class PlayerColorController : ChromaPolarityBase
{
    private void Update()
    {
        HandleColorSwitch();
    }

    private void HandleColorSwitch()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
        {
            SwitchColor();
            Debug.Log("Player switched color to: " + CurrentColor);
        }
    }
}