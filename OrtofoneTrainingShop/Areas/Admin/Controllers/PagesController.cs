using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Pages;

namespace OrtofoneTrainingShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Deklaracja listy PageVM
            List<PageVM> pagesList;

           
            using (Database db = new Database())
            {
                // Inicjalizacja  listy
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

            }

            //zwracamy strony do widoku
            return View(pagesList);
        }
    }
}