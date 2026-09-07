using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupQueueData
{
    public UIPopup popup;
    public string popupName;    
    public UIPopup Show()
    {
        if (popup == null) return null;
        popup.Show();
        return popup;
    }
}
