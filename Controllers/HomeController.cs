using System.Diagnostics;
using System.Text.Json;
using Fun_Run_Registration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Fun_Run_Registration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly FunRunDBContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, FunRunDBContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        // Action for the homepage
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Action for Race Info page
        [HttpGet]
        public IActionResult RaceInfo()
        {
            return View();
        }

        // Action for the Registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Action for the Waiver page
        [HttpGet]
        public IActionResult Waiver()
        {
            return View();
        }

        // Action for the homepage
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewParticipants()
        {
            var participants = _context.Participants.ToList();
            return View(participants);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var participant = _context.Participants.Find(id);
            if (participant == null)
            {
                return NotFound();
            }

            // Perform the delete operation and save changes
            _context.Participants.Remove(participant);
            _context.SaveChanges();

            // After deletion, redirect back to the list of participants
            return RedirectToAction(nameof(ViewParticipants));
        }

        public IActionResult EditRegistration(int id)
        {
            var participant = _context.Participants.Find(id);
            if (participant == null)
            {
                return NotFound();
            }

            return View(participant); // Pass the participant to the EditRegistration view
        }

        [HttpPost]
        public IActionResult Edit(Participant participant)
        {
            if (ModelState.IsValid)
            {
                // Update the participant in the database
                _context.Update(participant);  // Assuming you're using Entity Framework
                _context.SaveChanges();
                return RedirectToAction("ViewParticipants");
            }
            return View(participant);
        }

        [HttpPost]
        public IActionResult Save(Participant participant, string saveMethod)
        {
            _logger.LogInformation("Received Data: {@Participant}, SaveMethod: {SaveMethod}", participant, saveMethod);

            // Debugging: Log the saveMethod and participant data
            Console.WriteLine($"Save Method: {saveMethod}");
            Console.WriteLine($"Participant Data: {JsonSerializer.Serialize(participant)}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (saveMethod == "traditional")
                    {
                        // Traditional SQL saving logic
                        try
                        {
                            using (var connection = new SqlConnection(_configuration.GetConnectionString("FunRunDBContext")))
                            {
                                connection.Open();
                                var query = @"
                            INSERT INTO Participants (Name, Age, Email, Distance, TShirtSize, DogsName, DogsBreed, DogsAge, Payment)
                            VALUES (@Name, @Age, @Email, @Distance, @TShirtSize, @DogsName, @DogsBreed, @DogsAge, @Payment)";

                                using (var command = new SqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@Name", participant.Name);
                                    command.Parameters.AddWithValue("@Age", participant.Age);
                                    command.Parameters.AddWithValue("@Email", participant.Email);
                                    command.Parameters.AddWithValue("@Distance", participant.Distance);
                                    command.Parameters.AddWithValue("@TShirtSize", participant.TShirtSize);
                                    command.Parameters.AddWithValue("@DogsName", (object)participant.DogsName ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@DogsBreed", (object)participant.DogsBreed ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@DogsAge", (object)participant.DogsAge ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@Payment", participant.Payment);

                                    int rowsAffected = command.ExecuteNonQuery();
                                    Console.WriteLine($"Rows affected: {rowsAffected}"); // Debugging
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in traditional SQL logic: {ex.Message}"); // Debugging
                            _logger.LogError("Error in traditional SQL logic: {Message}", ex.Message);
                            return View("Error");
                        }
                    }
                    else if (saveMethod == "ef")
                    {
                        // Entity Framework saving logic
                        if (participant.Id == 0)
                        {
                            _context.Participants.Add(participant);  // New participant
                        }
                        else
                        {
                            var existingParticipant = _context.Participants.Find(participant.Id);
                            if (existingParticipant != null)
                            {
                                existingParticipant.Name = participant.Name;
                                existingParticipant.Age = participant.Age;
                                existingParticipant.Email = participant.Email;
                                existingParticipant.Distance = participant.Distance;
                                existingParticipant.TShirtSize = participant.TShirtSize;
                                existingParticipant.DogsName = participant.DogsName;
                                existingParticipant.DogsBreed = participant.DogsBreed;
                                existingParticipant.DogsAge = participant.DogsAge;
                                existingParticipant.Payment = participant.Payment;
                            }
                        }
                        _context.SaveChanges();
                    }
                }
                else
                {
                    _logger.LogWarning("ModelState is invalid: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return View("Register", participant);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving participant: {Message}", ex.Message);
                return View("Error");
            }

            return RedirectToAction("ViewParticipants");
        }

        // Default error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
