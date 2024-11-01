using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchEngine.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualStopWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StopWords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Word = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopWords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StopWords_Word_Language",
                table: "StopWords",
                columns: new[] { "Word", "Language" },
                unique: true);
            
            
            var insertedWords = new HashSet<(string word, string lang)>();

            void InsertStopWord(string word, string language)
            {
                if (insertedWords.Add((word, language)))
                {
                    migrationBuilder.InsertData(
                        table: "StopWords",
                        columns: new[] { "Id", "Word", "Language", "CreatedAt" },
                        values: new object[] { Guid.NewGuid(), word, language, DateTime.UtcNow }
                    );
                }
            }
            var englishStopWords = new[] {
                "a", "an", "and", "are", "as", "at", "be", "been", "but", "by", "for", "from", "had", "has", 
                "have", "he", "her", "his", "i", "in", "is", "it", "its", "of", "on", "that", "the", "to", 
                "was", "were", "will", "with", "you", "your", "about", "above", "after", "again", "all", 
                "also", "am", "any", "because", "before", "being", "below", "between", "both", "can", "did", 
                "do", "does", "doing", "down", "during", "each", "few", "further", "had", "has", "have", 
                "having", "here", "how", "if", "into", "just", "more", "most", "no", "not", "only", "or", 
                "other", "our", "out", "over", "same", "should", "so", "some", "such", "than", "then", 
                "there", "these", "they", "this", "through", "too", "under", "up", "very", "what", "when", 
                "where", "which", "while", "who", "whom", "why", "would"
            };

            var spanishStopWords = new[] {
                "a", "al", "algo", "algunas", "algunos", "ante", "antes", "como", "con", "contra", "cual", 
                "cuando", "de", "del", "desde", "donde", "durante", "e", "el", "ella", "ellas", "ellos", "en",
                "entre", "era", "erais", "eran", "eras", "eres", "es", "esa", "esas", "ese", "eso", "esos",
                "esta", "estaba", "estado", "estais", "estamos", "estan", "estar", "estas", "este", "esto",
                "estos", "estoy", "etc", "fin", "fue", "fueron", "fui", "fuimos", "ha", "hace", "haceis",
                "hacemos", "hacen", "hacer", "haces", "hacia", "hago", "hasta", "hay", "he", "la", "las",
                "le", "les", "lo", "los", "me", "mi", "mis", "mucho", "muchos", "muy", "nada", "ni", "no",
                "nos", "nosotros", "nuestra", "nuestras", "nuestro", "nuestros", "o", "os", "otra", "otras",
                "otro", "otros", "para", "pero", "poco", "por", "porque", "que", "quien", "quienes", "qué",
                "se", "sea", "seais", "seamos", "sean", "seas", "sera", "seras", "sere", "seria", "serian",
                "serias", "será", "serán", "serás", "seré", "seréis", "sería", "seríais", "seríamos",
                "serían", "serías", "si", "sido", "siendo", "sin", "sobre", "sois", "somos", "son", "soy",
                "su", "sus", "suya", "suyas", "suyo", "suyos", "sí", "también", "tanto", "te", "teneis",
                "tenemos", "tener", "tengo", "ti", "tiene", "tienen", "tienes", "todo", "todos", "tu", "tus",
                "tuve", "tuviera", "tuvierais", "tuvieran", "tuvieras", "tuvieron", "tuviese", "tuvieseis",
                "tuviesen", "tuvieses", "tuvimos", "tuviste", "tuvisteis", "tuvo", "tuya", "tuyas", "tuyo",
                "tuyos", "tú", "un", "una", "uno", "unos", "vosotras", "vosotros", "vuestra", "vuestras",
                "vuestro", "vuestros", "y", "ya", "yo", "él", "éramos"
            };

            var portugueseStopWords = new[] {
                "a", "ao", "aos", "aquela", "aquelas", "aquele", "aqueles", "aquilo", "as", "até", "com",
                "como", "da", "das", "de", "dela", "delas", "dele", "deles", "depois", "do", "dos", "e",
                "ela", "elas", "ele", "eles", "em", "entre", "era", "eram", "essa", "essas", "esse", "esses",
                "esta", "estas", "este", "estes", "eu", "foi", "foram", "há", "isso", "isto", "já", "lhe",
                "lhes", "mais", "mas", "me", "mesmo", "meu", "meus", "minha", "minhas", "muito", "na", "nas",
                "nem", "no", "nos", "nossa", "nossas", "nosso", "nossos", "num", "numa", "não", "nós", "o",
                "os", "ou", "para", "pela", "pelas", "pelo", "pelos", "por", "qual", "quando", "que", "quem",
                "se", "sem", "seu", "seus", "só", "sua", "suas", "são", "também", "te", "tem", "têm", "teu",
                "teus", "tu", "tua", "tuas", "um", "uma", "umas", "uns", "você", "vocês", "vos", "à", "às"
            };

            var frenchStopWords = new[] {
                "a", "ai", "aie", "aient", "aies", "ait", "alors", "après", "as", "au", "aucun", "aura",
                "aurai", "auraient", "aurais", "aurait", "auras", "aurez", "auriez", "aurions", "aurons",
                "auront", "aussi", "aux", "avaient", "avais", "avait", "avant", "avec", "avez", "aviez",
                "avions", "avoir", "avons", "ayant", "ayez", "ayons", "c", "ce", "ceci", "cela", "celle",
                "celles", "celui", "cependant", "ces", "cet", "cette", "ceux", "chez", "comme", "d", "dans",
                "de", "des", "donc", "dont", "du", "elle", "elles", "en", "entre", "et", "étaient", "étais",
                "était", "étant", "été", "être", "eu", "eue", "eues", "eurent", "eus", "eusse", "eussent",
                "eusses", "eussiez", "eussions", "eut", "eux", "eûmes", "eût", "eûtes", "il", "ils", "je",
                "l", "la", "le", "les", "leur", "leurs", "lui", "m", "ma", "mais", "me", "mes", "moi", "mon",
                "même", "n", "ne", "nos", "notre", "nous", "on", "ont", "ou", "par", "pas", "pour", "qu",
                "que", "quel", "quelle", "quelles", "quels", "qui", "s", "sa", "sans", "se", "sera", "serai",
                "seraient", "serais", "serait", "seras", "serez", "seriez", "serions", "serons", "seront",
                "ses", "soi", "soient", "sois", "soit", "sommes", "son", "sont", "soyez", "soyons", "suis",
                "sur", "t", "ta", "te", "tes", "toi", "ton", "tous", "tout", "tu", "un", "une", "vos",
                "votre", "vous", "y", "à", "étant", "été", "être"
            };

            foreach (var word in englishStopWords)
            {
                InsertStopWord(word, "en");
            }

            foreach (var word in spanishStopWords)
            {
                InsertStopWord(word, "es");
            }

            foreach (var word in portugueseStopWords)
            {
                InsertStopWord(word, "pt");
            }

            foreach (var word in frenchStopWords)
            {
                InsertStopWord(word, "fr");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StopWords");
        }
    }
}
