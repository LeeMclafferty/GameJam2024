using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextRandomizer : MonoBehaviour
{
    TMPro.TMP_Text textMesh;

    void Start()
    {
        textMesh = GetComponent<TMPro.TMP_Text>();
    }

    void Randomize()
    {
        textMesh.text = Random.value > 0.9 ? "PAWS" : "PAUSE";
    }
}
