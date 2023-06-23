using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace PageGuard.Schemas
{
    using NPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TableName("PageGuardHistory")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class PageGuardHistorySchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UserId")]
        public required int UserId { get; set; }

        [Column("NodeId")]
        public required int NodeId { get; set; }

        [Column("DateTimeRecord")]
        public required DateTime DateTimeRecord { get; set; }

        [Column("Status")]
        public required int Status { get; set; }


    }
}
