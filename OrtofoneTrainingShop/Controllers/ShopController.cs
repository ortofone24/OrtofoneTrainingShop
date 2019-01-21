using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Shop;

namespace OrtofoneTrainingShop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // deklarujemy CategoryVM list
            List<CategoryVM> categoryVMList;
           
            // inicjalizacja listy
            using (Database db = new Database())
            {
                categoryVMList = db.Categories.ToArray()
                                              .OrderBy(x => x.Sorting)
                                              .Select(x => new CategoryVM(x))
                                              .ToList();
            }

            // zwracamy partial z listą


            return PartialView(categoryVMList);
        }

        public ActionResult Category(string name)
        {
            // deklaracja produkt list view model productVMList
            List<ProductVM> productVMList;

            //pobieranie produktów z bazy
            using (Database db = new Database())
            {
                // pobranie id kategorii
                CategoryDTO categoryDto = db.Categories
                                            .Where(x => x.Slug == name)
                                            .FirstOrDefault();
                int catId = categoryDto.Id;

                //inicjalizacja produktow productVMList

                productVMList = db.Products
                                  .ToArray()
                                  .Where(x => x.CategoryId == catId)
                                  .Select(x => new ProductVM(x))
                                  .ToList();

                // pobieranie nazwy kategorii aby przekazać do ViewBag
                var productCat = db.Products
                                   .Where(x => x.CategoryId == catId)
                                   .FirstOrDefault();

                ViewBag.CategoryName = productCat.CategoryName;
            }

            //zwracamy widok z listą produktów z danej kategorii
            return View(productVMList);
        }
    }
}