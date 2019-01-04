﻿using System.Collections.Generic;
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
        [HttpGet]
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

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }

        // POST: Admin/Pages/AddPage
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

        // GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // deklaracja PageVM
            PageVM model;

            using (Database db = new Database())
            {
                // pobieramy stronę z bazy o przekazanym id
                PageDTO dto = db.Pages.Find(id);

                // sprawdzamy czy taka strona istnieje
                if (dto == null)
                {
                    return Content("Strona nie istnieje");
                }

                model = new PageVM(dto);

            }

            return View(model);
        }

        // Post: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Database db = new Database())
            {
                // pobranie id strony
                int id = model.Id;

                // inicjalizacja slug
                string slug = "home";

                // pobranie strony do edycji
                PageDTO dto = db.Pages.Find(id);

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }


                // sprawdzamy unikalność strony, adresu
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona lub adres strony już istnieje");
                }

                //modyfikacja DTO
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.HasSidebar = model.HasSidebar;
                dto.Body = model.Body;

                // zapis edytowanej strony do bazt
                db.SaveChanges();
            }

            // Ustawienie komunikatu Temp Data
            TempData["SM"] = "Wyedytowałeś stronę";

            //Redirect 
            return RedirectToAction("EditPage");
        }


        // GET: Admin/Pages/Details/id
        [HttpGet]
        public ActionResult Details(int id)
        {
            //deklaracja PageVM
            PageVM model;

            using (Database db = new Database())
            {
                // pobranie strony o id
                PageDTO dto = db.Pages.Find(id);

                // sprawdzenie czy strona o takim id istnieje
                if (dto == null)
                {
                    return Content("Strona o podanym id nie istnieje.");
                }
                
                //incjalizacja PageVM
                model = new PageVM(dto);

            }

            return View(model);
        }


        // GET: Admin/Pages/Delete/id
        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (Database db = new Database())
            {
                // pobranie strony z bazy do usunięcia
                PageDTO dto = db.Pages.Find(id);

                // usuwanie wybranej strony z bazy 
                db.Pages.Remove(dto);

                // Zapis zmian

                db.SaveChanges();
            }

            // przekierowanie
            return RedirectToAction("Index");
        }

        // Post: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult ReorderPages(int[] id)
        {
            using (Database db = new Database())
            {
                int count = 1;
                PageDTO dto;

                // sortowanie stron zapis na bazie

                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }


            return View();
        }

    }
}