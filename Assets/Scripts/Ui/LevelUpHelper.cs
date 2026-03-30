using UnityEngine;

public class LevelUpHelper : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowLevelUp(int level)
    {
        panel.SetActive(true);


    }

    public void ChooseUpgrade_Player()
    {
        Debug.Log("Picked Health Upgrade"); 
        PlayerController.instance.maxHealth += 30;
        if (PlayerController.instance.health + 15 > PlayerController.instance.maxHealth)
        {
            PlayerController.instance.health = PlayerController.instance.maxHealth;
        }
        else
            PlayerController.instance.health += 15;

        CloseLevelUp();
    }

    public void ChooseUpgrade_SpellBuff1()
    {
        Debug.Log("Picked buff1");
        CloseLevelUp();
    }
    
    public void ChooseUpgrade_SpellBuff2()
    {
        Debug.Log("Picked buff2");
        CloseLevelUp();
    }
    void CloseLevelUp()
    {
    }
}
