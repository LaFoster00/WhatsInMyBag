using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDelay : MonoBehaviour
{

    //[SerializeField] private GameObject[] PaperStacks;
    
    [SerializeField] private GameObject Indicator;
    [SerializeField] private GameObject PaperStackGO;
    [SerializeField] private GameObject PaperStackDecal;
    [SerializeField] private GameObject AttentionSign;

    
    //[SerializeField] private float spawnDelayIndicator;
    //[SerializeField] private float spawnDelayPaperStacksAndDecals;
    [SerializeField] private float spawnDelay = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        //PaperStacks = new GameObject[NumberOfObjects];
        PaperStackGO.gameObject.SetActive(false);
        PaperStackDecal.gameObject.SetActive(false);
        Indicator.gameObject.SetActive(false);
        AttentionSign.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void OnEnable()
    {
        StartCoroutine(EnableAttentionSign());
        StartCoroutine(SpawnStuff());
    }

    private void OnDisable()
    {
        AttentionSign.gameObject.SetActive(false);
        Indicator.gameObject.SetActive(false);
    }

    IEnumerator EnableAttentionSign()
    {
        yield return new WaitForSeconds(0.5f);
        AttentionSign.gameObject.SetActive(true);
    }

    IEnumerator SpawnStuff()
    {
        yield return new WaitForSeconds(spawnDelay);

        PaperStackGO.gameObject.SetActive(true);
        PaperStackDecal.gameObject.SetActive(true);
        Indicator.gameObject.SetActive(true);
    }


}
