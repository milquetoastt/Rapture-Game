using UnityEngine;

public class UiTester : MonoBehaviour
{
    public CooldownIndicatorUI testUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            testUI.StartCooldown(3f);
        }
    }
}