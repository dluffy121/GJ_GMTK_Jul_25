using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    float m_health = 100;

    public Action OnHealthZero;

    public float GetHealthVal()
    {
        return m_health;
    }

    public void IncreaseHealth(float a_incHealth)
    {
        m_health += a_incHealth; 
    }

    public void IncreaseHealthMultiplier(float a_incHealthFactor)
    {
        m_health *= a_incHealthFactor;
    }

    public void DecreaseHealth(float a_decHealth)
    {
        m_health -= a_decHealth;
        if (m_health <= 0)
            OnHealthZero?.Invoke();
    }
}
