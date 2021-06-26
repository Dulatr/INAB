using SQLite;

using System;

namespace INAB.Models
{
    [Table("transactions")]
    public class Transaction
    {
        [PrimaryKey,AutoIncrement]
        [NotNull]
        [Column("id")]
        public int ID { get; set; }

        [NotNull]
        [Column("account_id")]
        public int AccountID { get; set; }

        [NotNull]
        [Column("amount")]
        public double Amount { get; set; }

        [NotNull]
        [Column("type")]
        public int Type { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
    }
}
