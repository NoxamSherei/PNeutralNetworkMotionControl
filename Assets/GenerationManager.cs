using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GenerationManager : MonoBehaviour
{
    public DataRecorder dataRecorder;
    public UIControler UserInterface;
    public GameObject PreFabs;
    public Slider MutSlider;
    public Text MutText;
    public InputField TimeInput;
    public float TimeMultiplication;
    [Header("Dane Wejsciowe")]
    public int Size;
    public float Mutation;
    public int LifeTime;
    public Model[] Population;
    public int[] NeuralSchemat = { 1, 10, 10, 1 };
    public int SchematSize;

    [Header("Ulozenie")]
    public int Separate;
    public Vector3 Start;

    private int Generation;
    private float startTime;
    

    private void Awake()
    {
        TimeMultiplication = Time.timeScale;
        UserInterface.TMuiltiplicField.text = Time.timeScale.ToString("f0");
        UserInterface.TMuiltiplicSlider.value = Time.timeScale;
        startTime = Time.time;
        MutText.text = "Mutation: " + Mutation + "%";
        MutSlider.value = Mutation;
        TimeInput.text = LifeTime.ToString();
    }
    public void ChageTimeSpeed(bool v)
    {
        switch (v)
        {
            case true:
                Time.timeScale = UserInterface.TMuiltiplicSlider.value;
                break;
            case false:
                float m;
                float.TryParse(UserInterface.TMuiltiplicField.text, out m);
                if (m > 10) m = 10;
                Time.timeScale = m;
                break;
        }
        UserInterface.TMuiltiplicField.text = Time.timeScale.ToString("f0");
    }

    public void MutationChange()
    {
        Mutation = MutSlider.value;
        MutText.text = "Mutation: " + Mutation + "%";
    }
    public void TimeChange()
    {
        Int32.TryParse(TimeInput.text, out LifeTime);
    }
    private float timeOfStartGeneration;

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
                UserInterface.PopulationRating.text += ">>>" + Population[i].fitnes.ToString("f1") + "\tP" + i + " LMut:" + Population[i].lastMutation + "<<<\n";
            }
        }
        Invoke("Information", 0.01f);
    }

    public void StartSimulation()
    {
        startTime = Time.time;
        timeOfStartGeneration = Time.time;
        Population = new Model[Size];
        for (int i = 0; i < Population.Length; i++)
        {
            Vector3 position = Start;
            position.x = i * Separate;
            GameObject a = Instantiate(PreFabs, position, PreFabs.transform.rotation, this.transform.parent);
            Model b = a.GetComponent<Model>();
            b.Creation(NeuralSchemat, UserInterface);
            Population[i] = b;
            Population[i].Name = Population[i].name = i + " model";
        }
        NeuralSchemat[0] = Population[0].input.Length;
        NeuralSchemat[NeuralSchemat.Length - 1] = Population[0].OutputBone.Length;
        SchematSize = Population[0].neuralNetwork.NIDC;
        //UserInterface.GenerateLayers(NeuralSchemat);
        //UserInterface.ShowDownLayers();
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        UserInterface.PopulationInformation.text = Size + "\t:Population";
        Invoke("Information", 0.1f);
        dataRecorder = new DataRecorder(DateTime.UtcNow.ToString(), Size, NeuralSchemat, SchematSize, LifeTime, Mutation);
        startEvolution = true;
    }
    private bool startEvolution = false;
    private void FixedUpdate()
    {
        float SimTime = Time.time - startTime;
        int secondsFloat = (int)SimTime % 60;
        int minutes = (int)SimTime / 60;
        UserInterface.TimeText.text = "Time:" + String.Format("{0:00}:{1:00}", minutes, secondsFloat) + " Time Scale:" + Time.timeScale.ToString("f2");

        if (startEvolution)
        {

            float check = Time.time - timeOfStartGeneration;
            if (check >= LifeTime)
            {
                timeOfStartGeneration = Time.time;
                StartNewGeneration();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (UserInterface.DataPanel.active)
                {
                    UserInterface.DataPanel.SetActive(false);
                }
                else
                {
                    UserInterface.DataPanel.SetActive(true);
                }
            }
        }
        if (UserInterface.DataPanel.active)
        {
            UserInterface.DataText.text = dataRecorder.ShowData();
        }
    }
    float[] newThreeBest = new float[3];
    float[] lastThreeBest = new float[3];
    private void StartNewGeneration()
    {
        //Turn off Models
        foreach (var individual in Population)
        {
            individual.active = false;
        }
        //Sort fitness.
        Model[] Sorted = Population.OrderBy(p => p.fitnes).ToArray();
        //Remember last 3 best
        for (int i = 0; i < lastThreeBest.Length; i++)
        {
            lastThreeBest[i] = newThreeBest[i];
            newThreeBest[i] = Sorted[Sorted.Length - 1 - i].fitnes;
        }
        string ratting;
        ratting = "Last Winners\n";
        for (int i = 0; i < lastThreeBest.Length; i++)
        {
            ratting += (i + 1) + " F:" + newThreeBest[i].ToString("f2") + " LastF:" + lastThreeBest[i].ToString("f2") + " Prog:" + (newThreeBest[i] - lastThreeBest[i]).ToString("f2") + "\n";
        }
        UserInterface.Ratting.text = ratting;
        //Save Best Half and Create new Half from best, aka Algorithm evolutionStrategy
        int k = 0;
        for (int i = 0; i < Sorted.Length; i++)
        {
            int half = (Sorted.Length / 2);
            if (i < half)
            {
                //create a child
                int whereCut = UnityEngine.Random.Range(2, SchematSize - 2);
                //build new Neural network
                float[] Weights = new float[SchematSize];
                Sorted[k].neuralNetwork.WrtieWeights(0, whereCut, ref Weights);
                Sorted[k + 1].neuralNetwork.WrtieWeights(whereCut, SchematSize - 1, ref Weights);
                Sorted[i].ColorMixer(Sorted[k].MySkin.material.color, Sorted[k + 1].MySkin.material.color);
                k++;
                Sorted[i].neuralNetwork.ApplyNewWeights(Weights);
                //Mutate Child
                Sorted[i].Mutate(Mutation);
            }
            else
            {
                Sorted[i].lastMutation = 0;
            }
            Sorted[i].ResetPostion();
        }
        Generation++;
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        //Turn On Models
        foreach (var individual in Population)
        {
            individual.active = true;
        }
    }
    public void StopSimulation()
    {
        startEvolution = false;
        foreach (var pop in Population)
        {
            Destroy(pop.gameObject);
        }
    }
}
