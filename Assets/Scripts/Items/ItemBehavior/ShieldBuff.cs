using UnityEngine;
//Hauk
public class ShieldBuff : MonoBehaviour
{
    public float duration = 5f;
    
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
