using System;

namespace RecorderFunctions
{
    public class Person
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Active { get; set; } = true;
    }
}
