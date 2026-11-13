using FMODUnity;
using UnityEngine;

[LoadOnInit]
public class ButtonSettings : SingletonPersistent<ButtonSettings>
{
    public EventReference ClickSFX => clickSFX;
    [SerializeField] private EventReference clickSFX;
}
