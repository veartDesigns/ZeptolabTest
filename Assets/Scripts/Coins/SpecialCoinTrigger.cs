using UnityEngine;
using System.Collections;
using Zeptolab;

public class SpecialCoinTrigger : BaseCoinTrigger
{
    public override void OnTrigger(Collider collider)
    {
        GamePlayManager.Instance.OnGetSpecialCoin();
        base.Hide();
    }
}