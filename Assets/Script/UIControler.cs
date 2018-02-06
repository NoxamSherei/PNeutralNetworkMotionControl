using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour {

    public Text OtherInformation;
    public Text GenerationInformation;
    public Text PopulationInformation;
    public Text PopulationRating;
    public Text TimeText;
    public Text TimeMultiplication;
    public Text Ratting;

    public Slider TMuiltiplicSlider;
    public InputField TMuiltiplicField;
    public Transform LayerLayout;
    public UILayer[] Layers;
    public GameObject PrefabLayer;
    public Text DataText;
    public GameObject DataPanel;

    public Button Load;
    public Button Save;

    void Start() {
        DataPanel.SetActive(false);
        Invoke("FPSChecker", 0.01f);
    }

    private void FPSChecker()
    {
        //informacja o klatkach na sekunde
        OtherInformation.text = "FPS: "+(1 / Time.deltaTime).ToString("f0")+" ";
        Invoke("FPSChecker", 0.1f);
    }
    
    private List<LayerInfoPanel> hiddenLayers=new List<LayerInfoPanel>();
    public LayerInfoPanel PreFab;
    public Transform panel;

    public InputField NumberOfLayers;
    
    public void FillList(int[] layerSchemat)
    {
        int n = 0;
        hiddenLayers.Clear();
        for (int i = 1; i < layerSchemat.Length-1; i++)
        {
            LayerInfoPanel temp = Instantiate(PreFab, panel);
            temp.Information.text = "Layer " + i + ":";
            temp.InField.text = layerSchemat[i].ToString();
            temp.Controler = this;
            hiddenLayers.Add(temp);
            n++;
        }
        SaveNumberOfNeurons();
        NumberOfLayers.text = n.ToString();
    }

    public void ChangeLayerNumber()
    {
        int n;
        int.TryParse(NumberOfLayers.text, out n);
        if (n <= 0) n = 0;
        if (n >= 10) n = 10;
        
        if(n>hiddenLayers.Count)
        {
            for (int i = hiddenLayers.Count; i < n; i++)
            {
                LayerInfoPanel temp = Instantiate(PreFab, panel);
                temp.Information.text = "Layer " + (i+1) + ":";
                temp.InField.text = (i+1).ToString();
                temp.Controler = this;
                hiddenLayers.Add(temp);
            }
        }else if(n < hiddenLayers.Count)
        {
            int j = hiddenLayers.Count - n;
            for (int i = 0 ; i < j; i++)
            {
                Destroy(hiddenLayers[hiddenLayers.Count - 1].gameObject);
                hiddenLayers.Remove(hiddenLayers[hiddenLayers.Count - 1]);
            }
        }
        SaveNumberOfNeurons();
    }
    public GenerationManager connect;
    public Text warning;
    //
    public void SaveNumberOfNeurons()
    {
        int[] schema = new int[hiddenLayers.Count+2];
        for (int i = 0; i < hiddenLayers.Count; i++)
        {
            int.TryParse(hiddenLayers[i].InField.text, out schema[i+1]);
        }
        schema[0] = 28;
        schema[schema.Length - 1] = 8;
        int c=0;
        for (int i = 0; i < schema.Length-1; i++)//
        {
            c += schema[i] * schema[i + 1];
        }
        if ((c * connect.Size) >= 660800)
        {
            warning.text = ">>>Can run Slow!<<<";
        }
        else
        {
            warning.text = "";
        }
        Connections.text = "Connections " + c;
        connect.NeuralSchemat = schema;
    }
    public void DeleteList()
    {
        if(hiddenLayers!=null||hiddenLayers.Count!=0)
        {
            for (int i = 0; i < hiddenLayers.Count; i++)
            {
                Destroy(hiddenLayers[i].gameObject);
                hiddenLayers.Remove(hiddenLayers[i]);
            }
        }
    }
    public Text Connections;
    public InputField Popfield;
    public void PopulationCount()
    {
        int j;
        int.TryParse(Popfield.text, out j);
        if (j > 50) j = 50;
        if (j < 4) j = 4;
        connect.Size = j;
        Popfield.text=j+"";
    }
}
