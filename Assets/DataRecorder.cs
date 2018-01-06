using System;
using System.Collections.Generic;

public class DataRecorder 
{
    //to codename
    public string StartDate;
    public string TestTime;

    public int Generations;
    public int PopulationCount;
    public int[] LayerShemat;
    public int ConnectionCount;

    public List<int> LifeTime;
    public List<float> MutationChance;
    //end data
    public List<float> fitness;
    //public List<float> Velocity;
    //public List<float> Distance;
    //nex gen start data
    public List<string> Parents;
    public List<float> LastMutation;

    public DataRecorder(string startDate, int PopCount, int[] Schema, int Connections, int lifetime , float mutchance)
    {
        Parents = new List<string>();
        LastMutation = new List<float>();
        fitness = new List<float>();
        MutationChance = new List<float>();
        LifeTime = new List<int>();
        StartDate = startDate;
        Generations = 1;
        PopulationCount = PopCount;
        LayerShemat = new int[Schema.Length];
        for (int i = 0; i < Schema.Length; i++)
        {
            LayerShemat[i] = Schema[i];
        }
        ConnectionCount = Connections;
        LifeTime.Add(lifetime);
        MutationChance.Add(mutchance);
    }

    public string ShowData()
    {
        string output= StartDate+"\t"+ PopulationCount+"\t"+ ConnectionCount+"\t|";
        for (int i = 0; i < LayerShemat.Length; i++)
        {
            output += LayerShemat[i]+"|";
        }


        return output;
    }
}