using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class GenerationManager : MonoBehaviour
{
    public DataRecorder dataRecorder;
    public UIControler UserInterface;
    public GameObject PreFabs;
    public InputField MutaChanseInputField;
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

    public int Generation;
    public float startTime;

    
    private void Awake()
    {
        bool file =SaveLoad.CheckFile();
        UserInterface.Load.interactable = file;
        UserInterface.Save.interactable = false;
        UserInterface.connect = this;
        TimeMultiplication = Time.timeScale;
        UserInterface.TMuiltiplicField.text = Time.timeScale.ToString("f0");
        UserInterface.Popfield.text = Size.ToString();
        startTime = Time.time;
        MutaChanseInputField.text = Mutation.ToString();
        TimeInput.text = LifeTime.ToString();
        UserInterface.FillList(NeuralSchemat);
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
        float n;
        float.TryParse( MutaChanseInputField.text, out n);
        if (n > 100) n = 100;
        if (n < 0) n = 0;
        MutaChanseInputField.text = n+"%";
       Mutation = n;
    }
    public void TimeChange()
    {
        Int32.TryParse(TimeInput.text, out LifeTime);
    }
    private float timeOfStartGeneration;

    public void Information()
    {
        if (startEvolution)
        {
            UserInterface.PopulationRating.text = "";
            for (int i = 0; i < Population.Length; i++)
            {
                if (Population[i].Highlight != null)
                {
                    UserInterface.PopulationRating.text += ">>>";
                }
                UserInterface.PopulationRating.text += "Individual " + (i + 1) + " Fit:" + Population[i].fitnes.ToString("f1") + "\t Last Mut:" + Population[i].lastMutation + "\n";
            }
            Invoke("Information", 0.01f);
        }
    }
    public void LoadSimulation()
    {

        //Load a save;
        string[] loadedSave=SaveLoad.LoadSimulation();
        //CheckCorrection;
        if (loadedSave.Length != 11)
        {
            Debug.LogError("Błedny Save");
            return;
        }
        int.TryParse(loadedSave[0],out Size);
        float.TryParse(loadedSave[1], out Mutation);
        int.TryParse(loadedSave[2], out LifeTime);
        int.TryParse(loadedSave[3], out Generation);
        float.TryParse(loadedSave[4], out startTime);
        int.TryParse(loadedSave[10], out SchematSize);

        char IndividualSpacer = 'X';
        char Spacer = '|';
        string[] temporary = loadedSave[5].Split(Spacer);
        NeuralSchemat = new int[temporary.Length];
        for (int i = 0; i < temporary.Length; i++)
        {
             int.TryParse(temporary[i],out NeuralSchemat[i]);
        }


        UserInterface.Load.interactable = false;
        UserInterface.Save.interactable = true;
        timeOfStartGeneration = Time.time;
        Population = new Model[Size];

        List<int[]> layers = new List<int[]>();
        List<float[][]> outputs = new List<float[][]>();
        List<float[][][]> weights = new List<float[][][]>();


        string[] TemporaryLayers = loadedSave[6].Split(IndividualSpacer);
        string[] TemporaryOutputs = loadedSave[7].Split(IndividualSpacer);
        string[] TemporaryInputs = loadedSave[8].Split(IndividualSpacer);
        for (int p = 0; p < Size; p++)
        {
            //
            temporary = TemporaryLayers[p].Split(Spacer);
            int[] temp=new int[temporary.Length];
            for (int i = 0; i < temporary.Length; i++)
            {
                int.TryParse(temporary[i], out temp[i]);
            }
            layers.Add(temp);
            string[] outp = TemporaryOutputs[p].Split(Spacer);
            float[][] output;
            output = new float[NeuralSchemat.Length][];
            int t = 0;
            for (int i = 0; i < NeuralSchemat.Length; i++)
            {
                output[i] = new float[NeuralSchemat[i]];
                for (int j = 0; j < NeuralSchemat[i]; j++)
                {
                    float.TryParse(outp[t], out output[i][j]);
                    t++;
                }
            }
            outputs.Add(output);
            string[] inp = TemporaryInputs[p].Split(Spacer);
            float[][][] input;
            input = new float[NeuralSchemat.Length - 1][][];
            t = 0;
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new float[NeuralSchemat[i+1]][];
                for (int j = 0; j < input[i].Length; j++)
                {
                    input[i][j] = new float[NeuralSchemat[i]];
                    for (int n = 0; n < input[i][j].Length; n++)
                    {
                        float.TryParse(inp[t], out input[i][j][n]);
                        t++;
                    }
                }
            }
            weights.Add(input);

            Vector3 position = Start;
            position.x = p * Separate;
            GameObject a = Instantiate(PreFabs, position, PreFabs.transform.rotation, this.transform.parent);
            Model b = a.GetComponent<Model>();
            b.Creation(layers[p], outputs[p], weights[p], UserInterface,SchematSize);
            Population[p] = b;
            Population[p].id = p;
            Population[p].Name = Population[p].name = p + " model";
        }

        MutaChanseInputField.text = Mutation.ToString();
        UserInterface.Popfield.text = Size.ToString();
        TimeInput.text = LifeTime.ToString();
        UserInterface.DeleteList();
        UserInterface.FillList(NeuralSchemat);
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        UserInterface.PopulationInformation.text = Size + "\t:Population";
        Invoke("Information", 0.1f);
        string s = Time.time.ToString();
        dataRecorder = new DataRecorder(s, Size, NeuralSchemat, SchematSize, LifeTime, Mutation);
        dataRecorder.output = loadedSave[10];
        foreach (var individual in Population)
        {
            individual.active = true;
        }
        startEvolution = true;
    }
    public void SaveSimulation()
    {
        startEvolution = false;
        foreach (var individual in Population)
        {
            individual.active = false;
        }
        SaveLoad.SaveSimulation(this);
        StopSimulation();
    }


    public void StartSimulation()
    {
        UserInterface.Load.interactable = false;
        UserInterface.Save.interactable = true;
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
            Population[i].id = i;
            Population[i].Name = Population[i].name = i + " model";
        }
        NeuralSchemat[0] = Population[0].input.Length;
        NeuralSchemat[NeuralSchemat.Length - 1] = Population[0].OutputBone.Length;
        SchematSize = Population[0].neuralNetwork.NumberOFConnections;
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        UserInterface.PopulationInformation.text = Size + "\t:Population";
        Invoke("Information", 0.1f);
        string s = Time.time.ToString();
        dataRecorder = new DataRecorder(s, Size, NeuralSchemat, SchematSize, LifeTime, Mutation);
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

            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    if (UserInterface.DataPanel.active)
            //    {
            //        UserInterface.DataPanel.SetActive(false);
            //    }
            //    else
            //    {
            //        UserInterface.DataPanel.SetActive(true);
            //        UserInterface.DataText.text = dataRecorder.ShowData();
            //    }
            //}
        }


    }
    public void WriteString(Button b)
    {
        startEvolution = false;
        string path = "Assets/Logs/dataRecorder.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(dataRecorder.ShowData());
        writer.Close();
        foreach (var individual in Population)
        {
            individual.active = false;
        }
        float[] neural = new float[SchematSize];
        string s = Population[lastbest].name + "\t";
        Population[lastbest].neuralNetwork.WrtieWeights(0, SchematSize, ref neural);
        for (int j = 0; j < neural.Length; j++)
        {
            s += neural[j] + " ";
        }
        path = "Assets/Logs/" + Population[lastbest].name + ".txt";
        writer = new StreamWriter(path, true);
        writer.WriteLine(s);
        writer.Close();
        Debug.Log("Save end");
        startEvolution = true;
        foreach (var individual in Population)
        {
            individual.active = true;
        }
        b.interactable = true;
    }

    float[] newThreeBest = new float[3];
    float[] lastThreeBest = new float[3];
    int lastbest;
    private void StartNewGeneration()
    {
        //Turn off Models
        float[] lastFitness = new float[Population.Length];
        string[] lastParents = new string[Population.Length];
        float[] lastMutation = new float[Population.Length];
        foreach (var individual in Population)
        {
            individual.active = false;
        }
        //fit parens, mut
        //Sort fitness.
        Model[] Sorted = Population.OrderBy(p => p.fitnes).ToArray();
        //Remember last 3 best
        lastbest = Sorted[Sorted.Length - 1].id;
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
        int k = Sorted.Length-1;
        for (int i = 0; i < Sorted.Length; i++)
        {
            
            string parents = "NoN";
            lastFitness[Sorted[i].id] = Sorted[i].fitnes;
            Sorted[i].lastMutation = 0;
            int half = (Sorted.Length / 2);
            if (i < half)
            {
                //create a child
                int whereCut = UnityEngine.Random.Range(2, SchematSize - 3);
                //build new Neural network
                float[] Weights = new float[SchematSize];
                Sorted[k].neuralNetwork.WrtieWeights(0, whereCut, ref Weights);
                Sorted[k - 1].neuralNetwork.WrtieWeights(whereCut, SchematSize - 1, ref Weights);
                parents = Sorted[k].id + "x" + Sorted[k -1].id;
                Sorted[i].ColorMixer(Sorted[k].MySkin.material.color, Sorted[k- 1].MySkin.material.color);
                k--;
                //implement new neural network
                Sorted[i].neuralNetwork.ApplyNewWeights(Weights);
                //Mutation
                Sorted[i].Mutate(Mutation);
            }
            else
            {
            }
            lastParents[Sorted[i].id] = parents;
            lastMutation[Sorted[i].id] = Sorted[i].lastMutation;
            float lotto = UnityEngine.Random.Range(0f, 100f);
            Sorted[i].ResetPostion();
        }
        Generation++;
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        //Turn On Models
        foreach (var individual in Population)
        {
            individual.active = true;
        }
        dataRecorder.NexGenRecord(LifeTime, Mutation, lastFitness, lastParents, lastMutation);
        UserInterface.DataText.text = dataRecorder.ShowData();
    }


    public void StopSimulation()
    {
        Generation = 0;
        startEvolution = false;
        UserInterface.GenerationInformation.text = Generation + "\t:Generation";
        UserInterface.PopulationInformation.text = Size + "\t:Population";
        foreach (var pop in Population)
        {
            Destroy(pop.gameObject);
        }
        UserInterface.Load.interactable = SaveLoad.CheckFile();
        UserInterface.Save.interactable = true;
    }
}
