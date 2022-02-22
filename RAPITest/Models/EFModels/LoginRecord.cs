using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RAPITest.Models.EFModels
{
    public partial class LoginRecord
    {
        public int LoginRecordId { get; set; }
        public string UserId { get; set; }
        public DateTime LoginTime { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
