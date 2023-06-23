namespace PageGuard.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PageGuardHistoryModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NodeId { get; set; }
        public DateTime DateTimeRecord { get; set; }
        public int Status { get; set; }
    }
}
