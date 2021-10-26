using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public EnemyBase EnemyBase { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public List<Move> MovesList { get; set; }
    public Enemy(EnemyBase enemyBase, int level)
    {
        EnemyBase = enemyBase;
        Level = level;
        HP = MaxHp;

        MovesList = new List<Move>();
        foreach (var move in EnemyBase.LearnableMoves)
        {
            if (move.Level <= level)
                MovesList.Add(new Move(move.MoveBase));

            if (MovesList.Count >= 4)
                break;
        }
    }

    public int MaxHp => Mathf.FloorToInt(EnemyBase.MaxHp);
    public int Attack => Mathf.FloorToInt(EnemyBase.Attack);
    public bool TakeDamage(Move move, Enemy attacker)
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
