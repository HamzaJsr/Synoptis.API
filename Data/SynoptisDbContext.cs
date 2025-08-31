using Microsoft.EntityFrameworkCore;
using Synoptis.API.Models;

namespace Synoptis.API.Data
{
    public class SynoptisDbContext : DbContext
    {
        public SynoptisDbContext(DbContextOptions<SynoptisDbContext> options) : base(options) { }

        public DbSet<AppelOffre> AppelOffres { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<DocumentAppelOffre> DocumentsAppelOffre { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;

        public DbSet<Client> Clients { get; set; } = null!;

        public DbSet<ContactClient> ContactClients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Relation User → Company (FK: CompanyId) ---
            modelBuilder.Entity<User>()                  // 1) On commence la config de l'entité User
                .HasOne(u => u.Company)                  // 2) Chaque User a UNE Company (prop de navigation u.Company)
                .WithMany(c => c.Users)                  // 3) Une Company a PLUSIEURS Users (prop de nav c.Users)
                .HasForeignKey(u => u.CompanyId)         // 4) La clé étrangère se trouve côté User: User.CompanyId
                .OnDelete(DeleteBehavior.Restrict);      // 5) Si on tente de supprimer une Company référencée → on bloque (pas de cascade)

            // --- Relation hiérarchique User → Responsable (self-reference) ---
            modelBuilder.Entity<User>()                  // 6) Nouvelle config sur User (autre relation)
                .HasOne(u => u.Responsable)              // 7) Un User a éventuellement UN Responsable (prop u.Responsable)
                .WithMany(r => r.Collaborateurs)         // 8) Un Responsable a PLUSIEURS Collaborateurs (prop r.Collaborateurs)
                .HasForeignKey(u => u.ResponsableId)     // 9) La FK est sur User: User.ResponsableId (vers User.Id)
                .OnDelete(DeleteBehavior.Restrict);      // 10) On bloque la suppression en cascade (évite cycles/problèmes)

            // Client → Company
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Company)
                .WithMany(co => co.Clients)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ContactClient → Company
            modelBuilder.Entity<ContactClient>()
                .HasOne(c => c.Company)
                .WithMany() // pas obligé d'ajouter une collection dans Company
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ContactClient → Client
            modelBuilder.Entity<ContactClient>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contacts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppelOffre → Client (optionnel)
            modelBuilder.Entity<AppelOffre>()
                .HasOne(a => a.Client)
                .WithMany(c => c.AppelsOffre)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index utile pour la recherche
            modelBuilder.Entity<Client>()
                .HasIndex(c => new { c.CompanyId, c.RaisonSociale });
        }
    }
}
