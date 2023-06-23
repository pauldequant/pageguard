namespace PageGuard.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PageGuardStatusModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NodeId { get; set; }
        public DateTime DateTimeRecord { get; set; }
        public enum PageStatus
        {
            /// <summary>
            /// Defines the CheckedIn.
            /// </summary>
            CheckedIn = 0,

            /// <summary>
            /// Defines the CheckedOut.
            /// </summary>
            CheckedOut = 1,

            /// <summary>
            /// Defines the Override.
            /// </summary>
            Override = 2
        }
    }
}
