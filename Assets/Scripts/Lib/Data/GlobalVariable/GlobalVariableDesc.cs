using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "GlobalVariableDesc", menuName = "GlobalVariableDesc/GlobalVariableDesc", order = 1)]
public sealed class GlobalVariableDesc : ScriptableObject
{
    public string[] GlobalVariables;
    public int[] InitialValues;
}