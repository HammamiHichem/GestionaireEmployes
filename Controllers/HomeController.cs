using Microsoft.AspNetCore.Mvc;
using GestionaireEmployes.Models;
using Microsoft.AspNetCore.Identity;
using GestionaireEmployes.ViewModels;
using Microsoft.EntityFrameworkCore;
using GestionaireEmployes.Data;
using Microsoft.AspNetCore.SignalR;

namespace GestionaireEmployes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("EmployeeList");
                }

                ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

       public async Task<IActionResult> EmployeeList(string searchString)
{
    // Si une chaîne de recherche est fournie
    if (!string.IsNullOrEmpty(searchString))
    {
        // Recherche des employés dont le nom ou le poste correspond à la recherche
        var matchedEmployees = await _context.Employees
            .Where(e => e.FullName.Contains(searchString) || e.Position.Contains(searchString))
            .ToListAsync();

        // Si un seul employé est trouvé, redirige vers la page de détails
        if (matchedEmployees.Count == 1)
        {
            return RedirectToAction("EmployeeDetails", new { id = matchedEmployees.First().EmployeeId });
        }

        // Si aucun employé n'est trouvé
        if (!matchedEmployees.Any())
        {
            ViewBag.Message = "Aucun employé ne correspond à votre recherche.";
        }

        // Retourne la liste filtrée d'employés
        return View(matchedEmployees);
    }

    // Si aucune recherche, retourne tous les employés
    var allEmployees = await _context.Employees.ToListAsync();
    return View(allEmployees);
}



        public IActionResult AddEmployee()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync(); // Appel asynchrone.
                return RedirectToAction("EmployeeList");
            }
            return View(employee);
        }

                public async Task<IActionResult> EmployeeDetails(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }


        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
public async Task<IActionResult> EditEmployee(Employee employee)
{
    if (ModelState.IsValid)
    {
        _context.Update(employee);
        await _context.SaveChangesAsync();

        // Ajouter un message de succès avec Toastify
        TempData["SuccessMessage"] = "Employé modifié avec succès !";
        return RedirectToAction("EmployeeList");
    }

    return View(employee);
}


        [HttpPost]
public async Task<IActionResult> DeleteEmployee(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null) return NotFound();

    _context.Employees.Remove(employee);
    await _context.SaveChangesAsync();

    // Ajouter un message de succès avec Toastify
    TempData["SuccessMessage"] = "Employé supprimé avec succès !";
    return RedirectToAction("EmployeeList");
}


        public async Task<IActionResult> GetEmployeeDetails(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null)
    {
        return NotFound();
    }

    // Renvoyer la vue avec les détails de l'employé
    return View("EmployeeDetails", employee);
}


        [HttpGet]
public async Task<IActionResult> _PrintEmployeeDetails(int id)
{
    // Assurez-vous de bien récupérer les informations de l'employé dans la base de données
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null)
    {
        return NotFound();
    }

    // Retourner une vue partielle (HTML) avec les données de l'employé
    return PartialView("_PrintEmployeeDetails", employee);
}
public IActionResult StatisticsPage()
{
    // Filtrer les employés qui sont encore en cours (IsCurrentEmployee == true)
    var totalEmployees = _context.Employees.Count(e => e.IsCurrentEmployee); 
    var cdiEmployees = _context.Employees.Count(e => e.ContractType == "CDI" && e.IsCurrentEmployee);
    var cddEmployees = _context.Employees.Count(e => e.ContractType == "CDD" && e.IsCurrentEmployee);

    // Récupérer toutes les positions et leur fréquence parmi les employés en cours
    var positionsStatistics = _context.Employees
        .Where(e => e.Position != null && e.IsCurrentEmployee)  // Filtrer uniquement les employés en cours
        .GroupBy(e => e.Position)
        .Select(g => new
        {
            Position = g.Key,   // Utilisation de Position au lieu de JobTitle
            Count = g.Count()
        })
        .ToList();

    // Créer un objet pour transmettre les données à la vue
    var statistics = new
    {
        TotalEmployees = totalEmployees,
        CdiEmployees = cdiEmployees,
        CddEmployees = cddEmployees,
        Positions = positionsStatistics // Renommé pour correspondre à la vue
    };

    return View(statistics); // Passer les statistiques à la vue
}

public IActionResult Chat()
    {
        return View();
    }
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }
 
    public async Task SendMessage(string user, string message)
    {
        var chatMessage = new ChatMessage
        {
            User = user,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        // Diffuser le message à tous les clients connectés
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


    }
}
 }
