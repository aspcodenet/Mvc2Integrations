using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc2Integrations.ViewModels
{
    public class KrisListViewModel
    {
        public List<Kris> Items { get; set; }

        public class Kris
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }

        }
    }
}
