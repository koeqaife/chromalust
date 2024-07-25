using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTexture : MonoBehaviour
{
    public List<GameObject> gameobjects;
    public int spawn_chance = 100;
    private bool _spawned = false;
    public float offestx = 2.5f;
    public float offesty = 2.5f;
    public float scale = 1;
    private Vector3 screen;
    private Vector3 oldscreen;
    private GameObject instance;
    [SerializeField] private int sibling_index = 1;
    [SerializeField] private GameObject canvas;

    private void Start()
    {
        Debug.Log(spawn_chance);

        int spawn = Random.Range(spawn_chance, 101);

        Debug.Log(spawn);
        if (spawn == 100 || spawn == 101)
        {
            _spawned = true;
            int Index = Random.Range(0, gameobjects.Count);
            GameObject random = gameobjects[Index];
            instance = Instantiate(random, transform.position, Quaternion.identity, canvas.transform);

            screen = new Vector3(Screen.width, Screen.height, 0);
            pl();
        }
    }
    private void Update()
    {
        if (_spawned)
        {
            if (instance != null)
            {
                oldscreen = screen;
                screen = new Vector3(Screen.width, Screen.height, 0);
                if (screen != oldscreen)
                {
                    pl();
                }
            }
        }
    }
    private void pl()
    {
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        Vector2 centerPosition = rectTransform.TransformPoint(rectTransform.rect.center);
        instance.transform.SetSiblingIndex(sibling_index);
        instance.transform.localScale = new Vector3(screen.x*scale, screen.x*scale, transform.localScale.z);
        instance.transform.localPosition = new Vector2(instance.transform.localScale.x/offestx, 0-instance.transform.localScale.y/offesty);
    }
}
