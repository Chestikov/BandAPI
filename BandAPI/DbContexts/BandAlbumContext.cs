using BandAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace BandAPI.DbContexts
{
    public class BandAlbumContext : DbContext
    {
        public BandAlbumContext(DbContextOptions<BandAlbumContext> options) : base(options)
        {
        }

        public DbSet<Band> Bands { get; set; }

        public DbSet<Album> Albums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Band>().HasData(
                new Band()
                {
                    Id = Guid.Parse("a7c5fded-f96c-4e5e-b6ed-dffd74ce0e7c"),
                    Name = "Metallica",
                    Founded = new DateTime(1980, 1, 1),
                    MainGenre = "Heavy Metal"
                },
                new Band()
                {
                    Id = Guid.Parse("2c2278a5-63ac-4be6-b3d7-2c52ca339e04"),
                    Name = "Guns N Roses",
                    Founded = new DateTime(1985, 2, 1),
                    MainGenre = "Rock"
                },
                new Band()
                {
                    Id = Guid.Parse("c4160eb1-42af-4912-930f-0bc21fad3760"),
                    Name = "ABBA",
                    Founded = new DateTime(1965, 7, 1),
                    MainGenre = "Disco"
                },
                new Band()
                {
                    Id = Guid.Parse("0276a268-33e5-4858-8589-4fa10ffbd5f9"),
                    Name = "Oasis",
                    Founded = new DateTime(1991, 12, 1),
                    MainGenre = "Alternative"
                },
                new Band()
                {
                    Id = Guid.Parse("46ad6b44-d02c-4db6-8104-d0c73520bac6"),
                    Name = "A-ha",
                    Founded = new DateTime(1981, 6, 1),
                    MainGenre = "Pop"
                });

            modelBuilder.Entity<Album>().HasData(
                new Album()
                {
                    Id = Guid.Parse("55c45c62-2be4-4764-bba3-c7d4b6fdc0fe"),
                    Title = "Master Of Puppets",
                    Description = "One pf the best heavy metal albuns ever",
                    BandId = Guid.Parse("a7c5fded-f96c-4e5e-b6ed-dffd74ce0e7c")
                },
                new Album()
                {
                    Id = Guid.Parse("16945b9e-cfb2-455a-b5e2-be67f05bf33f"),
                    Title = "Appetite for Destruction",
                    Description = "Amazing Rock album with raw sound",
                    BandId = Guid.Parse("a7c5fded-f96c-4e5e-b6ed-dffd74ce0e7c")
                },
                new Album()
                {
                    Id = Guid.Parse("9f999965-b620-4ac0-bdf7-d0a37b6b8b5f"),
                    Title = "Waterloo",
                    Description = "Very groovy albim",
                    BandId = Guid.Parse("c4160eb1-42af-4912-930f-0bc21fad3760")
                },
                new Album()
                {
                    Id = Guid.Parse("509ecd4d-d3f3-40a8-8973-c00b4153d417"),
                    Title = "Be Here Now",
                    Description = "Arguably one og the best albums by Oasis",
                    BandId = Guid.Parse("0276a268-33e5-4858-8589-4fa10ffbd5f9")
                },
                new Album()
                {
                    Id = Guid.Parse("e78d2f07-22e8-4cee-bcd5-c952a069ca8f"),
                    Title = "Hunting Hight and Low",
                    Description = "Awesome Debut album by A-Ha",
                    BandId = Guid.Parse("46ad6b44-d02c-4db6-8104-d0c73520bac6")
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
