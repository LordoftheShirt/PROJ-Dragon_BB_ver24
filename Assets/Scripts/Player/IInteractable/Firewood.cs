using Mono.Cecil;
using UnityEngine;

public class Firewood : ResourceNode
{
    override public void Interact()//kan ta bort om inte ska göra något speciellt med firewood
    {
        base.Interact();
        // Implementation for interacting with firewood
        Debug.Log("Interacted with firewood!");
        //CollectResource(1); //görs i resourceNode
    }

    override public string GetDescription()//kan ta bort om inte ska göra något speciellt med firewood
    {
        return "Collect Wood";
    }
}
