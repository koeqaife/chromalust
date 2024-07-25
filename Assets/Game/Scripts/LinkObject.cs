using UnityEngine;

public class LinkObject : MonoBehaviour
{
    [SerializeField] private Transform LinkTo;
    [SerializeField] private float offestx;
    [SerializeField] private float offesty;
    void Update()
    {
        transform.position = new Vector3(LinkTo.position.x + offestx, LinkTo.position.y + offesty);
    }
}
