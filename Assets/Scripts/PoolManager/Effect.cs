using UnityEngine;

public class Effect : MonoBehaviour, IPool
{
    float lifeTime = 0;
    public void Init(Vector3 createPos, float lifeTime)
    {
        transform.position = createPos;
        this.lifeTime = lifeTime;
    }

    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
        lifeTime = 0;
    }

    private void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
                SetActiveFalse();
        }
    }
}
