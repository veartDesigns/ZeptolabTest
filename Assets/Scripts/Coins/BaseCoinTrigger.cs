using UnityEngine;
using System.Collections;
using System;
using Zeptolab;

public class BaseCoinTrigger : MonoBehaviour
{
    private float _spawnTime;
    private IEnumerator _coroutine;

    private Renderer _renderer;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _renderer = GetComponent<MeshRenderer>();
        _spawnTime = UnityEngine.Random.Range(5,15);

        GamePlayManager.Instance.OnGameEnd += OnGameEnd;
    }

    private IEnumerator WaitToSpawn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Spawn();
    }
    private void OnGameEnd(bool win)
    {
        if(_collider != null) _collider.enabled = false;
        StopTimer();
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
        if (_renderer != null) _renderer.enabled = false;
       if(_collider != null) _collider.enabled = false;
    }

    public void OnDestroy()
    {
        StopTimer();
        GamePlayManager.Instance.OnGameEnd -= OnGameEnd;
    }

    private void StopTimer()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
