using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Account;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;


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
        [HttpGet]
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

        // POST: /account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // sprawdzenie model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // sprawdzamy uzytkownika
            bool isValid = false;
            using (Database db = new Database())
            {
                if (db.Users.Any(x => x.UserName.Equals(model.UserName) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "Nieprawidłowa nazwa uzytkownika lub hasło");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
                }

            }

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


        // GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("~/account/login");
        }

        public ActionResult UserNavPartial()
        {
            // pobieramy user name
            string username = User.Identity.Name;

            // deklarujemy model
            UserNavPartialVM model;

            using (Database db = new Database())
            {
                // pobieramy uzytkownika
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == username);

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            return PartialView(model);
        }

        // GET: /account/user-profile
        [ActionName("user-profile")]
        [HttpGet]
        public ActionResult UserProfile()
        {
            //pobieramy nazwę użytkownika
            string username = User.Identity.Name;

            // deklarujemy model
            UserProfileVM model;

            // kontekst
            using (Database db = new Database())
            {
                // pobieramy użytkownika
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == username);

                model = new UserProfileVM(dto);
            }

            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [ActionName("user-profile")]
        [HttpPost]
        public ActionResult UserProfile(UserProfileVM model)
        {
            // sprawdzenie modelstate
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // sprawdzamy hasla
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Hasła nie pasują do siebie!");
                    return View("UserProfile", model);
                }
            }

            // kontekst
            using (Database db = new Database())
            {
                // pobieramy nazwę użytkownika
                string username = User.Identity.Name;

                // sprawdzenie czy nazwa uzytkownika jest unikalna
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.UserName == username))
                {
                    ModelState.AddModelError("", "Nazwa uzytkownika " + model.UserName +  " zajeta");
                    model.UserName = "";
                    return View("UserProfile", model);
                }
                
                // edycja DTO
                UserDTO dto = db.Users.Find(model.Id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.UserName = model.UserName;

                // jeśli haslo nie jest puste lub nie posiada białych znaków
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                // zapis
                db.SaveChanges();
            }

            // ustawienie komunikatu zmiennej TEMP DATA
            TempData["SM"] = "Edytowałeś swój profil";

            // na koniec przekierowanie 
            return Redirect("~/account/user-profile");
        }
    }
}