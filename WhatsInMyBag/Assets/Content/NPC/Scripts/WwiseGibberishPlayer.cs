using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AkGameObj))]
public class WwiseGibberishPlayer : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event gibberish;
    [SerializeField] private float MinIntervalTime = 5;
    [SerializeField] private float MaxIntervalTime = 10;

    private void Start()
    {
        StartCoroutine(PlayerGibberish(Random.Range(MinIntervalTime, MaxIntervalTime)));
    }

    private IEnumerator PlayerGibberish(float time)
    {
        yield return new WaitForSeconds(time);
        gibberish.Post(gameObject);
        StartCoroutine(PlayerGibberish(Random.Range(MinIntervalTime, MaxIntervalTime)));
    }
}
