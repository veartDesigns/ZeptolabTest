using UnityEngine;
using System.Collections;
using Zeptolab;

public class SpecialCoinTrigger : BaseCoinTrigger
{
    private float _lifeTime = 10;

    private void Start()
    {
        StartCoroutine(WaitToDestroy(_lifeTime));
    }

    public override void OnTrigger(Collider collider)
    {
        GamePlayManager.Instance.OnGetSpecialCoin();
        base.Hide();
    }

    private IEnumerator WaitToDestroy(float waitTme)
    {
        yield return new WaitForSeconds(waitTme);
        OnDestroy();
        Destroy(this.gameObject);
    }
}