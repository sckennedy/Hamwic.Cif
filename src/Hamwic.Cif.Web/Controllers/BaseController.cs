using Hamwic.Cif.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hamwic.Cif.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        public ApplicationDbContext DbContext {get; set;}
    }
}