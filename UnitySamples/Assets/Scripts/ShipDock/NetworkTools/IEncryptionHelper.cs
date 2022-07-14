namespace ShipDock.Network
{
    public interface IEncryptionHelper
    {
        byte[] Encrypt(byte[] data, string publicKey);
        string Encrypt(string strText, string strPublicKey);
        string Decrypt(string strEntryText, string strPrivateKey);
    }
}