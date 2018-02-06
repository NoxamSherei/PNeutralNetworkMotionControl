using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayer : MonoBehaviour {

    public Image[] Neurons;
    public GameObject PrefabNeuron;
	public void Initialize(int howMany)
    {
        Neurons = new Image[howMany];
        for (int i = 0; i < howMany; i++)
        {
            Neurons[i] =Instantiate(PrefabNeuron, this.transform).GetComponent<Image>();
        }
    }
}
