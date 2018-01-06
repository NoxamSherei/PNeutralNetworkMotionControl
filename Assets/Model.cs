using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public float fitnes = 0;
    public NeutralNetwork neuralNetwork;
    public Light point1;
    public string Name;
    public SkinnedMeshRenderer MySkin;
    public GameObject[] OutputBone;
    public GameObject[] InputInformation;
    public GameObject[] Points;
    public int test;
    private Vector3 LastPos;
    private Vector3[] StartPos;
    private Quaternion[] StartRot;
    private UIControler Gen;
    public bool active = false;
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
        input = new float[InputInformation.Length + 5 + OutputBone.Length+ Points.Length*3+ InputInformation.Length*3];
        layerShema[0] = input.Length;
        layerShema[layerShema.Length - 1] = OutputBone.Length;
        error= new float[OutputBone.Length];
        neuralNetwork = new NeutralNetwork(layerShema);
        ChoseColor();
        active = true;
    }
    public float[] input;
    public float[] error;
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
            input[input.Length - 1] = position.x - LastPos.x;
            input[input.Length - 2] = position.y - LastPos.y;
            input[input.Length - 3] = position.z - LastPos.z;
            input[input.Length - 4] = rotation.x;
            input[input.Length - 5] = rotation.z;
            TestFitnes();
            LastPos = position;
            //Download Bones Rotate
            for (int i = 0; i < InputInformation.Length; i++)
            {
                input[i] = InputInformation[i].transform.localRotation.y;
            }
            for (int i = 0; i < OutputBone.Length; i++)
            {
                input[i+ InputInformation.Length] = error[i];
            }
            for (int i = 0; i < (Points.Length)*3; i=i+3)
            {

                input[i + InputInformation.Length + OutputBone.Length] = Points[i/3].transform.position.x - this.transform.position.x;
                input[i + InputInformation.Length + OutputBone.Length+1] = Points[i/3].transform.position.z - this.transform.position.z;
                input[i + InputInformation.Length + OutputBone.Length+2] = Points[i/3].transform.position.y - this.transform.position.y;
            }
            for (int i = 0; i < (InputInformation.Length) * 3; i = i + 3)
            {

                input[i + InputInformation.Length + OutputBone.Length+ (Points.Length) * 3] = InputInformation[i/3].transform.position.x - this.transform.position.x;
                input[i + InputInformation.Length + OutputBone.Length + 1+ (Points.Length) * 3] = InputInformation[i/3].transform.position.z - this.transform.position.z;
                input[i + InputInformation.Length + OutputBone.Length + 2+ (Points.Length) * 3] = InputInformation[i/3].transform.position.y - this.transform.position.y;
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
                        error[i] =0;
                    }
                    else
                    {
                        error[i] = 1;
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
                        error[i] = 0;
                    }
                    else
                    {
                        error[i] = -1;
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
    public void ColorMixer(Color c1, Color c2)
    {
        Color color = new Color();
        color = c1*0.75f + c2*0.25f;
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
    public int lastMutation = 0;

    public GameObject prefabHigh;
    public GameObject Highlight;

    private void OnMouseEnter()
    {
        Highlight = Instantiate(prefabHigh, this.transform);
    }
    private void OnMouseExit()
    {
        Destroy(Highlight);
    }
}
