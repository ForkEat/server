using System;

namespace ForkEat.Web.Adapters.Files
{
    public class DbFileResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}