using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Transform health;
    [SerializeField] private Gradient gradient;

    private Image fillGradient;

    private void Awake()
    {
        fillGradient = health.GetComponent<Image>();
    }

    public void SetHP(float hpNormalized)
    {
        health.localScale = new Vector3(hpNormalized, 1f);
        fillGradient.color = gradient.Evaluate(hpNormalized);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        float currentHP = health.localScale.x;
        bool isHeal = currentHP < newHP;
        float changeAmount = currentHP - newHP;

        while (isHeal ? (currentHP < newHP) : (currentHP > newHP))
        {
            currentHP -= changeAmount * Time.deltaTime;

            health.localScale = new Vector3(currentHP, 1f);
            fillGradient.color = gradient.Evaluate(currentHP);

            yield return null;
        }

        health.localScale = new Vector3(newHP, 1f);
        fillGradient.color = gradient.Evaluate(newHP);

    }

}
