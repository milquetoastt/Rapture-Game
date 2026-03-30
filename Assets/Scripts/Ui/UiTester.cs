using UnityEngine;

public class UiTester : MonoBehaviour
{
    public CooldownIndicatorUI testUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerController.instance.GainExperience(3);
        }
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            //PlayerController.instance.GainExperience(3);
            
            UIManager.instance.CloseLevelUp();
            
            //Time.timeScale = 0f;
            //Debug.Log(Time.timeScale);
        }
        
    }
}