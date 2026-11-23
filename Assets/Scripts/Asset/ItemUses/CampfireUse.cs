public class CampfireUse : BuildingUse
{
    public override void Use()
    {
        
    }

    public override void Select()
    {
        BuildingPlacer.SetGhostToFireKit();
    }

    public override void Unselect()
    {
        BuildingPlacer.RemoveGhost();
    }
}
