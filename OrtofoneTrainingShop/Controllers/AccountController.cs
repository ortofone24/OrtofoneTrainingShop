using System;
using System.Collections.Generic;
using System.Linq;
using OrtofoneTrainingShop.Models.Data;
using System.Web;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.ViewModels.Account;


namespace OrtofoneTrainingShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {

            return Redirect("~/account/login");
        }

        // GET: /account/login
        public ActionResult Login()
        {
            // sprawdzanie czy uzytkownik nie jest juz zalogowany
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            // zwracamy widok logowania
            return View();
        }


        // GET: /account/create-account/
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {

            return View("CreateAccount");
        }



        // POST: /account/actionname/
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // sprawdzenie model state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // sprawdzenie hasła
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Hasła nie pasują do siebie");
                return View("CreateAccount", model);
            }

            using (Database db = new Database())
            {
                // sprawdzenie czy nazwa uzytkownika jest unikalna
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", "Nazwa uzytkownika " + model.UserName + " juz jest zajeta");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                // utworzenie uzytkownika
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    UserName = model.UserName,
                    Password = model.Password,
                };


                //dodanie uzytkownika
                db.Users.Add(userDTO);

                //zapis na bazie
                db.SaveChanges();

                // dodanie roli dla uzytkownika
                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = userDTO.Id,
                    RoleId = 2
                };

                //dodanie roli 
                db.UserRoles.Add(userRoleDTO);
                // zapis na bazie
                db.SaveChanges();
            }

            //TempData komunikat
            TempData["SM"] = "Jesteś teraz zarejestrowany i możesz się zalogować!";
        
            
        return Redirect("~/account/login");
        }
        
    }
}