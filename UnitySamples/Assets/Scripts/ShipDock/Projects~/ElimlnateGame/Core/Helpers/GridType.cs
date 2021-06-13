using System;
using UnityEngine;

namespace Elimlnate
{
    public class GridType
    {
        public int gridTypeKey;
        public int mainGridType;
        public int weight;
        public bool isStaticAsset;
        public GameObject gridRes;
        public Func<GameObject> creater;
    }
}
