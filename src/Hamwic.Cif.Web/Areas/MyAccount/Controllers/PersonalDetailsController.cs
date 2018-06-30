using Microsoft.AspNetCore.Mvc;

namespace Hamwic.Cif.Web.Areas.MyAccount.Controllers
{
    [Area("MyAccount")]
    public class PersonalDetailsController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}