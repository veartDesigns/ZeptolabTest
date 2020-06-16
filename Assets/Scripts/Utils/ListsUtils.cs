using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListsUtils
{
    static public List<int> RandomizeList(List<int> lst)
    {

        for (int i = 0; i < lst.Count; i++)
        {
            int temp = lst[i];
            int randomIndex = Random.Range(i, lst.Count);
            lst[i] = lst[randomIndex];
            lst[randomIndex] = temp;
        }
        return lst;
    }
}
