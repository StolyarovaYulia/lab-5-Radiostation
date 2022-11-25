using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Radiostation.DAL.Entities;

namespace Radiostation.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(this RadiostationContext db)
        {
            db.Database.EnsureCreated();

            if (db.Performers.Any()) return;

            const string seedUserName = "admin";
            //const string seedUserPassword = "5asfnjaAA909";
            const string seedUserHashedPassword =
                "AQAAAAEAACcQAAAAENdHIYBFh0VNwivT/L8mvPkxuLO7vgat/5RCczb0AidSM/3fXfYXmPh2FwipgdLS1Q==";

            var seedUser = new IdentityUser
            {
                UserName = seedUserName,
                NormalizedUserName = seedUserName.ToUpper(),
                PasswordHash = seedUserHashedPassword
            };

            db.Users.Add(seedUser);
            db.SaveChanges();

            var role = new IdentityRole
            {
                Name = "admin",
                NormalizedName = "ADMIN"
            };

            var role2 = new IdentityRole
            {
                Name = "user",
                NormalizedName = "USER"
            };

            db.Roles.Add(role);
            db.Roles.Add(role2);
            db.SaveChanges();

            var userRole = new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = seedUser.Id
            };

            db.UserRoles.Add(userRole);
            db.SaveChanges();

            const int performerNumber = 35;
            const int genreNumber = 35;
            const int trackNumber = 300;

            var performers = Enumerable.Range(1, performerNumber)
                .Select(performerId => new Performer
                {
                    Name = "TestPerformer_" + performerId,
                    IsGroup = false,
                    Description = "TestDescription" + performerId,
                    GroupList = ""
                })
                .ToList();
            db.Performers.AddRange(performers);
            db.SaveChanges();

            var genres = Enumerable.Range(1, genreNumber)
                .Select(genreId => new Genre
                {
                    Description = "TestDescription" + genreId,
                    Name = "TestGenre" + genreId,
                })
                .ToList();
            db.Genres.AddRange(genres);
            db.SaveChanges();

            var tracks = Enumerable.Range(1, trackNumber)
                .Select(trackId => new Track
                {
                    Duration = new Random().Next(1, 10).ToString() + ":" + new Random().Next(1, 10).ToString() + new Random().Next(1, 10).ToString(),
                    CreationDate = DateTime.Now.AddDays(-new Random().Next(0, 10)),
                    GenreId = new Random().Next(1, genreNumber),
                    Name = "TestTrack" + trackId,
                    PerformerId = new Random().Next(1, performerNumber),
                    Rating = new Random().Next(1, 6)
                })
                .ToList();

            db.Tracks.AddRange(tracks);
            db.SaveChanges();
        }
    }
}