using MemoryPack;
using System.Collections.Generic;
using SystemModule.Data;

namespace SystemModule.Packets.ServerPackets
{
    [MemoryPackable]
    public partial struct MarketDataMessage
    {
        public IList<MarketItem> List { get; set; }
        public int TotalCount { get; set; }
    }

    [MemoryPackable]
    public partial class MarketSaveDataItem
    {
        [MemoryPackAllowSerialize]
        public MarketItem Item { get; set; }
        public byte ServerIndex { get; set; }
        public string ServerName { get; set; }
        public byte GroupId { get; set; }
    }

    [MemoryPackable]
    public partial struct MarketRegisterMessage
    {
        public byte ServerIndex { get; set; }
        public string ServerName { get; set; }
        public byte GroupId { get; set; }
    }
}