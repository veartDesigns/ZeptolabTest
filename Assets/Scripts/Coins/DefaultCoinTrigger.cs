using UnityEngine;
using System.Collections;
using Zeptolab;

public class DefaultCoinTrigger : BaseCoinTrigger
{
    public override void OnTrigger(Collider collider)
    {
        GamePlayManager.Instance.OnGetCoin();
        base.WaitAndRespawn();
    }
}
