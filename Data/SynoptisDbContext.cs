using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synoptis.API.Models;

namespace Synoptis.API.Data
{
    public class SynoptisDbContext : DbContext
    {

        //La je cree le constructeur 
        public SynoptisDbContext(DbContextOptions<SynoptisDbContext> options) : base(options) { }




        //La je met mon model de donnee AppelDoffre je le met en tant que DbSet pour quil soit transferer a la bdd lors de la migration
        public DbSet<AppelOffre> AppelOffres { get; set; } = null!;

    }
}