using System;

namespace ForkEat.Web.Adapters.Files
{
    public class DbFile
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public byte[] Data { get; set; }
        public string Name { get; set; }
    }
}