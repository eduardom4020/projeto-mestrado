using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public GameObject PieChart;

    // Start is called before the first frame update
    void Start()
    {
        var instantiatedPieChart = Instantiate(PieChart, this.transform);
        instantiatedPieChart.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        //instantiatedPieChart.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
