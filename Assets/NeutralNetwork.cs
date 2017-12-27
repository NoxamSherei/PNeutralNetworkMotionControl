
using UnityEngine;

public class NeutralNetwork : MonoBehaviour {
    

    private int[] layers;
    private float[][] outputs;
    private float[][][] inputWeight;
    private float[] NeuronInputsDirectConnection;
    private float bias;

    public NeutralNetwork(int[] layerSchema)
    {
        layers= layerSchema;
        outputs=new float[layerSchema.Length][];
        inputWeight = new float[layerSchema.Length-1][][];
        //Zliczenie Ilości Wejść
        int NIDC = 0;
        for (int i = 1; i < layerSchema.Length; i++)
        {
            NIDC += (layerSchema[i] * layerSchema[i - 1]);
        }
        NeuronInputsDirectConnection=new float[NIDC];
        //Inicjalizowanie Wejść i wyjść Neuronowych
        int m = 0;
        for (int i = 0; i < inputWeight.Length; i++)
        {
            inputWeight[i] = new float[layerSchema[i+1]][];
            outputs[i] = new float[layerSchema[i]];
            for (int j = 0; j < inputWeight[i].Length; j++)
            {
                inputWeight[i][j] = new float[layerSchema[i]];
                for (int n = 0; n < inputWeight[i][j].Length; n++)
                {
                    //Przypisuje wagę neuronu
                    inputWeight[i][j][n] = Random.Range(-0.5f,0.5f);
                    NeuronInputsDirectConnection[m] = inputWeight[i][j][n];
                    m++;
                }
            }
        }
        //Zainicjalizowanie ostateniej warstwy wyjściowej
        outputs[layerSchema.Length-1] = new float[layerSchema[layerSchema.Length - 1]];
        bias = 1;
    }

    public float[] FeedForward(float[] inputs)
    {
        outputs[0] = inputs;
        for (int i = 0; i < inputWeight.Length; i++)
        {
            for (int j = 0; j < inputWeight[i].Length; j++)
            {
                float signal = 0;
                for (int n = 0; n < inputWeight[i][j].Length; n++)
                {
                    signal += outputs[i][n] * inputWeight[i][j][n];
                }
                signal += bias;
                signal = Mathf.Sign(signal);
                outputs[i+1][j]=signal;
            }
        }
        //Zwracam wartosci na wyjściach neuronowych
        return outputs[layers.Length-1];
    }

    public int Mutate(float chance)
    {
        int m = 0;
        for (int i = 0; i < inputWeight.Length; i++)
        {
            for (int j = 0; j < inputWeight[i].Length; j++)
            {
                for (int n = 0; n < inputWeight[i][j].Length; n++)
                {
                    float lotto = Random.Range(0f,100f);
                    if (lotto >= (100 - chance))
                    {
                        inputWeight[i][j][n] = MutateOperation(inputWeight[i][j][n]);
                        m++;
                    }
                }
            }
        }
        return m;
    }

    private float MutateOperation(float v)
    {
        int los = Random.Range(0, 4);
        float procent = 0;
        switch (los)
        {
            case 0:
                v = -v;
                break;
            case 1:
                v= Random.Range(0f, 1f);
                break;
            case 2:
                v= Random.Range(-1f, 0f);
                break;
            case 3:
                procent= Random.Range(0f,100f);
                v += v * procent;
                break;
            case 4:
                procent= Random.Range(0f, 100f);
                v -= v * procent;
                break;
        }
        return v;
    }
}
