using Hamwic.Cif.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hamwic.Cif.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Private readonly variables

        
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [Route("create")]
        public async Task<IActionResult> CreateUserAsync()
        {
            await Task.Delay(0);
            return Content("User was created", "text/html");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
