using UnityEngine;
using System.Collections;
using System;

public class BaseCoinTrigger : MonoBehaviour
{
    private float _spawnTime = 5f;
    private IEnumerator _coroutine;

    private Renderer _renderer;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();
    }

    private IEnumerator WaitToSpawn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Spawn();
    }

    private void Spawn()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger(other);
    }

    public virtual void OnTrigger(Collider other)
    {
    }

    public void WaitAndRespawn()
    {
        Hide();
        _coroutine = WaitToSpawn(_spawnTime);
        StartCoroutine(_coroutine);
    }

    public void Hide()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
