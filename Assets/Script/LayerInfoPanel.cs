using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerInfoPanel : MonoBehaviour {
    public Text Information;
    public InputField InField;
    public UIControler Controler;

    public void ChangeNumberInLayer()
    {
        Controler.SaveNumberOfNeurons();
    }

}
