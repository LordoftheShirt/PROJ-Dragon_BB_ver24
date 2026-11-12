using UnityEngine;

public class Scrap : ResourceNode
{
    override public void Interact()//kan ta bort om inte ska göra något speciellt med scrap
    {
        base.Interact();
        // Implementation for interacting with scrap
        Debug.Log("Interacted with scrap!");
        //CollectResource(1); //görs i resourceNode
    }

    override public string GetDescription()//kan ta bort om inte ska göra något speciellt med scrap
    {
        return "Collect Scrap";
    }
}
