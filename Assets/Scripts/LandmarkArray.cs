using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Landmark
{
    [SerializeField] private GameObject model;
    [SerializeField] private Sprite mapIcon;

    public GameObject GetModel()
    {
        return model;
    }

    public Sprite GetMapIcon()
    {
        return mapIcon;
    }
}


[CreateAssetMenu(fileName = "NewLandmarkArray", menuName = "Landmark Array")]
public class LandmarkArray : ScriptableObject
{
    [SerializeField] private Landmark[] landmarks;

    public Landmark[] GetLandmarks()
    {
        return landmarks;
    }
}
