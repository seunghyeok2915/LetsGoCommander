using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BossAreaCheck : MonoBehaviour
{
    private bool bInRange;

    public UnityEvent onTriggerEnter;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        meshRenderer.material.DOFade(0.6f, 0.9f).SetLoops(-1, LoopType.Yoyo);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !bInRange)
        {
            bInRange = true;
            onTriggerEnter?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
