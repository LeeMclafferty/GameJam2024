using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text timeText;
    [SerializeField] TMPro.TMP_Text counterText;
    // Start is called before the first frame update
    void Start()
    {
        if(Timer.Instance != null)
            Timer.Instance.OnTick.AddListener(UpdateTime);
    }

    void UpdateTime(string time)
    {
        timeText.text = time;
    }
}
