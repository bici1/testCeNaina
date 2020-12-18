using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace WindowsMobile.annotation
{
    [System.AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class Sequence: Attribute
    {
        public string Prefix { get; set; }
        public string Name { get; set; }
    }
}
