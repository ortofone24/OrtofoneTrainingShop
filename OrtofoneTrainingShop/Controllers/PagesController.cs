using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Pages;

namespace OrtofoneTrainingShop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{pages}
        public ActionResult Index(string page = "")
        {
            // ustawiamy adres naszej strony
            if (page == "")
            {
                page = "home";
            }
            
            // deklarujemy pageVM i pageDTO
            PageVM model;
            PageDTO dto;

            // sprawdzamy czy strona istnieje
            using (Database db = new Database())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new {page = ""});
                }
            }

            // pobieramy pageDTO
            using (Database db = new Database())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // ustawiamy tytuł naszej strony
            ViewBag.PageTitle = dto.Title;

            // sprawdzamy czy strona ma pasek boczny z użyciem thernary operator
            ViewBag.Sidebar = dto.HasSidebar == true ? "Tak" : "Nie";

            
            // inicjalizujemy model naszego widoku pageVM
            model = new PageVM(dto);

            // zwracamy widok z modelem pageVM
            return View(model);
        }
    }
}