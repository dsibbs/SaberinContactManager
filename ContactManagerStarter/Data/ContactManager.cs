namespace ContactManager.Data
{
    public class ContactManager : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Contact> Contacts { get; set;}
    }
}
