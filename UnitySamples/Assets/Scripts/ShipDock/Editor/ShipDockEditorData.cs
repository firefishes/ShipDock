using System.Collections.Generic;
using UnityEditor;

namespace ShipDock.Editors
{
    public class ShipDockEditorData : Singletons<ShipDockEditorData>
    {
        public string platformPath;
        public bool isBuildFromCoordinator;
        public string coordinatorPath;
        public Dictionary<string, string> assetsFromCoordinator;
        public BuildTarget buildPlatform;
        public UnityEngine.Object[] selections;
        public KeyValueList<string, List<ABAssetCreater>> ABCreaterMapper;
    }

}
