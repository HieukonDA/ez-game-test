using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private float _reduceSpeed = 2f;
    private float target = 1;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        _healthBarImage.fillAmount = Mathf.Lerp(_healthBarImage.fillAmount, target, Time.deltaTime * _reduceSpeed);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        _healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
