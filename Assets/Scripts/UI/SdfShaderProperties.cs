using UnityEngine;

public static class SdfShaderProperties
{
    public static readonly int COLOR = Shader.PropertyToID("_Color");
    public static readonly int OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
    public static readonly int OUTLINE_THICKNESS = Shader.PropertyToID("_OutlineThickness");
}
