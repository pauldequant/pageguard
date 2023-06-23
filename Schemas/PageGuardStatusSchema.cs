namespace PageGuard.Schemas
{
    using NPoco;
    using System;
    using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

    [TableName("PageGuardStatus")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class PageGuardStatusSchema
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

    }
}
