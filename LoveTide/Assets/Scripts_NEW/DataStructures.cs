using System;
using System.Collections.Generic;

[Serializable]
public class DialogLine
{
    public string EventIndex;
    public string DialogIndex;
    public string ActorName;
    public string Dialog;
    public string ActorFace;
    public string ActorAnimator;
}

[Serializable]
public class DialogCollection
{
    public List<DialogLine> dialogs = new List<DialogLine>();
    
    public DialogCollection()
    {
        dialogs = new List<DialogLine>();
    }
}