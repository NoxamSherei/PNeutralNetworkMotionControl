using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour {

    public Text OtherInformation;
    public Text GenerationInformation;
    public Text PopulationInformation;
    public Text PopulationRating;
    public Transform LayerLayout;
    public UILayer[] Layers;
    public GameObject PrefabLayer;
    void Start() {
        Invoke("FPSChecker", 0.01f);
    }

    private void FPSChecker()
    {
        //informacja o klatkach na sekunde
        OtherInformation.text = (1 / Time.deltaTime).ToString("f0");
        Invoke("FPSChecker", 0.1f);
    }

    //public void GenerateLayers(int[] layers)
    //{
    //    Layers = new UILayer[layers.Length];
    //    for (int i = 0; i < Layers.Length; i++)
    //    {
    //        Layers[i] = Instantiate(PrefabLayer, LayerLayout).GetComponent<UILayer>();
    //        Layers[i].Initialize(layers[i]);
    //    }
    //}

    //public void ShowUpLayers()
    //{
    //    LayerLayout.gameObject.SetActive(true);
    //}
    //public void ShowDownLayers()
    //{
    //    LayerLayout.gameObject.SetActive(false);
    //}
}
