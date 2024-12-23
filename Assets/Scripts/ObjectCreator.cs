using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public static ObjectCreator instance;

    [Header("Traps")]
    public GameObject fallingPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void CreateObject(GameObject prefabs, Transform target, bool shouldBeDestroyed = false, float delay = 0)
    {
        StartCoroutine(CreateObjectCoroutine(prefabs, target, shouldBeDestroyed, delay));
    }

    private IEnumerator CreateObjectCoroutine(GameObject prefabs, Transform target, bool shouldBeDestroyed, float delay)
    {
        Vector3 newPos = target.position;

        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefabs, newPos, Quaternion.identity);

        if (shouldBeDestroyed)
            Destroy(newObject, 15f);
    }
}
