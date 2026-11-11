using UnityEngine;

public class CabbinActions : MonoBehaviour
{
    public void WarmUpFromCabin(float timeSpent, float resourcesSpentOnWarmth, GameObject player)
    {
        player.GetComponent<PlayerMeter>().IncreaseWarmth(timeSpent * resourcesSpentOnWarmth);
        //Mer logik med häsla 
    }

    public void RestInCabin(float resourcesSpentOnRest, GameObject player)
    {
        //Byt till dag och resetta player stats
    }
}
