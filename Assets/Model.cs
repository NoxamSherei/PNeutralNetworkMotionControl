using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {
    public float fitnes = 0;
    private NeutralNetwork neuralNetwork;
    public SkinnedMeshRenderer MySkin;

    public void Creation(int[] layerShema)
    {
        neuralNetwork = new NeutralNetwork(layerShema);
        TestFitnes();
    }

    public void TestFitnes()
    {
        Color color = MySkin.material.color;
        color.r = 1 - (fitnes) / 155;
        color.g = (fitnes) / 155;
        color.b = (fitnes) / 155;
        MySkin.material.color= color;
        fitnes++;
        Invoke("TestFitnes", 0.1f);
    }
}
