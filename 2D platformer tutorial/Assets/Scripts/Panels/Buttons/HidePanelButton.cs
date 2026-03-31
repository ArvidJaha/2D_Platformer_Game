using UnityEngine;

public class HidePanelButton : MonoBehaviour
{
    private Manager _manager;

    void Start()
    {
        _manager = Manager.Instance;
    }
    public void DoHidePanel()
    {
        _manager.HideLastPanel();
    }
} 
