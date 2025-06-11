using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private float _reduceSpeed = 2f;
    private float _target = 1;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        if (Mathf.Abs(_healthBarImage.fillAmount - _target) > 0.01f)
        {
            _healthBarImage.fillAmount = Mathf.Lerp(_healthBarImage.fillAmount, _target, Time.deltaTime * _reduceSpeed);
        }
        else
        {
            _healthBarImage.fillAmount = _target; 
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        _target = Mathf.Clamp01(currentHealth / maxHealth);
    }
}
