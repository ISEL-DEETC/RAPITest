using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SetupTestsWorkerService.Models.EFModels
{
    public partial class Report
    {
        public int ReportsId { get; set; }
        public int ApiId { get; set; }
        public byte[] ReportFile { get; set; }
        public DateTime ReportDate { get; set; }

        public virtual Api Api { get; set; }
    }
}
