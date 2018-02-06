using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;


public static class SaveLoad
{
    public static void SaveSimulation(GenerationManager manager)
    {
        Debug.Log(DateTime.Now + " Rozpoczynam zapisanie pliku");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/Symulation.op", FileMode.Create);
        Data data = new Data(manager);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static string[] LoadSimulation()
    {
        Debug.Log(DateTime.Now + " Rozpoczynam Wczytywanie pliku");
        if (File.Exists(Application.persistentDataPath + "/Symulation.op"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/Symulation.op", FileMode.Open);

            Data data = bf.Deserialize(stream) as Data;

            stream.Close();
            return data.stats;
        }
        else
        {
            return new string[1];
        }
    }

    public static bool CheckFile()
    {
        if (File.Exists(Application.persistentDataPath + "/Symulation.op"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
[Serializable]
public class Data
{
    public string[] stats;

    public Data(GenerationManager manager)
    {
        stats = new string[11];
        stats[0] = manager.Size.ToString();
        stats[1] = manager.Mutation.ToString();
        stats[2] = manager.LifeTime.ToString();
        stats[3] = manager.Generation.ToString();
        stats[4] = manager.startTime.ToString();
        for (int i = 0; i < manager.NeuralSchemat.Length; i++)
        {
            if((i+1) == manager.NeuralSchemat.Length)
            {
                stats[5] += manager.NeuralSchemat[i] + "";
            }
            else
            {
                stats[5] += manager.NeuralSchemat[i] + "|";
            }
        }
        //zapis populacji
        List<int[]> layers = new List<int[]>();
        List<float[][]> outputs = new List<float[][]>();
        List <float[][][]> weights=new List<float[][][]>();
        int[] simple1= manager.Population[0].neuralNetwork.Layers;
        float[][] simple2=manager.Population[0].neuralNetwork.OutupWeight;
        float[][][] simple3= manager.Population[0].neuralNetwork.InputWeight;
        for (int i = 0; i < manager.Size; i++)
        {
            layers.Add(manager.Population[i].neuralNetwork.Layers);
            outputs.Add(manager.Population[i].neuralNetwork.OutupWeight);
            weights.Add(manager.Population[i].neuralNetwork.InputWeight);
        }
        for (int p = 0; p < manager.Size; p++)
        {
            for (int i = 0; i < simple3.Length; i++)
            {
                stats[6] += layers[p][i] + "|";
                for (int j = 0; j < simple3[i].Length; j++)
                {
                    
                    for (int n = 0; n < simple3[i][j].Length; n++)
                    {
                        if ((i + 1) == simple3.Length && (j + 1) == simple3[i].Length && (n + 1) == simple3[i][j].Length)
                        {
                            stats[8] += weights[p][i][j][n] + "";
                        }
                        else
                        {
                            stats[8] += weights[p][i][j][n] + "|";
                        }
                    }
                }
                
            }
            for (int i = 0; i < simple2.Length; i++)
            {
                for (int j = 0; j < simple2[i].Length; j++)
                {
                    if ((i + 1) == simple2.Length && (j + 1) == simple2[i].Length)
                    {
                        stats[7] += outputs[p][i][j] + "";
                    }
                    else
                    {
                        stats[7] += outputs[p][i][j] + "|";
                    }
                }
            }
            stats[6] += layers[p][layers[p].Length-1] + "";
            if ((p + 1) == manager.Size)
            {
                stats[6] += "";
                stats[7] += "";
                stats[8] += "";
            }else
            {
                stats[6] += "X";
                stats[7] += "X";
                stats[8] += "X";
            }
        }
        stats[9] = manager.dataRecorder.output;
        stats[10] = manager.SchematSize.ToString();
        Debug.Log("Complete Save");
        //
    }
}

