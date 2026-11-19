using UnityEngine;

public class Cursor : Singleton<Cursor>
{
    // TODO: Make a SDF that morphs (lerps) to show the current state!
    public enum State
    {
        Normal,
        Interactable,

    }
}
