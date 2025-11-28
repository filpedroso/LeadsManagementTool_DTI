namespace exercises;

// Classe que representa uma tabela de produtos
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Duracao { get; set; } // duração em horas
}

// Classe DbContext
public class RocketseatContext : DbContext
{
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=rocketseat.db");
}

// Adicionando um novo produto com duração de 100 horas
var contexto = new RocketseatContext();
contexto.Produtos.Add(new Produto { Nome = "Formação C# Rocketseat", Duracao = 100 });
contexto.SaveChanges();

// Consultando produtos
var cursos = contexto.Produtos.ToList();

// Esse código define uma entidade (Produto) com propriedades Id, Nome e Duracao (em horas), 
// e também o contexto de banco de dados (RocketseatContext) utilizado pelo Entity Framework.
// Em seguida, cria uma nova instância do contexto, adiciona um novo produto ("Formação C# Rocketseat") 
// com duração de 100 horas, e salva essa informação no banco SQLite chamado 'rocketseat.db'.
// Por fim, recupera e armazena todos os produtos cadastrados no banco em uma lista (cursos).
