﻿namespace PageGuard.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AllCheckoutsModel : PageStatusQueryModel
    {
        /// <summary>
        /// Gets or sets the DateTimeRecord.
        /// </summary>
        public DateTime DateTimeRecord { get; set; }

        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the UserEmail.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the Avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets the Text.
        /// </summary>
        public string Text { get; set; }
    }
}
