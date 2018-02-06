
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

    class Data
    {
        public int LifeTime;
        public float MutationChance;
        public float[] fitness;
        public string[] Parents;
        public float[] LastMutation;
        public Data(int L, float M, float[] F, string[] P, float[] LM)
        {
            LifeTime = L;
            MutationChance = M;
            fitness = F;
            Parents = P;
            LastMutation = LM;
        }

    }
    private List<Data> data = new List<Data>();

    public DataRecorder(string startDate, int PopCount, int[] Schema, int Connections, int lifetime, float mutchance)
    {
        StartDate = startDate;
        Generations = 1;
        PopulationCount = PopCount;
        LayerShemat = new int[Schema.Length];
        for (int i = 0; i < Schema.Length; i++)
        {
            LayerShemat[i] = Schema[i];
        }
        ConnectionCount = Connections;

        output = "This Test Start: " + StartDate + "\t Population cout: " + PopulationCount + "\t Connectios: " + ConnectionCount + "\t\t Neural Network Schemat: |";
        for (int i = 0; i < LayerShemat.Length; i++)
        {
            output += LayerShemat[i] + "|";
        }
        output += "\n";
    }
    public string output;
    public string ShowData()
    {
        return output;
    }

    public void NexGenRecord(int L, float M, float[] F, string[] P, float[] LM)
    {
        Data next = new Data(L, M, F, P, LM);
        data.Add(next);
        int i = data.Count-1;
        output += "Generation: " + i + "\t Life for this generation: " + data[i].LifeTime + "\t Mutation Chance for this generation" + data[i].MutationChance + "|\nPop:\t\t";
        for (int j = 0; j < PopulationCount; j++)
        {
            output += "\tIndividual:\t" + j + "\t" + data[i].LastMutation[j] + "\tFit:" + data[i].fitness[j].ToString("2f") + "\t";
        }
        output += "\n";
    }
    
}