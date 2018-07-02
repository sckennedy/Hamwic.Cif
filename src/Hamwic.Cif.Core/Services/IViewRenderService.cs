using System.Threading.Tasks;

namespace Hamwic.Cif.Core.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}