using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPaperVFX : MonoBehaviour
{
    [SerializeField] private GameObject Indicator;
    [SerializeField] private GameObject PaperStackGO;
    [SerializeField] private GameObject PaperStackDecal;
    [SerializeField] private GameObject AttentionSign;
    
    // Start is called before the first frame update
    void Start()
    {
        PaperStackGO.gameObject.SetActive(false);
        PaperStackDecal.gameObject.SetActive(false);
        Indicator.gameObject.SetActive(false);
        AttentionSign.gameObject.SetActive(false);
    }
}
