using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;

namespace HangfireSample.Controllers
{
    public class UserController : Controller
    {
        private readonly WelcomeEmailService _welcomeEmailService;

        public UserController(WelcomeEmailService welcomeEmailService)
        {
            _welcomeEmailService = welcomeEmailService;
        }

        [Obsolete]
        public IActionResult Register(string email)
        {
            // Kullanıcının kaydını yapma işlemi burada gerçekleştirilecek

            // Kaydolma işleminin ardından hoş geldin e-postasını 1 saat sonra göndermek için Hangfire'a planlama yapma
            var delay = TimeSpan.FromHours(1);
            BackgroundJob.Schedule(() => _welcomeEmailService.SendWelcomeEmail(email), delay);

            
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Her saat başı çalışan görev"), Cron.Hourly);

            //// Bir işin tamamlanmasından belirli bir süre sonra çalışacak bir görev
            //var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("İş tamamlandı."));
            //var delay = TimeSpan.FromHours(1);
            //BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("1 saat sonra çalışan görev"), delay);

            //// Bir kuyruğa görev ekleme
            //BackgroundJob.Enqueue(() => Console.WriteLine("Kuyruklu görev"));

            //// Bir kuyruğu temizleme
            //BackgroundJob.Enqueue(() => Console.WriteLine("Kuyruğu temizleme"), TimeSpan.FromHours(1));


            return View();
        }
    }
}
