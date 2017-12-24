using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationManager : MonoBehaviour {
    
    public UIControler UserInterface;
    public GameObject PreFabs;
    public Slider MutSlider;
    public Text MutText;

    public int Size;
    public float Mutation;
    public int LifeTime;
    public Model[] Population;
    public int[] NeuralSchemat= {1,10,10,1};

    [Header("Ulozenie")]
    public int Separate;
    public Vector3 Start;

    private int Generation;

    private void Awake()
    {
        MutText.text = "Mutation: " + Mutation+"%";
        MutSlider.value = Mutation;
    }
    public void MutationChange()
    {
        Mutation=MutSlider.value;
        MutText.text = "Mutation: " + Mutation + "%";
    }

    public void Information()
    {
        UserInterface.PopulationRating.text = "";
        for (int i = 0; i < Population.Length; i++)
        {
            UserInterface.PopulationRating.text+=Population[i].fitnes+ "\tP" + i + ": " + "\n";
        }
        Invoke("Information", 0.1f);
    }

    public void StartSimulation()
    {
        Population = new Model[Size];
        Quaternion zero = new Quaternion(0, 0, 0, 0);
        for (int i = 0; i < Population.Length; i++)
        {
            Vector3 position = Start;
            position.x = i * Separate;
            GameObject a = Instantiate(PreFabs, position, zero, this.transform.parent);
            Model b = a.GetComponent<Model>();
            b.Creation(NeuralSchemat);
            Population[i] = b;
            Population[i].name = i+" model";
        }
        UserInterface.GenerationInformation.text = Generation+"\t:Generation";
        UserInterface.PopulationInformation.text = Size+"\t:Population";
        Invoke("Information",0.1f);
    }

    public void StopSimulation()
    {
        foreach (var pop in Population)
        {
            Destroy(pop.gameObject);
        }
    }
}
