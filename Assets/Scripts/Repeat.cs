using UnityEngine;

public class Repeat : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField] private float repeatWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        if (repeatWidth == 0){
            repeatWidth = GetComponent<BoxCollider>().size.x / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < startPos.x - repeatWidth){
            transform.position = startPos;
        }   
    }
}
