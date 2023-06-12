
## Hangfire ile Arka Plan İşleri Uygulaması
Bu dokümantasyon, Hangfire'ı kullanarak basit arka plan işlerinin nasıl oluşturulduğunu anlatmaktadır.

İçindekiler
1.Hangfire Nedir
2.Hangfire Kurulumu
3.Arka Plan İşleri Senaryoları
  -Senaryo 1: Hoş Geldin E-postasının Gönderilmesi
  -2.Senaryo 2: Günlük Görev
  -Senaryo 3: İş Tamamlandıktan Sonra Görev
  -Senaryo 4: Kuyruğa Görev Ekleme
4.Sonuç


## Hangfire Nedir

ASP.NET Core Hangfire, .NET tabanlı bir açık kaynaklı arka plan görev yöneticisi ve iş sırası (job queue) kütüphanesidir. Hangfire, ASP.NET Core uygulamalarında tekrarlayan, zamanlanmış veya arka planda çalışması gereken görevleri kolayca yönetmek için kullanılır.

Hangfire, kullanıcı dostu bir arabirim sağlar ve iş sırasında aksaklıklar yaşansa bile görevleri güvenli bir şekilde gerçekleştirir. Hangfire, iş sırası işlerini bir veritabanında depolar ve bu işleri çalıştırmak için işçi süreçleri oluşturur. Bu sayede, uygulamanızın temel işlevselliği etkilenmeden arka planda işlerin gerçekleştirilmesi sağlanır.

ASP.NET Core Hangfire kullanarak aşağıdaki gibi işlemleri gerçekleştirebilirsiniz:

Görevleri zamanlayabilir ve belirli aralıklarla çalışmasını sağlayabilirsiniz.
Uzun süren işlemleri arka planda çalıştırabilir ve kullanıcının beklemesini önleyebilirsiniz.
Görevleri tekrarlayabilir ve belirli periyotlarda çalışmasını sağlayabilirsiniz.
Paralel iş sıraları oluşturabilir ve birden fazla işçi süreciyle çalışmasını sağlayabilirsiniz.
İş sırası durumunu takip edebilir ve gerçekleştirilen işlerin başarılı veya başarısız olduğunu izleyebilirsiniz.
Hangfire, ASP.NET Core ile kolayca entegre olabilen bir yapıya sahiptir ve NuGet paketi olarak kullanılabilir. Kullanıcı dostu API'leri ve zengin özellikleri sayesinde, arka planda çalışan görevleri yönetmek için güçlü bir çözüm sunar.

## Hangfire Kurulumu

Hangfire'ı projede kullanabilmek için aşağıdaki adımları izleyin:

1) .csproj dosyanızın <Project> elementinin içine aşağıdaki paket bağımlılıklarını ekleyin:
```xml
  <ItemGroup>
    <PackageReference Include="Hangfire.AspNetCore" Version="1.7.0" />
    <PackageReference Include="Hangfire.SqlServer" Version="1.7.0" />
  </ItemGroup>
```
  
  2) appsettings.json dosyasına aşağıdaki bağlantı dizesini ekleyin:
  ```json
  "ConnectionStrings": {
  "hangfireConnectionString": "Server=.;Database=HangfireSampleDB;Trusted_Connection=True;Integrated Security=SSPI;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
  ```
  
  3) Program.cs dosyasına aşağıdaki kodu ekleyin:
  ```csharp
  using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;

// ...

public void ConfigureServices(IServiceCollection services)
{
    // Hangfire'ı yapılandırın
    builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfireConnectionString")));
    
    // Hangfire sunucusunu ekleyin
    builder.Services.AddHangfireServer();
  

    // Diğer servislerin eklenmesi
    services.AddScoped<WelcomeEmailService>();

    // ...
}


    // Hangfire gösterge panelini kullanmak için
    app.UseHangfireDashboard();

    // Hangfire sunucusunu başlatın
    app.UseHangfireServer(new BackgroundJobServerOptions());

    // ...


 ```
  Hangfire'ı başarıyla projeye ekledikten sonra, arka plan işlerini kullanmaya başlayabilirsiniz.
  
##  Arka Plan İşleri Senaryoları
Bu bölümde, Hangfire kullanarak gerçekleştirebileceğiniz farklı arka plan işleri senaryolarını anlatacağım.
  
 ## Senaryo 1: Hoş Geldin E-postasının Gönderilmesi
Bu senaryoda, kullanıcının kaydını yaptıktan sonra hoş geldin e-postasının belirli bir süre sonra gönderilmesi işlemi gerçekleştirilmektedir.
  
1)  WelcomeEmailService.cs dosyasına aşağıdaki kodu ekleyin:
  ```csharp
  using System;

public class WelcomeEmailService
{
    public void SendWelcomeEmail(string email)
    {
        Console.WriteLine($"Hoş geldin e-postası gönderiliyor: {email}");
        // E-posta gönderme işlemleri burada gerçekleştirilebilir
    }
}


// ...
  ```
  2) UserController.cs dosyasına aşağıdaki kodu ekleyin:
  ```csharp
using Hangfire;
using Microsoft.AspNetCore.Mvc;

// ...
public class UserController : Controller
{
    private readonly WelcomeEmailService _welcomeEmailService;

    public UserController(WelcomeEmailService welcomeEmailService)
    {
        _welcomeEmailService = welcomeEmailService;
    }

    public IActionResult Register(string email)
    {
        // Kullanıcının kaydını yapma işlemi burada gerçekleştirilecek

        // Kaydolma işleminin ardından hoş geldin e-postasını 1 saat sonra göndermek için Hangfire'a planlama yapma
        var delay = TimeSpan.FromHours(1);
        BackgroundJob.Schedule(() => _welcomeEmailService.SendWelcomeEmail(email), delay);

        return View();
    }
}
```
 Kullanıcı kaydı yapıldıktan sonra, hoş geldin e-postası planlanan bir iş olarak Hangfire tarafından 1 saat sonra yürütülecektir.
  
 ## Senaryo 2: Günlük Görev
Bu senaryoda, her gün belirli bir saatte çalışan bir görevin oluşturulması işlemi gerçekleştirilmektedir.
 1) Program.cs dosyasına aşağıdaki kodu ekleyin:
  ```chasrp
  using Hangfire;

// ...

public void ConfigureServices(IServiceCollection services)
{
    // ...

    // Saatlik görevi ayarlayın
    RecurringJob.AddOrUpdate(() => Console.WriteLine("Her saat başı çalışan görev"), Cron.Hourly);

    // ...
}

  ```
##  Senaryo 3: İş Tamamlandıktan Sonra Görev
Bu senaryoda, bir işin tamamlandıktan belirli bir süre sonra çalışan bir görevin oluşturulması işlemi gerçekleştirilmektedir.
  
  1) UserController.cs dosyasındaki Register metoduna aşağıdaki kodu ekleyin:

  ```chasrp
  using Hangfire;

// ...

public IActionResult Register(string email)
{
    // ...

    // İş tamamlandıktan 1 saat sonra çalışacak bir görev
    var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("İş tamamlandı."));
    var delay = TimeSpan.FromHours(1);
    BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("1 saat sonra çalışan görev"), delay);

    return View();
}

  ```
  
  Kullanıcı kaydı işlemi tamamlandıktan sonra "İş tamamlandı." yazısı konsola yazdırılacak ve 1 saat sonra da "1 saat sonra çalışan görev" yazısı konsola yazdırılacaktır.
  
##  Senaryo 4: Kuyruğa Görev Ekleme
Bu senaryoda, bir kuyruğa görevin eklenmesi işlemi gerçekleştirilmektedir.
  1) UserController.cs dosyasındaki Register metoduna aşağıdaki kodu ekleyin:
```chasrp
using Hangfire;

// ...

public IActionResult Register(string email)
{
    // ...

    // Kuyruğa görev ekleme
    BackgroundJob.Enqueue(() => Console.WriteLine("Kuyruklu görev"));

    return View();
}

 ```
  Kuyruğa eklenen görev, Hangfire tarafından sıraya alınacak ve uygun zamanda çalıştırılacaktır.
  
 ## Sonuç
Bu dokümantasyon, Hangfire'ı kullanarak basit bir arka plan işleri uygulamasının nasıl oluşturulduğunu anlatmaktadır. Senaryoları takip ederek Hangfire'ın farklı işlemleri nasıl gerçekleştirebildiğini görebilirsiniz. 
