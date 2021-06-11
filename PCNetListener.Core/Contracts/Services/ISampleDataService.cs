using System.Collections.Generic;
using System.Threading.Tasks;

using PCNetListener.Core.Models;

namespace PCNetListener.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetContentGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetListDetailsDataAsync();
    }
}
