using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text waves;
    [SerializeField] Text cutdown;

    void Awake()
    {
        // waves = GetComponentInChildren<Text>();
        // cutdown = GetComponentInChildren<Text>();
    }

    void Update()
    {
        waves.text = "第 " + EnemyManager.Instance.WaveNumber + " 波";
        cutdown.text = EnemyManager.Instance.curTimeBetweenWaves + " 秒";
    }


}
