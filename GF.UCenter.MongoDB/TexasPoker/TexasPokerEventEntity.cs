namespace GF.UCenter.MongoDB.TexasPoker
{
    using System.Collections.Generic;
    using Attributes;
    using Entity;

    public class FirstCard
    {
        public string CardType { get; set; }
        public string CardSuit { get; set; }
    }

    public class SecondCard
    {
        public string CardType { get; set; }
        public string CardSuit { get; set; }
    }

    public class PlayerCommonInfo
    {
        public string PlayerEtGuid { get; set; }
        public int SeatIndex { get; set; }
        public FirstCard FirstCard { get; set; }
        public SecondCard SecondCard { get; set; }
    }

    public class PreflopInfo
    {
        public string PlayerEtGuid { get; set; }
        public string PlayerOperate { get; set; }
        public int OperateChips { get; set; }
    }

    public class EndInfo
    {
        public string PlayerEtGuid { get; set; }
        public int BeforeSysGetWinChips { get; set; }
    }

    [CollectionName("TexasPokerEvent")]
    public class TexasPokerEventEntity : EntityBase
    {
        //public string _id { get; set; }
        public string CardGameType { get; set; }
        public double SysPumpingPersent { get; set; }
        public int SeatNum { get; set; }
        public string GameSpeed { get; set; }
        public int DesktopTbId { get; set; }
        public int MegaBetNum { get; set; }
        public string DesktopEtGuid { get; set; }
        public List<PlayerCommonInfo> PlayerCommonInfo { get; set; }
        public List<PreflopInfo> PreflopInfo { get; set; }
        public object FlopInfo { get; set; }
        public object TurnInfo { get; set; }
        public object RiverInfo { get; set; }
        public object ShowDownInfo { get; set; }
        public List<EndInfo> EndInfo { get; set; }
        public string EventType { get; set; }
        public string EventTm { get; set; }
    }
}
