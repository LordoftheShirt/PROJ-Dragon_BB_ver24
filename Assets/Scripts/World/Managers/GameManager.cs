using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 90;
    } 

    public void ResetPlayer()
    {
        //ReferenceManager.Instance.player.Gravity = 0;
        GameObject cab = ReferenceManager.Instance.cabbin;
        ReferenceManager.Instance.player.GetComponent<CharacterController>().enabled = false;
        ReferenceManager.Instance.player.gameObject.transform.position = new Vector3(cab.transform.position.x, cab.transform.position.y + cab.transform.lossyScale.y, cab.transform.position.z);
        ReferenceManager.Instance.player.GetComponent<CharacterController>().enabled = true;
        ReferenceManager.Instance.dayNightManager.Newday();
        //ReferenceManager.Instance.player.Gravity = -15;
        Debug.Log("new pos = " + new Vector3(cab.transform.position.x, cab.transform.position.y + cab.transform.lossyScale.y, cab.transform.position.z));
    }

}