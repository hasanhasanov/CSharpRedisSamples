using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedisClientSample.Controllers
{
    public class AnswerController : Controller
    {
        public IRepository RedisRepo { get; set; }
        //
        // GET: /Answer/

        public ActionResult Index()
        {
            return View();
        }

        // TODO
    }
}
