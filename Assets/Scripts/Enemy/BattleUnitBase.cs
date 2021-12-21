using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Create New Enemy")]
public class BattleUnitBase : ScriptableObject
{
    [SerializeField] private string unitName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;

    //Base Stats
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private AudioClip entrySound;
    [SerializeField] private AudioClip exitSound;


    [SerializeField] List<LearnableMoves> learnableMoves;

    public AudioClip EntrySound => entrySound;
    public AudioClip ExitSound => exitSound;

    public string UnitName => unitName;
    public string Description => description;
    public Sprite FrontSprite => frontSprite;
    public int MaxHp => maxHp;
    public int Attack => attack;
    public List<LearnableMoves> LearnableMoves => learnableMoves;
}

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase => moveBase;
    public int Level => level;
}
