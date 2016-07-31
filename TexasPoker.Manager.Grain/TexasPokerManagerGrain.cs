using System.Collections.Generic;
using System.Threading.Tasks;
using TexasPoker.Manager.GrainInterface;

namespace TexasPoker.Manager.Grain
{
    public class TexasPokerManagerGrain : Orleans.Grain, ITexasPokerManager
    {
        float rate = 0.0f;
        Dictionary<int, int> seatFees = new Dictionary<int, int>()
        {
            { 1,10 },
            { 2,20 },
            { 3,20 },
            { 4,10 },
        };

        Task<float> ITexasPokerManager.GetPumpingRate()
        {
            return Task.FromResult(rate);
        }

        Task<float> ITexasPokerManager.SetPumpingRate(float rate)
        {
            this.rate = rate;
            return Task.FromResult(this.rate);
        }

        Task<Dictionary<int, int>> ITexasPokerManager.GetSeatFee()
        {
            return Task.FromResult(this.seatFees);
        }

        Task<Dictionary<int, int>> ITexasPokerManager.SetSeatFee(int desktopId, int fee)
        {
            this.seatFees[desktopId] = fee;
            return Task.FromResult(this.seatFees);
        }
    }
}
