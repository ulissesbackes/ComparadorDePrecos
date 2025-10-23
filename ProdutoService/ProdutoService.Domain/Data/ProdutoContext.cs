using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using ProdutoService.Domain.Models;

namespace ProdutoService.Infrastructure.Data;

public class ProdutoContext : DbContext
{
    public ProdutoContext(DbContextOptions<ProdutoContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ListaCompras> ListasCompras { get; set; }
    public DbSet<ListaItem> ListaItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar chave composta para ListaItem
        modelBuilder.Entity<ListaItem>()
            .HasKey(li => new { li.ListaId, li.ProdutoId });

        // Configurar relacionamentos
        modelBuilder.Entity<ListaItem>()
            .HasOne(li => li.Lista)
            .WithMany(l => l.Itens)
            .HasForeignKey(li => li.ListaId);

        modelBuilder.Entity<ListaItem>()
            .HasOne(li => li.Produto)
            .WithMany(p => p.ListaItens)
            .HasForeignKey(li => li.ProdutoId);

        // Configurar índices
        modelBuilder.Entity<Produto>()
            .HasIndex(p => p.Nome);

        modelBuilder.Entity<Produto>()
            .HasIndex(p => p.Mercado);

        modelBuilder.Entity<ListaCompras>()
            .HasIndex(l => l.UsuarioId);

        // Configurar precisão decimal para PrecoAtual
        modelBuilder.Entity<Produto>()
            .Property(p => p.PrecoAtual)
            .HasPrecision(18, 2);
    }
}