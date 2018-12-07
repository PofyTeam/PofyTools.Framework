﻿using PofyTools;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : Panel
{
    public Text title, message;
    public Button confirm, cancel;

    public void ShowDialog (string title, string message, UnityAction onConfirm)
    {
        this.confirm.onClick.RemoveAllListeners ();
        this.confirm.onClick.AddListener (onConfirm);
        this.confirm.onClick.AddListener (this.Close);

        this.cancel.onClick.RemoveAllListeners ();
        this.cancel.onClick.AddListener (this.Close);

        this.title.text = title;
        this.message.text = message;

        this.Open ();
    }
}
