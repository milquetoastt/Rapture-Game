using UnityEngine;

public class UiTester : MonoBehaviour
{
    public CooldownIndicatorUI testUI;
    public CrystalBoss crystalguy;
    public PanelSlideUI panelSlideUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            //PlayerController.instance.GainExperience(3);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //PlayerController.instance.GainExperience(3);

            //UIManager.instance.CloseLevelUp();

            //Time.timeScale = 0f;
            //Debug.Log(Time.timeScale);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            //UIManager.instance.WardrobeSequence(0);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GloveSpells.instance.UnlockSpell(1);
            GloveSpells.instance.UnlockSpell(2);
            GloveSpells.instance.UnlockSpell(3);
            
            MantleSpells.instance.UnlockSpell(1);
            MantleSpells.instance.UnlockSpell(2);
            MantleSpells.instance.UnlockSpell(3);
            
            BootSpells.instance.UnlockSpell(1);
            BootSpells.instance.UnlockSpell(2);
            BootSpells.instance.UnlockSpell(3);
            
            HatSpells.instance.UnlockSpell(1);
            HatSpells.instance.UnlockSpell(2);
            HatSpells.instance.UnlockSpell(3);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameDataManager.instance.ResetSave();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            panelSlideUI.SlideIn();
            
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            panelSlideUI.SlideOut();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            //PlayerController.instance.Level();
            
        }
    }
}