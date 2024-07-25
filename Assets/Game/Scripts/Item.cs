using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IItem
{
    public static event Action<int> OnItemCollected;
    [SerializeField] protected int ItemID;
    public void Collect()
    {
        OnItemCollected.Invoke(ItemID);
        Destroy(gameObject);
    }
    private void Start() {
        
    }

}
