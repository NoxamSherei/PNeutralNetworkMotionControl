using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GenerationManager : MonoBehaviour {
    
    public UIControler UserInterface;
    public GameObject PreFabs;
    public Slider MutSlider;
    public Text MutText;
    public InputField TimeInput;

    [Header("Dane Wejsciowe")]
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
        TimeInput.text = LifeTime.ToString();
    }
    public void MutationChange()
    {
        Mutation=MutSlider.value;
        MutText.text = "Mutation: " + Mutation + "%";
    }
    public void TimeChange()
    {
        Int32.TryParse(TimeInput.text,out LifeTime);
    }

    public void Information()
    {
        UserInterface.PopulationRating.text = "";
        for (int i = 0; i < Population.Length; i++)
        {
            if (Population[i].Highlight == null)
            {
                UserInterface.PopulationRating.text += Population[i].fitnes.ToString("f1") + "\tP" + i + " LMut:" + Population[i].lastMutation + "\n";
            }
            else
            {
                UserInterface.PopulationRating.text +=">>>"+ Population[i].fitnes.ToString("f1") + "\tP" + i + " LMut:" + Population[i].lastMutation + "<<<\n";
            }
        }
        Invoke("Information", 0.01f);
    }

    public void StartSimulation()
    {
        Population = new Model[Size];
        for (int i = 0; i < Population.Length; i++)
        {
            Vector3 position = Start;
            position.x = i * Separate;
            GameObject a = Instantiate(PreFabs, position, PreFabs.transform.rotation, this.transform.parent);
            Model b = a.GetComponent<Model>();
            b.Creation(NeuralSchemat,UserInterface);
            Population[i] = b;
            Population[i].Name= Population[i].name = i+" model";
        }
        NeuralSchemat[0] = Population[0].input.Length;
        NeuralSchemat[NeuralSchemat.Length-1] = Population[0].OutputBone.Length;
        //UserInterface.GenerateLayers(NeuralSchemat);
        //UserInterface.ShowDownLayers();
        UserInterface.GenerationInformation.text = Generation+"\t:Generation";
        UserInterface.PopulationInformation.text = Size+"\t:Population";
        Invoke("Information",0.1f);
        Invoke("NexGen",LifeTime);
    }

    private void NexGen()
    {
        foreach (var pop in Population)
        {
            pop.active = false;
        }
        Model[] Sorted = Population.OrderBy(p => p.fitnes).ToArray();
        for (int i = 0; i < Sorted.Length; i++)
        {
            int half = (Sorted.Length / 2);
            if (i<half)
            {
                Sorted[i].Mutate(Mutation);
            }else
            {
                Sorted[i].lastMutation = 0;
            }
            Sorted[i].ResetPostion();
        }
        Generation++;
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        foreach (var pop in Population)
        {
            pop.active = true;
        }
        Invoke("NexGen", LifeTime);
    }

    public void StopSimulation()
    {
        foreach (var pop in Population)
        {
            Destroy(pop.gameObject);
        }
    }
}
