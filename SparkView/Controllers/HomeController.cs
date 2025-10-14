using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SparkView.Models;
using System.Diagnostics;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace SparkView.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // SMTP settings
        private readonly string smtpHost = "smtp.sparkviewclubandresorts.in"; // your SMTP host
        private readonly int smtpPort = 587;                                 // 587 for STARTTLS
        private readonly string smtpUser = "info@sparkviewclubandresorts.in"; // your email username
        private readonly string smtpPass = "Spark#321123";             // your email password
        private readonly string fromName = "Spark View Club and Resorts";     // display name
        private readonly string fromEmail = "info@sparkviewclubandresorts.in"; // sending email


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // ------------------ Views ------------------
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Destination()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TermsAndCondition()
        {
            return View();
        }

        public IActionResult MembershipPlan()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult InternationDestination()
        {
            return View();
        }

        public IActionResult DomesticDestination()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MembershipLogin()
        {
            return View();
        }

        // ------------------ Membership Login POST ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MembershipLoginPost(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Email is required";
                return RedirectToAction("MembershipLogin");
            }

            try
            {
                var memberId = $"HC-{DateTime.UtcNow:yyMMdd}-{Random.Shared.Next(1000, 9999)}";
                var tempPassword = GenerateTempPassword(10);

                SendMemberCredentials(email, memberId, tempPassword);

                TempData["Success"] = $"Credentials sent to {email}. Check your inbox.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending membership email");
                TempData["Error"] = "Something went wrong. Please try again later.";
            }

            return RedirectToAction("MembershipLogin");
        }

        // ------------------ Send Email ------------------
        private void SendMemberCredentials(string toEmail, string memberId, string tempPassword)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Your Member ID and Temporary Password";
            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h3>Welcome to Haven Club</h3>
                    <p>Here are your credentials:</p>
                    <ul>
                        <li><strong>Member ID:</strong> {memberId}</li>
                        <li><strong>Temporary Password:</strong> {tempPassword}</li>
                    </ul>
                    <p>Please login and change your password immediately.</p>
                    <p>— Haven Club Team</p>"
            };

            using var smtpClient = new SmtpClient();

            // Connect using STARTTLS (port 587)
            smtpClient.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);

            // Authenticate if credentials provided
            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPass))
            {
                smtpClient.Authenticate(smtpUser, smtpPass);
            }

            smtpClient.Send(message);
            smtpClient.Disconnect(true);
        }

        // ------------------ Generate Temporary Password ------------------
        private static string GenerateTempPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var pwd = new char[length];
            for (int i = 0; i < length; i++)
            {
                pwd[i] = chars[Random.Shared.Next(chars.Length)];
            }
            return new string(pwd);
        }

        // ------------------ Error View ------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
