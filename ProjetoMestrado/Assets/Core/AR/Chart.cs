using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChartProperties
{
    public string Title;
    public VisualizationTypeEnum VisualizationType;
    public string UnitSymbol;
    public UnitPositionEnum UnitPosition;
    public bool ShowUnit;
    public List<Series> Series;
}

[ExecuteInEditMode]
public class Chart : MonoBehaviour
{
    public ChartProperties Properties;
    private ChartProperties PreviousState;

    private PiechartDrawer PiechartDrawer;

    private void SetPreviousState() 
    {
        if(Properties != null)
        {
            PreviousState = new ChartProperties
            {
                Title = Properties.Title,
                VisualizationType = Properties.VisualizationType,
                UnitSymbol = Properties.UnitSymbol,
                UnitPosition = Properties.UnitPosition,
                ShowUnit = Properties.ShowUnit,
                Series = new List<Series>()
            };

            PreviousState.Series.AddRange(Properties.Series);
        }
    }

    private void Awake()
    {
        Debug.Log("AWAKE!");
    }

    private void Start()
    {
        Debug.Log("HERE");
        SetPreviousState();

        PiechartDrawer = new PiechartDrawer(Properties, PreviousState);
    }

    void Update()
    {
        if(PiechartDrawer == null)
            PiechartDrawer = new PiechartDrawer(Properties, PreviousState);

        Debug.Log("On update");

        //if(Properties != null && PreviousState != null)
        //    PiechartDrawer.SetStates(Properties, PreviousState);

        PiechartDrawer.ListenToPropsChange(transform);

        //switch (VisualizationType)
        //{
        //    case VisualizationTypeEnum.Piechart:

        //    default:
        //        return;
        //}

        SetPreviousState();
    }
}
