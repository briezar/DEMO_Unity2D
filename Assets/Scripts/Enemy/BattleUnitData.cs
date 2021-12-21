using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitData
{
    public BattleUnitBase BattleUnitBase { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public List<Move> MovesList { get; set; }
    public BattleUnitData(BattleUnitBase battleUnitBase, int level)
    {
        BattleUnitBase = battleUnitBase;
        Level = level;
        HP = MaxHp;

        MovesList = new List<Move>();
        foreach (var move in BattleUnitBase.LearnableMoves)
        {
            if (move.Level <= level)
                MovesList.Add(new Move(move.MoveBase));

            if (MovesList.Count >= 4)
                break;
        }
    }

    public int MaxHp => Mathf.FloorToInt(BattleUnitBase.MaxHp);
    public int Attack => Mathf.FloorToInt(BattleUnitBase.Attack);
    public bool TakeDamage(Move move, BattleUnitData attacker)
    {
        float modifiers = Random.Range(0.85f, 1f) / 50f;
        float dmg = move.MoveBase.Power * (float)attacker.Attack;
        int outputDamage = Mathf.FloorToInt(dmg * modifiers);

        HP -= outputDamage;
        if (HP > MaxHp) HP = MaxHp;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public bool TakeDamage(int damage)
    {
        HP -= damage;
        if (HP > MaxHp) HP = MaxHp;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public void PlayEntrySound()
    {
        if (BattleUnitBase.EntrySound != null) SoundManager.Instance.PlaySFX(BattleUnitBase.EntrySound);
    }
    public void PlayExitSound()
    {
        if (BattleUnitBase.ExitSound != null) SoundManager.Instance.PlaySFX(BattleUnitBase.ExitSound);
    }

    public Move GetRandomMove()
    {
        int randomMove = Random.Range(0, MovesList.Count);
        return MovesList[randomMove];
    }

    public Move GetRandomMove(int startInclusive, int endExclusive)
    {
        int randomMove = Random.Range(startInclusive, endExclusive);
        return MovesList[randomMove];
    }
}
