using System.Collections;
using System.Collections.Generic;
using GameEvents;
using Player.Controller;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class ActionPopupManager : MonoBehaviour
{
    [SerializeField] private GameObject npcSurprisePopup;
    //[SerializeField] private Vector3 popupPositionOffset = Vector3.zero;
    [SerializeField] private float popupSize = 1f;

    [SerializeField] private List<GameObject> activePopups;
    [SerializeField] private List<GameObject> enemiesWithPopups;
    
    void OnEnable()
    {
        GameEventManager.AddListener<EnemySeesBodyEvent>(onEnemySeesBody);
    }
    
    void OnDisable()
    {
        GameEventManager.RemoveListener<EnemySeesBodyEvent>(onEnemySeesBody);
    }

    
    // Update is called once per frame
    void Update()
    {

        if (activePopups.Count != null)
        {
            for (int i = 0; i < activePopups.Count; i++)
            {
                if (activePopups[i] != null)
                {
                    Vector3 screenSpacePosition =
                        GuiUtility.GuiUtility.GetScreenPointAboveObject(enemiesWithPopups[i].GetComponent<Collider>());
                    IndicatorIcon icon = enemiesWithPopups[i].GetComponent<IndicatorIcon>();
                    activePopups[i].transform.position = screenSpacePosition + icon.PopupOffset;
                }
            }
        }
        
        if (Keyboard.current.mKey.IsPressed())
        {
            npcSurprisePopup.SetActive(true);
            npcSurprisePopup.GetComponent<PlayableDirector>().Play();
        }

        if (Keyboard.current.nKey.IsPressed())
        {
            GameObject newPopup = Instantiate(npcSurprisePopup, Random.Range(0, Screen.height) * Vector3.up + Random.Range(0, Screen.width) * Vector3.right, Quaternion.identity);
            newPopup.transform.SetParent(this.transform, true);
            newPopup.transform.localScale = popupSize * Vector3.one;
            newPopup.GetComponent<PlayableDirector>().Play();
        }
    }


    private void onEnemySeesBody(EnemySeesBodyEvent e)
    {
        //Debug.Log("EnemySeesBodyEvent");
        SpawnNpcSurprisePopup(e.enemy);
    }

    private void SpawnNpcSurprisePopup(GameObject enemy)
    {
        Vector3 ScreenSpaceSpawnPosition = GuiUtility.GuiUtility.GetScreenPointAboveObject(enemy.GetComponent<Collider>());
        //Debug.Log("ActionPopupManager: Enemy Position WS: " + enemy.transform.position);
        //Debug.Log("ActionPopupManager: Enemy Position SS: " + ScreenSpaceSpawnPosition);

        IndicatorIcon icon = enemy.GetComponent<IndicatorIcon>();
        GameObject newPopup = Instantiate(npcSurprisePopup, ScreenSpaceSpawnPosition + icon.PopupOffset, Quaternion.identity);
        
        activePopups.Add(newPopup);
        enemiesWithPopups.Add(enemy);
        
        newPopup.transform.localScale = popupSize * Vector3.one;
        newPopup.SetActive(true);
        newPopup.transform.SetParent(this.transform, true);

        //newPopup.GetComponent<PlayableDirector>().Play();
        
        StartCoroutine(DestroyAfterSeconds(newPopup, 2f));
    }
    
    private IEnumerator DestroyAfterSeconds(GameObject gameObject,float seconds)
    {
        yield return new WaitForSeconds(seconds);

        activePopups.RemoveAt(0);
        enemiesWithPopups.RemoveAt(0);
        
        Destroy(gameObject);
    }
}
