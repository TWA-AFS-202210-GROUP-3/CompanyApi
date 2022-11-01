namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
        }

        public string? ID { get; set; }
        public string Name { get; set; }
    }
}
