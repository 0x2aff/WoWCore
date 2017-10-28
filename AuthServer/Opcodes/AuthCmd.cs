namespace WoWCore.AuthServer.Opcodes
{
    public enum AuthCmd : byte
    {
        LogonChallenge = 0x00,
        LogonProof = 0x01,
        ReconnectChallenge = 0x02,
        ReconnectProof = 0x03,
        Realmlist = 0x10
    }
}
