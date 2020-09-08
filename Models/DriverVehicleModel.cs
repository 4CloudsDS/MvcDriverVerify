using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDriverVerify.Models
{
    public partial class DriverVehicleModel
    {

        public List<User> Users { get; set; }
        public SelectList Vehicles { get; set; }
        public string UserVehicles { get; set; }
        public string S { get; set; }
    }
}
