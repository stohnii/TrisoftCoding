using System.Threading.Tasks;

namespace TrisoftCoding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IDataUpdater _updater = new DataUpdater();
            await _updater.UpdateFiles();
        }
    }
}
