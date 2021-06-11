using System.Threading.Tasks;

namespace PCNetListener.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle();

        Task HandleAsync();
    }
}
