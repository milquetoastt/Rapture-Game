using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    public int which;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenWardrobe()
    {
        UIManager.instance.WardrobeSequence(which);
    }
}
