using UnityEngine;

public class NeutralNetwork : MonoBehaviour
{


    private int[] layers;
    private float[][] outputs;
    private float[][][] inputWeight;
    private float bias=1;
    public int NumberOFConnections;

    public int[] Layers
    {
        set { layers = value; }
        get { return layers; }
    }
    public float[][][] InputWeight
    {
        set { inputWeight = value; }
        get { return inputWeight; }
    }
    public float[][] OutupWeight
    {
        set { outputs = value; }
        get { return outputs; }
    }

    public NeutralNetwork(int[] layerSchema)
    {
        layers = layerSchema;
        outputs = new float[layerSchema.Length][];
        inputWeight = new float[layerSchema.Length - 1][][];
        //zliczanie połączeń
        NumberOFConnections = 0;
        for (int i = 1; i < layerSchema.Length; i++)
        {
            NumberOFConnections += (layerSchema[i] * layerSchema[i - 1]);
        }
        //Inicjalizowanie Wejść i wyjść Neuronowych
        int m = 0;
        for (int i = 0; i < inputWeight.Length; i++)
        {
            inputWeight[i] = new float[layerSchema[i + 1]][];
            outputs[i] = new float[layerSchema[i]];
            for (int j = 0; j < inputWeight[i].Length; j++)
            {
                inputWeight[i][j] = new float[layerSchema[i]];
                for (int n = 0; n < inputWeight[i][j].Length; n++)
                {
                    //Przypisuje wagę neuronu
                    inputWeight[i][j][n] = UnityEngine.Random.Range(-0.25f, 0.25f);
                    m++;
                }
            }
        }
        //Zainicjalizowanie ostateniej warstwy wyjściowej
        outputs[layerSchema.Length - 1] = new float[layerSchema[layerSchema.Length - 1]];
        bias = 1;
    }

    public NeutralNetwork(int[] v1, float[][] v2, float[][][] v3,int s1)
    {
        layers = v1;
        outputs = v2;
        inputWeight = v3;
        NumberOFConnections = s1;
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
                outputs[i + 1][j] = signal;
            }
        }
        //Zwracam wartosci na wyjściach neuronowych
        return outputs[layers.Length - 1];
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
                    float lotto = UnityEngine.Random.Range(0f, 100f);
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
        int los = UnityEngine.Random.Range(0, 5);
        float procent = 0;
        switch (los)
        {
            case 0:
                v = -v;
                break;
            case 1:
                v = UnityEngine.Random.Range(0f, 1f);
                break;
            case 2:
                v = UnityEngine.Random.Range(-1f, 0f);
                break;
            case 3:
                procent = UnityEngine.Random.Range(0f, 100f);
                v += v * procent;
                break;
            case 4:
                procent = UnityEngine.Random.Range(0f, 100f);
                v -= v * procent;
                break;
            case 5:
                v = UnityEngine.Random.Range(-.25f, .25f);
                break;
        }
        return v;
    }

    public void ApplyNewWeights(float[] weights)
    {
        int k = 0;
        for (int i = 0; i < inputWeight.Length; i++)
        {
            for (int j = 0; j < inputWeight[i].Length; j++)
            {
                for (int n = 0; n < inputWeight[i][j].Length; n++)
                {
                    inputWeight[i][j][n] = weights[k];
                    k++;
                }
            }
        }
    }

    public void WrtieWeights(int start, int end, ref float[] weights)
    {
        int k = start;
        for (int i = 0; i < inputWeight.Length; i++)
        {
            for (int j = 0; j < inputWeight[i].Length; j++)
            {
                for (int n = 0; n < inputWeight[i][j].Length; n++)
                {
                    weights[k] = inputWeight[i][j][n];
                    k++;
                    if (k > end)
                    {
                        break;
                    }
                }
                if (k > end)
                {
                    break;
                }
            }
            if (k > end)
            {
                break;
            }
        }
    }
}
