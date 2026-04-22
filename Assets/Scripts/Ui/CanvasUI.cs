using System;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public static CanvasUI instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
