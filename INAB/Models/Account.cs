using SQLite;

namespace INAB.Models
{
    [Table("accounts")]
    public class Account
    {   
        [PrimaryKey,AutoIncrement]
        [Column("id")]
        public int ID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [NotNull]
        [Column("balance")]
        public double Balance { get; set; }
    }
}
