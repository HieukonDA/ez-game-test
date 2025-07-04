using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private GameObject _damageNumberPrefab;
    private ObjectPool<GameObject> _pool;

    void Start()
    {
        _pool = new ObjectPool<GameObject>(
            () => Instantiate(_damageNumberPrefab, transform),
            10,
            transform
        );
    }

    public void SpawnDamageNumber(Vector3 position, int damage, bool isPlayer)
    {
        GameObject obj = _pool.Get();
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        text.text = damage.ToString();
        text.color = isPlayer ? Color.red : Color.green; // Màu khác nhau cho đánh/nhận
        obj.transform.position = position;
        obj.SetActive(true);
        StartCoroutine(FadeOut(obj));
    }

    private IEnumerator FadeOut(GameObject obj)
    {
        float timer = 0f;
        Vector3 startPos = obj.transform.position;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up, timer / 0.5f);
            yield return null;
        }
        obj.SetActive(false);
        _pool.Return(obj);
    }
}