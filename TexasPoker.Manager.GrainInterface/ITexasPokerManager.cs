using System.Collections.Generic;
using System.Threading.Tasks;

namespace TexasPoker.Manager.GrainInterface
{
    public interface ITexasPokerManager : Orleans.IGrainWithStringKey
    {
        Task<float> GetPumpingRate();

        Task<float> SetPumpingRate(float rate);

        Task<Dictionary<int, int>> GetSeatFee();

        Task<Dictionary<int, int>> SetSeatFee(int desktopId, int fee);
    }
}
