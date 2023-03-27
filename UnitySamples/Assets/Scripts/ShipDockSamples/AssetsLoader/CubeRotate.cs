using ShipDock;
using UnityEngine;

public class CubeRotate : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        float x = Utils.UnityRangeRandom(0f, 300f);
        float y = Utils.UnityRangeRandom(0f, 300f);
        float z = Utils.UnityRangeRandom(0f, 300f);
        transform.Rotate(x * deltaTime, y * deltaTime, z * deltaTime);
    }
}
