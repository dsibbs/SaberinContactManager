using ContactManager.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerStarter
{
    public class Seeder
    {
        public static void Initialize(ApplicationContext dataContext)
        {
            dataContext.Database.EnsureCreated();

            if (!dataContext.Contacts.Any())
            {
                var bill =
                new Contact
                {
                    Id = new Guid("930d4f10-9daf-4582-b4bb-cb9abfd382b3"),
                    Title = "Mr.",
                    FirstName = "Bill",
                    LastName = "Gates",
                    DOB = new DateTime(1960, 05, 01)
                };

                var steve =
                    new Contact
                    {
                        Id = new Guid("b728f6ef-65d8-4da2-8e5f-0f67e3c3401c"),
                        Title = "Mr.",
                        FirstName = "Steve",
                        LastName = "Jobs",
                        DOB = new DateTime(1950, 09, 21)
                    };

                var sundar =
                    new Contact
                    {
                        Id = new Guid("99580d68-9d2f-4552-862e-06b3204193f1"),
                        Title = "Mr.",
                        FirstName = "Sundar",
                        LastName = "Pichai",
                        DOB = new DateTime(1980, 01, 11)
                    };


                dataContext.Contacts.AddRange(bill, steve, sundar);

                dataContext.EmailAddresses.AddRange(
                    new EmailAddress
                    {
                        Id = new Guid("5111f412-a7f4-4169-bb27-632687569ccd"),
                        Email = "Bill@gates.com",
                        Type = EmailType.Personal,
                        Contact = bill
                    },

                    new EmailAddress
                    {
                        Id = new Guid("3ddeb084-5e5d-4eca-b275-e4f6886e04dc"),
                        Email = "Steve@Jobs.com",
                        Type = EmailType.Personal,
                        Contact = steve
                    },

                    new EmailAddress
                    {
                        Id = new Guid("3a406f64-ad7b-4098-ab01-7e93aae2b851"),
                        Email = "SteveJobs@apple.com",
                        Type = EmailType.Business,
                        Contact = steve
                    },

                    new EmailAddress
                    {
                        Id = new Guid("d1a50413-20c0-4972-a351-8be24e1fc939"),
                        Email = "SundarPichai@gmail.com",
                        Type = EmailType.Business,
                        Contact = sundar
                    });

                dataContext.Addresses.AddRange(
                    new Address
                    {
                        Id = new Guid("b39467d9-a1f4-4843-aee9-7e9060448ded"),
                        Street1 = "10 Main St",
                        Street2 = string.Empty,
                        City = "Melvile",
                        State = "NY",
                        Zip = 11757,
                        Type = AddressType.Primary,
                        Contact = bill
                    },

                    new Address
                    {
                        Id = new Guid("1632295e-fc3c-49c5-b30f-d673fc816b73"),
                        Street1 = "245 Coral Place",
                        Street2 = "Appt #3",
                        City = "Westbury",
                        State = "NY",
                        Zip = 11590,
                        Type = AddressType.Primary,
                        Contact = steve
                    },

                    new Address
                    {
                        Id = new Guid("a0762d8b-9dc6-4fa0-99dc-6f7438732760"),
                        Street1 = "1 Apple Way",
                        Street2 = string.Empty,
                        City = "Los Angles",
                        State = "CA",
                        Zip = 11757,
                        Type = AddressType.Business,
                        Contact = steve
                    },

                    new Address
                    {
                        Id = new Guid("7931505c-fca8-4a35-8d24-b32df341643d"),
                        Street1 = "10 Main St",
                        Street2 = string.Empty,
                        City = "Melvile",
                        State = "NY",
                        Zip = 11757,
                        Type = AddressType.Primary,
                        Contact = sundar
                    });




                dataContext.SaveChanges();
            }
        }
    }
}
