namespace ApiCatalogo.Models
{
    public class CategoriaModel
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        public ICollection<ProdutoModel>? Produtos { get; set; }
    }
}
