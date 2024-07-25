using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Player player;
    private PlatformEffector2D effector;
    void Awake() {
        effector = GetComponent<PlatformEffector2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player"))
            player = collision.gameObject.GetComponent<Player>();
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        if (player == null)
            return;
        if (player.fallThrought) {
            effector.rotationalOffset = 180;
            player = null;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        effector.rotationalOffset = 0;
    }

}
