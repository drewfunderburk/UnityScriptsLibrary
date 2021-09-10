using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 1;
    [SerializeField] private bool _invincible = false;
    [SerializeField] private bool _destroyOnDeath = false;
    [Space]
    [SerializeField] private bool _allowInvincibilityFrames = true;
    [SerializeField] private float _invincibilityFramesDuration = 1f;

    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;

    private float _health = 1;
    private MeshRenderer _meshRenderer;

    private float _invincibilityTimer = 0f;

    public float Health { get => _health; private set => _health = value; }
    public bool DestroyOnDeath { get => _destroyOnDeath; private set => _destroyOnDeath = value; }
    public bool Invincible { get => _invincible; set => _invincible = value; }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // Don't bother if invincible
        if (Invincible)
            return;

        if (damage > 0)
        {
            OnTakeDamage.Invoke();
            Health -= damage;

            if (_allowInvincibilityFrames)
                StartCoroutine(IFramesCoroutine());
        }

        if (Health <= 0)
        {
            Health = 0;

            OnDeath.Invoke();
            if (DestroyOnDeath)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    private IEnumerator IFramesCoroutine()
    {
        // Set invincible
        Invincible = true;

        // While timer is not expired
        while (_invincibilityTimer < _invincibilityFramesDuration)
        {
            // Increment timer
            _invincibilityTimer += Time.deltaTime;

            // Toggle the mesh renderer to achieve flashing effect
            if (_meshRenderer)
                _meshRenderer.enabled = !_meshRenderer.enabled;

            // Wait for next frame
            yield return null;
        }

        // Set not invincible
        Invincible = false;

        // Turn mesh renderer on
        _meshRenderer.enabled = true;

        // Reset timer
        _invincibilityTimer = 0f;
    }
}
