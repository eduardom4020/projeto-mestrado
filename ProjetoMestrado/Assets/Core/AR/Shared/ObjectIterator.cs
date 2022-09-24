using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectIterator
{
    public static GameObject GetChildByNameAndLayer(string name, int layerNum, Transform currTransform)
    {
        GameObject iterationResult = null;

        foreach (Transform child in currTransform)
        {
            if (child.gameObject.name == name && child.gameObject.layer == layerNum)
            {
                iterationResult = child.gameObject;
            }
            else
            {
                var childIterationResult = GetChildByNameAndLayer(name, layerNum, child);

                if (childIterationResult != null)
                    iterationResult = childIterationResult;
            }
        }

        return iterationResult;
    }

    public static List<GameObject> GetChildsByNameSimilaityAndLayer(string name, int layerNum, Transform currTransform)
    {
        List<GameObject> iterationResults = new List<GameObject>();

        foreach (Transform child in currTransform)
        {
            if (child.gameObject.name.Contains(name) && child.gameObject.layer == layerNum)
            {
                iterationResults.Add(child.gameObject);
            }
            else
            {
                var childIterationResults = GetChildsByNameSimilaityAndLayer(name, layerNum, child);

                if (childIterationResults != null)
                    iterationResults.AddRange(childIterationResults);
            }
        }

        return iterationResults;
    }
}
