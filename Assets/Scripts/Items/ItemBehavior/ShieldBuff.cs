using UnityEngine;

public class ShieldBuff : MonoBehaviour
{
    public float duration = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the shield buff
        transform.Rotate(new Vector3(0, 100, 0) * Time.deltaTime);
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            Destroy(gameObject);
        }
    }
}
