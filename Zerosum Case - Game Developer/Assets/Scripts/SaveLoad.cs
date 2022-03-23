using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;

    private void Awake()
    {
        settings.level = PlayerPrefs.GetInt("level");
        settings.coin = PlayerPrefs.GetInt("coin");
        settings.coin = PlayerPrefs.GetInt("newCoin");
        settings.sensitivity = PlayerPrefs.GetFloat("sensitivity");
    }
}
