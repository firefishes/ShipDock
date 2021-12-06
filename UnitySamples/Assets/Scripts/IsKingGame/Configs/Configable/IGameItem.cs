
using LitJson;

namespace IsKing
{
    public interface IGameItem
    {
        int GetID();
        void AutoFill();
        //JsonData ToJSON();
    }
}