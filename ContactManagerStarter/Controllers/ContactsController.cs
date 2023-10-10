using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Data;
using ContactManager.Hubs;
using ContactManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;

namespace ContactManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<ContactHub> _hubContext;

        public ContactsController(ApplicationContext context, IHubContext<ContactHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var contactToDelete = await _context.Contacts
                .Include(x => x.EmailAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (contactToDelete == null)
            {
                return BadRequest();
            }

            _context.EmailAddresses.RemoveRange(contactToDelete.EmailAddresses);
            _context.Contacts.Remove(contactToDelete);

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("Update");

            return Ok();
        }

        public async Task<IActionResult> EditContact(Guid id)
        {
            var contact = await _context.Contacts
                .Include(x => x.EmailAddresses)
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            var viewModel = new EditContactViewModel
            {
                Id = contact.Id,
                Title = contact.Title,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                DOB = contact.DOB,
                PrimaryEmail = contact.PrimaryEmail,
                EmailAddresses = contact.EmailAddresses,
                Addresses = contact.Addresses
            };

            return PartialView("_EditContact", viewModel);
        }

        public async Task<IActionResult> GetContacts()
        {
            var contactList = await _context.Contacts
                .OrderBy(x => x.FirstName)
                .ToListAsync();

            return PartialView("_ContactTable", new ContactViewModel { Contacts = contactList });
        }

        public IActionResult Index()
            {
                return View();
            }

        public IActionResult NewContact()
        {
            return PartialView("_EditContact", new EditContactViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact([FromBody]SaveContactViewModel model)
        {
            var contact = model.ContactId == Guid.Empty
                ? new Contact { Title = model.Title, FirstName = model.FirstName, LastName = model.LastName, DOB = model.DOB, PrimaryEmail = model.PrimaryEmail }
                : await _context.Contacts.Include(x => x.EmailAddresses).Include(x => x.Addresses).FirstOrDefaultAsync(x => x.Id == model.ContactId);

            if (contact == null)
            {
                return NotFound();
            }

            _context.EmailAddresses.RemoveRange(contact.EmailAddresses);
            _context.Addresses.RemoveRange(contact.Addresses);

            var found = true;
            foreach (var email in model.Emails)
            {
                contact.EmailAddresses.Add(new EmailAddress
                {
                    Type = email.Type,
                    Email = email.Email,
                    Contact = contact
                });
                if (found)
                {
                    model.PrimaryEmail = email.Email;
                    found = false;
                }
                if (email.Type==EmailType.Primary)
                {
                    model.PrimaryEmail = email.Email;
                }
            }


            foreach (var address in model.Addresses)
            {
                contact.Addresses.Add(new Address
                {
                    Street1 = address.Street1,
                    Street2 = address.Street2,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Type = address.Type
                });
            }

            contact.Title = model.Title;
            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.DOB = model.DOB;
            contact.PrimaryEmail = model.PrimaryEmail;

            if (model.ContactId == Guid.Empty)
            {
                await _context.Contacts.AddAsync(contact);
            }
            else
            {
                _context.Contacts.Update(contact);
            }


            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("Update");

            return Ok();
        }
    }

}