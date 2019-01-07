using OrtofoneTrainingShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.Data;


namespace OrtofoneTrainingShop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //deklaracja listy kategorii do wyświetlenia
            List<CategoryVM> categoryVMList;

            using (Database db = new Database())
            {
                categoryVMList = db.Categories
                                   .ToArray()
                                   .OrderBy(x => x.Sorting)
                                   .Select(x => new CategoryVM(x))
                                   .ToList();
            }

            return View(categoryVMList);
        }


        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Deklaracja id
            string id;

            using (Database db = new Database())
            {
                // Sprawdzenie czy nazwa kategoria jest unikalna
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "tytulzajety";
                }

                // Inicjalizacja dto
                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 1000;

                // Zapis do bazy
                db.Categories.Add(dto);
                db.SaveChanges();

                //Pobieramy id
                id = dto.Id.ToString();
            }
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public ActionResult ReorderCategories(int[] id)
        {
            //db contekst
            using (Database db = new Database())
            {
                // inicjalizacja licznika count
                int count = 1;

                // deklaracja DTO
                CategoryDTO dto;

                // sortowanie kategorii
                foreach (var catId in id)
                {
                    // przypisanie do dto idiki z bazy
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;


                    // zapis na bazie
                    db.SaveChanges();
                    count++;
                }
            }
            
            return View();
        }

        //GET: Admin/Shop/DeleteCategory
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            //kontekst
            using (Database db = new Database())
            {
                //pobieramy kategorię do usunięcia o podanym id
                CategoryDTO dto = db.Categories.Find(id);

                //usuwanie kategorie
                db.Categories.Remove(dto);

                // zapis na bazie
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }

        // POST Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            //kontekst

            using (Database db = new Database())
            {
                // sprawdzamy czy kategoria jest unikalna
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "tytulzajety";
                }

                //pobieramy kategorie 
                CategoryDTO dto = db.Categories.Find(id);

                // edycja kategorii
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //zapis na bazie
                db.SaveChanges();
            }
            return "Ok";
        }
    }
}