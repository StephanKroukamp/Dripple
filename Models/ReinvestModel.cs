using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Dripple.Models
{
    public class ReinvestModel
    {
        public Guid Address { get; set; }

        public List<SelectListItem> AvailableAddresses { get; set; }
    }
}