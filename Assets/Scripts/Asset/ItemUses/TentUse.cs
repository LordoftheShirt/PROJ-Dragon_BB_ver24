public class TentUse : BuildingUse
{
    public override void Use()
    {
        
    }

    public override void Select()
    {
        BuildingPlacer.SetGhostToTent();
    }

    public override void Unselect()
    {
        BuildingPlacer.RemoveGhost();
    }
}
