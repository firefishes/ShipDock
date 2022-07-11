using ShipDock.Config;

namespace StaticConfig
{
    public partial class PeaceOfficer
    {
        public int[] GetRolePropertyValues()
        {
            int[] result = DataParser.ParseParamToInts(ref roleProperties);
            return result;
        }
    }
}