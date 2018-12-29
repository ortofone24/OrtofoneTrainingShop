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
        // GET: Admin/Views/Pages
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

        // GET: Admin/Views/Pages
        [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }

        // POST: Admin/Views/Pages
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Sprawdzanie model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Database db = new Database())
            {
                string slug;

                // Inicjalizacja PageDTO
                PageDTO dto = new PageDTO();

                

                // gdy niemamy adresu strony to przypisujemy tytuł
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ","-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // zapobiegamy dodaniu takiej samej nazwy strony
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten tytuł lub adres strony już istnieje.");
                    return View(model);
                }

                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;

                //zapis DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "Dodałeś nową stronę";

            return RedirectToAction("AddPage");
        }

    }
}