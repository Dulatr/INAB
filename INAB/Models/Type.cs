using SQLite;

namespace INAB.Models
{
    [Table("types")]
    public class Type
    {
        [PrimaryKey,AutoIncrement]
        [Column("id")]
        public int ID { get; set; }

        [Column("identifier")]
        public string Identifier { get; set; }
    }
}
