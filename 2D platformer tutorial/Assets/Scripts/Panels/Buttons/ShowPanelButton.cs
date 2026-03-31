using UnityEngine;

public class ShowPanelButtons: MonoBehaviour
{
    public string PanelId;

    private Manager _manager;

    private void Start()
    {
        _manager = Manager.Instance;
    }
    public void DoShowPanel()
    {
        _manager.ShowPanel(PanelId);
    }
}
