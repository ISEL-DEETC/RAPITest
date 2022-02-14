using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ModelsLibrary.Models.EFModels
{
    public partial class ExternalDll
    {
        public int ExternalDllId { get; set; }
        public int ApiId { get; set; }
        public string FileName { get; set; }
        public byte[] Dll { get; set; }

        public virtual Api Api { get; set; }
    }
}
