using System;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float regenDelay = 1.5f;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;

    // Event for UI Updates (later)
    public event Action<float, float> OnStaminaChanged;

    private float _regenTimer;

    private void Awake()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        HandleRegen();
    }

    private void HandleRegen()
    {
        if (_regenTimer > 0)
        {
            _regenTimer -= Time.deltaTime;
            return;
        }

        if (CurrentStamina < maxStamina)
        {
            CurrentStamina = Mathf.Min(CurrentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
        }
    }

    public bool HasStamina(float amount)
    {
        return CurrentStamina >= amount;
    }

    public bool TryConsumeStamina(float amount)
    {
        if (CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            _regenTimer = regenDelay;
            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
            return true;
        }

        Debug.Log(CurrentStamina);

        return false;
    }
}
