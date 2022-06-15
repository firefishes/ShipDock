using ShipDock.Tools;
using UnityEngine;

public class CubeRotate : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        float x = Utils.UnityRangeRandom(1f, 10f);
        float y = Utils.UnityRangeRandom(1f, 10f);
        float z = Utils.UnityRangeRandom(1f, 10f);
        transform.Rotate(x * deltaTime, y * deltaTime, z * deltaTime);
    }
}
