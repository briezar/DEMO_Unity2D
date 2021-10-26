using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Enemy/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private int power;
    [SerializeField] private int accuracy;

    public string MoveName => moveName;
    public string Description => description;
    public int Power => power;
    public int Accuracy => accuracy;


}
