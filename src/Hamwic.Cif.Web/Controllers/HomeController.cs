using Hamwic.Cif.Core.Data;
using Hamwic.Cif.Core.Entities;
using Hamwic.Cif.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Hamwic.Cif.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Private readonly variables

        #endregion

        #region Constructor
        public HomeController()
        {
        }

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
