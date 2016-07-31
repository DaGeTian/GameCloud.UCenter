using GF.MongoDB.Common;
using GF.MongoDB.Common.Attributes;

namespace TexasPoker.Entity
{
    public class DefActor
    {
        public string IsBot { get; set; }
        public string IsChipTrader { get; set; }
        public string AccountId { get; set; }
        public string ActorId { get; set; }
        public string NickName { get; set; }
        public string IpAddress { get; set; }
        public string Gender { get; set; }
        public string IndividualSignature { get; set; }
        public string Level { get; set; }
        public string Experience { get; set; }
        public string ChipAcc { get; set; }
        public string ChipBank { get; set; }
        public string Gold { get; set; }
        public string ProfileSkinTableId { get; set; }
        public string IsVIP { get; set; }
        public string VIPDataTime { get; set; }
        public string VIPPoint { get; set; }
        public string GameTotal { get; set; }
        public string GameWin { get; set; }
        public string JoinDataTime { get; set; }
        public string LoginDateTime { get; set; }
        public string LogoutDateTime { get; set; }
        public string ListItemData { get; set; }
        public string SendChipsTm { get; set; }
        public string Icon { get; set; }
    }

    public class DefBag
    {
        public string MapItemData { get; set; }
        public string SlotCount { get; set; }
        public string ItemValue { get; set; }
    }

    public class DefPlayer
    {
        public string GiveChipTotal { get; set; }
        public string GiveChipDateTime { get; set; }
        public string FirstLoginDateTime { get; set; }
    }

    public class DefPlayerMailBox
    {
        public string ListMailData { get; set; }
        public string ListSystemEvent { get; set; }
    }

    public class DefPlayerFriend
    {
        public string MapFriend { get; set; }
        public string MapMsgRecord { get; set; }
    }

    public class DefPlayerTask
    {
        public string FirstRun { get; set; }
        public string ListTaskData { get; set; }
    }

    public class MapComponent
    {
        public DefActor DefActor { get; set; }
        public DefBag DefBag { get; set; }
        public DefPlayer DefPlayer { get; set; }
        public DefPlayerMailBox DefPlayerMailBox { get; set; }
        public DefPlayerFriend DefPlayerFriend { get; set; }
        public DefPlayerTask DefPlayerTask { get; set; }
        public object DefPlayerTrade { get; set; }
        public object DefPlayerRanking { get; set; }
    }

    [CollectionName("EtPlayer")]
    public class PlayerEntity : EntityBase
    {
        public object entity_children { get; set; }
        public string entity_guid { get; set; }
        public string entity_type { get; set; }
        public MapComponent map_component { get; set; }
    }
}
