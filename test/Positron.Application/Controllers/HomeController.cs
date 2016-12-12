using Microsoft.AspNetCore.Mvc;
using Positron.Application.Models;

namespace Positron.Application.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult TestId(string id)
        {
            return Json(new TestModel
            {
                Value = id
            });
        }

        public IActionResult TestAjax([FromBody] TestModel model)
        {
            return Json(model);
        }

        public IActionResult FormTest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FormTest(TestModel model)
        {
            ViewData["Message"] = $"Value = '{model.Value}', Value2 = '{model.Value2}'";

            return View(model);
        }
    }
}
