using UnityEngine;

public class Model : MonoBehaviour
{
    public int id;
    public float fitnes = 0;
    public NeutralNetwork neuralNetwork;
    public Light point1;
    public string Name;
    public SkinnedMeshRenderer MySkin;
    public GameObject[] OutputBone;
    public GameObject[] InputInformation;
    private Vector3 LastPos;
    private Vector3[] StartPos;
    private Quaternion[] StartRot;
    private UIControler Gen;
    public bool active = false;

    public int lastMutation = 0;
    public float[] input;
    public float[] blockade;
    private float[] lastVelocity = new float[3];
    private float[] acceleration = new float[3];
    public void Creation(int[] layerShema, UIControler mana)
    {
        Gen = mana;
        LastPos = this.transform.position;
        StartPos = new Vector3[InputInformation.Length];
        StartRot = new Quaternion[InputInformation.Length];
        for (int i = 0; i < InputInformation.Length; i++)
        {
            StartPos[i] = InputInformation[i].transform.position;
            StartRot[i] = InputInformation[i].transform.localRotation;
        }
        input = new float[InputInformation.Length  + OutputBone.Length+3+3+3+2];
        layerShema[0] = input.Length;
        layerShema[layerShema.Length - 1] = OutputBone.Length;
        blockade= new float[OutputBone.Length];
        neuralNetwork = new NeutralNetwork(layerShema);
        ChoseColor();
        active = true;
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string inf="name: "+this.name+"\n";
            for (int i = 0; i < InputInformation.Length; i++)
            {
                inf += i+" Position: " + InputInformation[i].transform.position + " LocalPosition:" + InputInformation[i].transform.localPosition+"\n ";
            }
            Debug.Log(inf);
        }
        if (active)
        {
            //Download Position Rotation Change
            Vector3 position = InputInformation[0].transform.position;
            Quaternion rotation = InputInformation[0].transform.localRotation;

            input[input.Length - 9]=acceleration[0] = (((position.x - LastPos.x) / Time.deltaTime)- lastVelocity[0]) / Time.deltaTime;
            input[input.Length - 10]=acceleration[1] = (((position.y - LastPos.y) / Time.deltaTime) - lastVelocity[0]) / Time.deltaTime;
            input[input.Length - 11]=acceleration[2] = (((position.z - LastPos.z) / Time.deltaTime) - lastVelocity[0]) / Time.deltaTime;
            input[input.Length - 1] = position.x - LastPos.x;
            input[input.Length - 2] = position.y - LastPos.y;
            input[input.Length - 3] = position.z - LastPos.z;
            input[input.Length - 4] = rotation.x;
            input[input.Length - 5] = rotation.z;
            input[input.Length - 6] = lastVelocity[0] = input[input.Length - 1] / Time.deltaTime;
            input[input.Length - 7] = lastVelocity[1] = input[input.Length - 1] / Time.deltaTime;
            input[input.Length - 8] = lastVelocity[2] = input[input.Length - 1] / Time.deltaTime;
            TestFitnes();
            LastPos = position;
            //Download Bones Rotate
            for (int i = 0; i < InputInformation.Length; i++)
            {
                input[i] = InputInformation[i].transform.localRotation.y;
            }
            for (int i = 0; i < OutputBone.Length; i++)
            {
                input[i+ InputInformation.Length] = blockade[i];
            }
            //Lunch Neural Network
            float[] output = neuralNetwork.FeedForward(input);
            //Move bones
            for (int i = 0; i < output.Length; i++)
            {
                Quaternion rotate = new Quaternion(0, 0, 0, 0);
                if (output[i] > 0)
                {
                    rotate = OutputBone[i].transform.localRotation;
                    float test = (rotate.y + 0.005f);
                    if (test < 0.35f)
                    {
                        rotate.y = rotate.y + 0.005f;
                        OutputBone[i].transform.localRotation = rotate;
                        blockade[i] =0;
                    }
                    else
                    {
                        blockade[i] = 1;
                    }
                }
                else if (output[i] < 0)
                {

                    rotate = OutputBone[i].transform.localRotation;
                    float test = (rotate.y - 0.005f);
                    if (test > -0.45f)
                    {
                        rotate.y = rotate.y - 0.005f;
                        OutputBone[i].transform.localRotation = rotate;
                        blockade[i] = 0;
                    }
                    else
                    {
                        blockade[i] = -1;
                    }
                }
            }
        }
    }
    public void ChoseColor()
    {
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        MySkin.material.color = color;
    }

    public void Creation(int[] v1, float[][] v2, float[][][] v3, UIControler userInterface,int s1)
    {

        Gen = userInterface;
        LastPos = this.transform.position;
        StartPos = new Vector3[InputInformation.Length];
        StartRot = new Quaternion[InputInformation.Length];
        for (int i = 0; i < InputInformation.Length; i++)
        {
            StartPos[i] = InputInformation[i].transform.position;
            StartRot[i] = InputInformation[i].transform.localRotation;
        }
        input = new float[InputInformation.Length + OutputBone.Length + 3 + 3 + 3 + 2];
        blockade = new float[OutputBone.Length];
        neuralNetwork = new NeutralNetwork(v1,v2,v3,s1);
        ChoseColor();
    }

    public void ColorMixer(Color c1, Color c2)
    {
        Color color = new Color();
        if(c1!=this.MySkin.material.color)color = c1;
        else
        {
            float r=Random.Range(0f,1f);
            float g= Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            color = new Color(r, g, b);
        }
        MySkin.material.color = color;
    }


    public void TestFitnes()
    {
        fitnes = (this.transform.position.z-StartPos[0].z)*10;
        //Color color = MySkin.material.color;
        //color.r = 1 - (fitnes) / 255;
        //color.g = (fitnes) / 255;
        //color.b = (fitnes) / 255;
        //MySkin.material.color = color;
    }

    public void Mutate(float chance)
    {
        lastMutation = neuralNetwork.Mutate(chance);
    }
    public void ResetPostion()
    {
        for (int i = 0; i < InputInformation.Length; i++)
        {
            InputInformation[i].transform.position = StartPos[i];
            InputInformation[i].transform.localRotation = StartRot[i];
        }
        fitnes = 0;
    }

    public GameObject prefabHigh;
    public GameObject Highlight;

    private void OnMouseEnter()
    {
        Highlight = Instantiate(prefabHigh);
    }
    private void OnMouseExit()
    {
        Destroy(Highlight);
    }
}
