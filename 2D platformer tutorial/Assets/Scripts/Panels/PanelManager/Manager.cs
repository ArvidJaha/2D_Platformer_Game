using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : Singleton<Manager>
{
     
    //This is holding all of our instances
    private List<PanelInstanceModel> _listInstances = new List<PanelInstanceModel>();

    //pool of panels
    private ObjectPool _objectPool;

    private void Start()
    {
        //cache the object pool
        _objectPool = ObjectPool.Instance;
    }
    public void ShowPanel(string panelId)
    {
        GameObject panelInstance = _objectPool.GetObjectFromPool(panelId);
        
        panelInstance.transform.localPosition = Vector3.zero;
        if (panelInstance != null)
        {
            //Add this new panel to the queue
            _listInstances.Add(new PanelInstanceModel
            {
                PanelId = panelId,
                PanelInstance = panelInstance
            });
        }
        else
        {
            Debug.LogWarning($"Trying to use panelId = {panelId}, but this is not found in panels");
        }
    }
    
    public void HideLastPanel()
    {
        if (AnyPanelShowing())
        {
            var lastPanel = _listInstances[_listInstances.Count - 1];

            _listInstances.Remove(lastPanel);

            _objectPool.poolObject(lastPanel.PanelInstance);
        }
    }

    public bool AnyPanelShowing()
    {
        return GetAmountPanelIsInQueue() > 0;
    }

    //Returns how many panels we have in queue
    public int GetAmountPanelIsInQueue()
    {
        return _listInstances.Count;
    }
}
