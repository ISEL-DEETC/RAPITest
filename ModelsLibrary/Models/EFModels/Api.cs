using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ModelsLibrary.Models.EFModels
{
    public partial class Api
    {
        public Api()
        {
            ExternalDll = new HashSet<ExternalDll>();
            Report = new HashSet<Report>();
        }

        public int ApiId { get; set; }
        public string ApiTitle { get; set; }
        public string UserId { get; set; }
        public byte[] ApiSpecification { get; set; }
        public byte[] SerializedTests { get; set; }
        public byte[] Tsl { get; set; }
        public byte[] Dictionary { get; set; }
        public DateTime? NextTest { get; set; }
        public int? TestTimeLoop { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<ExternalDll> ExternalDll { get; set; }
        public virtual ICollection<Report> Report { get; set; }
    }
}
