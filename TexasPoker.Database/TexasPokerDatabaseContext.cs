using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.Database;
using GF.Database.Adapters;

namespace TexasPoker.Database
{
    [Export]
    public class TexasPokerDatabaseContext : DatabaseContext
    {
        [ImportingConstructor]
        public TexasPokerDatabaseContext(ExportProvider exportProvider, DatabaseContextSettings settings)
            : base(exportProvider, settings)
        {
        }

        /// <summary>
        /// Gets the player.
        /// </summary>
        public ICollectionAdapter<PlayerEntity> Players
        {
            get
            {
                return this.GetAdapter<PlayerEntity>();
            }
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public ICollectionAdapter<BuyChipsEventEntity> BuyChipsEvents { get { return this.GetAdapter<BuyChipsEventEntity>(); } }
        public ICollectionAdapter<BuyCoinsEventEntity> BuyCoinsEvents { get { return this.GetAdapter<BuyCoinsEventEntity>(); } }
        public ICollectionAdapter<BuyVIPEventEntity> BuyVIPEvents { get { return this.GetAdapter<BuyVIPEventEntity>(); } }
        public ICollectionAdapter<ChipBuyGiftEventEntity> ChipBuyGiftEvents { get { return this.GetAdapter<ChipBuyGiftEventEntity>(); } }
        public ICollectionAdapter<ChipBuyItemEventEntity> ChipBuyItemEvents { get { return this.GetAdapter<ChipBuyItemEventEntity>(); } }
        public ICollectionAdapter<ChipsChangeByManagerEventEntity> ChipsChangeByManagerEvents { get { return this.GetAdapter<ChipsChangeByManagerEventEntity>(); } }
        public ICollectionAdapter<CoinBuyGiftEventEntity> CoinBuyGiftEvents { get { return this.GetAdapter<CoinBuyGiftEventEntity>(); } }
        public ICollectionAdapter<CoinBuyItemEventEntity> CoinBuyItemEvents { get { return this.GetAdapter<CoinBuyItemEventEntity>(); } }
        public ICollectionAdapter<DailyGetChipsEventEntity> DailyGetChipsEvents { get { return this.GetAdapter<DailyGetChipsEventEntity>(); } }
        public ICollectionAdapter<LostAllSendChipsEventEntity> LostAllSendChipsEvents { get { return this.GetAdapter<LostAllSendChipsEventEntity>(); } }
        public ICollectionAdapter<PlayerGetChipsFromOtherEventEntity> PlayerGetChipsFromOtherEvents { get { return this.GetAdapter<PlayerGetChipsFromOtherEventEntity>(); } }
        public ICollectionAdapter<PlayerReportEventEntity> PlayerReportEvents { get { return this.GetAdapter<PlayerReportEventEntity>(); } }
        public ICollectionAdapter<PlayerSendOtherChipsEventEntity> PlayerSendOtherChipsEvents { get { return this.GetAdapter<PlayerSendOtherChipsEventEntity>(); } }
        public ICollectionAdapter<TexasPokerEventEntity> TexasPokerEvents { get { return this.GetAdapter<TexasPokerEventEntity>(); } }
    }
}
