using UnityEngine;

public class BuildPlacerComponent : ItemComponent
{
    public BuildingPlacer BuildingPlacer => BuildingPlacer.Instance;

    [SerializeField] private Building building;

    public override void Select()
    {
        switch (building)
        {
            case Building.Flag:
                BuildingPlacer.SetGhostToFlag();
                break;

            case Building.Campfire:
                BuildingPlacer.SetGhostToFireKit();
                break;

            case Building.Tent:
                BuildingPlacer.SetGhostToTent();
                break;
        }
    }

    public override void Unselect()
    {
        BuildingPlacer.RemoveGhost();
    }

    public enum Building
    {
        Flag,
        Campfire,
        Tent,
    }
}
