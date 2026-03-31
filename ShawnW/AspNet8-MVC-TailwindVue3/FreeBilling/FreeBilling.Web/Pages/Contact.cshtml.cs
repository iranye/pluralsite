using FreeBilling.Web.Models;
using FreeBilling.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreeBilling.Web.Pages;

public class ContactModel : PageModel
{
    private readonly IEmailService emailService;

    public string Title { get; set; } = "Contact Me";
    public string Message { get; set; } = "";

    public ContactModel(IEmailService emailService)
    {
        this.emailService = emailService;
    }

    [BindProperty]
    public ContactViewModel Contact { get; set; } = new();

    public void OnGet()
    {
    }

    private void ClearData()
    {
        Contact = new ContactViewModel();
        ModelState.Clear();
    }

    public void OnPost()
    {
        if (ModelState.IsValid)
        {
            var fromEmail = "bob@aol.com";
            emailService.SendMail(fromEmail, Contact.Email, Contact.Subject, Contact.Body);

            ClearData();
            Message = "Thank you.  Your Message Will Be Sent";
        }
        else
        {
            Message = "Please fix errors";
        }
    }
}
