using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour {

    Text yearText;

    public static readonly float TIME_PER_YEAR = 60;

    public void Awake()
    {
        yearText = GetComponentInChildren<Text>();
        yearText.text = "Year: 1";
    }

    public void Update()
    {

        yearText.text = "Year: " + TimeManager.year.ToString();
    }
}
