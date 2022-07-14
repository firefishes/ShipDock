
using UnityEngine.Networking;

namespace ShipDock.Network
{
    public class HTTPCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }
}