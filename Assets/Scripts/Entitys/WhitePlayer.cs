using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePlayer : MonoBehaviour
{
    public int index = 1;
    private GroupManager groupManager;

    private void Start()
    {
        groupManager = FindObjectOfType<GroupManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.instance.PlaySound(3);

            var effect = PoolManager.GetItem<MagicPoofEffect>();

            var position = transform.position;
            effect.Init(new Vector3(position.x, position.y + 1.5f, position.z), 0.5f);
            groupManager.MakeSoldier(index, transform);
            gameObject.SetActive(false);
        }
    }
}
