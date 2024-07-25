using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    void Start()
    {
        Item.OnItemCollected += ItemCollected;
    }
    void ItemCollected(int ID) {
        Debug.Log(ID);
    }
    void Update() {
    }
}
