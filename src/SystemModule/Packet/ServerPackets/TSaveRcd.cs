using SystemModule.Packet.ClientPackets;

namespace SystemModule.Packet.ServerPackets
{
    public class TSaveRcd
    {
        public string sAccount;
        public string sChrName;
        public int nSessionID;
        public object PlayObject;
        public HumDataInfo HumanRcd;
        public int nReTryCount;

        public TSaveRcd()
        {
            HumanRcd = new HumDataInfo();
        }
    }
}