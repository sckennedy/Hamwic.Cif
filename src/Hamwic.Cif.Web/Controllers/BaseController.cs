using Hamwic.Cif.Core.Data;
using Hamwic.Core.Commands;
using Hamwic.Core.Events;
using Microsoft.AspNetCore.Mvc;

namespace Hamwic.Cif.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        public ApplicationDbContext DbContext {get; set;}
        public ICommandProcessor CommandProcessor { get; set; }
        public IEventDispatcher EventDispatcher { get; set; }
    }
}