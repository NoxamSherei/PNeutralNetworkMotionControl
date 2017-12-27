using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public float fitnes = 0;
    private NeutralNetwork neuralNetwork;
    public SkinnedMeshRenderer MySkin;
    public GameObject[] OutputBone;
    public GameObject[] InputInformation;
    public int test;
    private Vector3 LastPos;
    private Vector3[] StartPos;
    private Quaternion[] StartRot;

    public void Creation(int[] layerShema)
    {
        LastPos = this.transform.position;
        StartPos = new Vector3[InputInformation.Length];
        StartRot = new Quaternion[InputInformation.Length];
        for (int i = 0; i < InputInformation.Length; i++)
        {
            StartPos[i]= InputInformation[i].transform.position;
            StartRot[i] = InputInformation[i].transform.localRotation;
        }
        layerShema[0] = InputInformation.Length + 2;
        layerShema[layerShema[layerShema.Length - 1]] = OutputBone.Length;
        neuralNetwork = new NeutralNetwork(layerShema);
        TestFitnes(0);
    }

    private void FixedUpdate()
    {
        float[] input = new float[7];
        //Download Position Change
        Vector3 position = InputInformation[4].transform.position;
        input[6] = position.x - LastPos.x;
        input[5] = position.y - LastPos.y;
        input[4] = position.z - LastPos.z;
        TestFitnes(input[4]);
        LastPos = position;
        //Download Bones Rotate
        for (int i = 0; i < InputInformation.Length - 1; i++)
        {
            input[i] = InputInformation[i].transform.localRotation.x;
        }
        //Lunch Neural Network
        float[] output = neuralNetwork.FeedForward(input);
        //Move bones
        for (int i = 0; i < output.Length; i++)
        {
            Quaternion rotate = new Quaternion(0, 0, 0, 0);
            if (output[i] > 0.25)
            {
                rotate = OutputBone[i].transform.localRotation;
                if ((rotate.x - 0.005f) < 0.35)
                {
                    rotate.x = rotate.x - 0.005f;
                    OutputBone[i].transform.localRotation = rotate;
                }
            }
            else if (output[i] < -0.25)
            {

                rotate = OutputBone[i].transform.localRotation;
                if ((rotate.x + 0.005f) < 0.65)
                {
                    rotate.x = rotate.x + 0.005f;
                    OutputBone[i].transform.localRotation = rotate;
                }
            }
        }
    }

    public void TestFitnes(float change)
    {
        fitnes += change;
        Color color = MySkin.material.color;
        color.r = 1 - (fitnes) / 155;
        color.g = (fitnes) / 155;
        color.b = (fitnes) / 155;
        MySkin.material.color = color;
        Invoke("TestFitnes", 0.1f);
    }

    public void Mutate(float chance)
    {
        lastMutation=neuralNetwork.Mutate(chance);
    }
    public void ResetPostion()
    {
        for (int i = 0; i < InputInformation.Length; i++)
        {
             InputInformation[i].transform.position= StartPos[i];
             InputInformation[i].transform.localRotation = StartRot[i];
        }
        fitnes = 0;
    }
    public int lastMutation = 0;
}
