using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text timeText;
    [SerializeField] TMPro.TMP_Text counterText;
    

    void Start()
    {
        if(Timer.Instance != null)
            Timer.Instance.OnTick.AddListener(UpdateTime);

        if(MouseObjectManager.Instance != null)
            MouseObjectManager.Instance.OnCollectMouse.AddListener(UpdateCounter);
            UpdateCounter(MouseObjectManager.Instance.FormatCount);

    }

    void UpdateTime(string time)
    {
        timeText.text = time;
    }

    void UpdateCounter(string count)
    {
        counterText.text = count;
    }
}
