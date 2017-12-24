using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour {

    public Text OtherInformation;
    public Text GenerationInformation;
    public Text PopulationInformation;
    public Text PopulationRating;
	void Start () {
        Invoke("FPSChecker", 0.01f);
	}

    private void FPSChecker()
    {
        //informacja o klatkach na sekunde
        OtherInformation.text = (1 / Time.deltaTime).ToString("f0");
        Invoke("FPSChecker", 0.1f);
    }

}
