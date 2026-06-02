using BancoApi.Domain;
using Microsoft.EntityFrameworkCore;
                

namespace BancoApi.Data;

public class BancoContext : DbContext
{
    public BancoContext(DbContextOptions<BancoContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<LimiteCredito> LimitesCredito => Set<LimiteCredito>();
    public DbSet<ChavePix> ChavesPix => Set<ChavePix>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();            
            entity.Property(c => c.TipoPessoa).HasColumnName("tipo_pessoa").HasConversion<string>();
            entity.Property(c => c.Nome).HasColumnName("nome_razao");
            entity.Property(c => c.DocumentoPrincipal).HasColumnName("documento");
            entity.Property(c => c.DocumentoSecundario).HasColumnName("documento_secundario");
            entity.Property(c => c.DataNascimentoFundacao).HasColumnName("data_nascimento_fundacao");
            entity.Property(c => c.Email).HasColumnName("email");
            entity.Property(c => c.Telefone).HasColumnName("telefone");
            entity.Property(c => c.Logradouro).HasColumnName("logradouro");
            entity.Property(c => c.NumeroEndereco).HasColumnName("numero_endereco");
            entity.Property(c => c.Cep).HasColumnName("cep");
            entity.Property(c => c.Cidade).HasColumnName("cidade");
            entity.Property(c => c.Uf).HasColumnName("uf");
            entity.Property(c => c.RendaMensal).HasColumnName("renda_mensal");
            entity.Property(c => c.SenhaHash).HasColumnName("senha_hash");
            entity.Property(c => c.TentativasFalhas).HasColumnName("tentativas_falhas");
            entity.Property(c => c.StatusCliente).HasColumnName("status_cliente").HasConversion<string>();
            entity.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
            entity.HasIndex(c => c.DocumentoPrincipal).IsUnique();
            entity.HasIndex(c => c.NumeroConta).IsUnique();
            entity.HasIndex(c => c.ChavePix).IsUnique();
            entity.Property(c => c.NumeroConta).HasColumnName("numero_conta");
            entity.Property(c => c.ChavePix).HasColumnName("chave_pix");
            entity.Property(c => c.NumeroAgencia).HasColumnName("numero_agencia");
            entity.Property(c => c.TipoConta).HasColumnName("tipo_conta").HasConversion<string>();
            entity.Property(c => c.Saldo).HasColumnName("saldo");
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.ToTable("transacoes");
            entity.HasOne(t => t.ContaOrigem)
                .WithMany()
                .HasForeignKey(t => t.ContaOrigemId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.ContaDestino)
                .WithMany()
                .HasForeignKey(t => t.ContaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LimiteCredito>(entity =>
        {
            entity.ToTable("limites_credito");
        });

        modelBuilder.Entity<ChavePix>(entity =>
        {
            entity.ToTable("chaves_pix");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.ClienteId).HasColumnName("cliente_id");
            entity.Property(c => c.Tipo).HasColumnName("tipo").HasConversion<string>();
            entity.Property(c => c.Valor).HasColumnName("valor");
            entity.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(c => c.Ativa).HasColumnName("ativa");
            entity.HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(c => c.Valor).IsUnique();
        });
    }
}