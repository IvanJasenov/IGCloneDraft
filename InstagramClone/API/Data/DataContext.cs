using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    // now we use use IdentityDbContext instead of DbContext class as before
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>
                                                 ,AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<AppUser> AppUser { get; set; }

        public DbSet<PhotoComment> PhotoComments { get; set; }

        public DbSet<PhotoLikes> PhotoLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region QueryFilter
            // retrive data where IsApproved property is true 
            builder.Entity<Photo>()
                   .HasQueryFilter(p => EF.Property<bool>(p, "IsApproved") == true);
            #endregion

            #region Many to many - UserRoles

            builder.Entity<AppUser>()
                    .HasMany(u => u.UserRoles)
                    .WithOne(u => u.User)
                    .HasForeignKey(u => u.UserId)
                    .IsRequired();
            
            builder.Entity<AppRole>()
                    .HasMany(u => u.UserRoles)
                    .WithOne(u => u.Role)
                    .HasForeignKey(u => u.RoleId)
                    .IsRequired();

            #endregion

            #region Likes reationship
            // form a primary key for this particular table which will be a combination of both poperties, FKs in this case
            builder.Entity<UserLike>()
                    .HasKey(k => new { k.LikedUserId, k.SourceUserId });

            builder.Entity<UserLike>()
                   .HasOne(s => s.SourceUser)
                   .WithMany(el => el.LikedUsers)
                   .HasForeignKey(s => s.SourceUserId)
                   .OnDelete(DeleteBehavior.NoAction); //  if we delete a user we also delete its related entities 
            
            builder.Entity<UserLike>()
                   .HasOne(s => s.LikedUser)
                   .WithMany(el => el.LikedByUsers)
                   .HasForeignKey(s => s.LikedUserId)
                   .OnDelete(DeleteBehavior.NoAction); //  if we delete a user we also delete its related entities

            #endregion

            #region Message
            builder.Entity<Message>()
                    .HasOne(u => u.Recipient)
                    .WithMany(m => m.MessagesRecieved)
                    .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                    .HasOne(u => u.Sender)
                    .WithMany(m => m.MessagesSend)
                    .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region PhotoComments
            // define many-many relationshio

            builder.Entity<PhotoComment>()
                .HasOne(p => p.Photo)
                .WithMany(el => el.PhotoComments)
                .HasForeignKey(s => s.PhotoId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<PhotoComment>()
                .HasOne(p => p.AppUser)
                .WithMany(el => el.PhotoComments)
                .HasForeignKey(s => s.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);


            #endregion

            #region PhotoLikes
            builder.Entity<PhotoLikes>()
                .HasKey(k => new { k.AppUserId, k.PhotoId });

            builder.Entity<PhotoLikes>()
                .HasOne(p => p.AppUser)
                .WithMany(u => u.PhotoLikes)
                .HasForeignKey(el => el.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<PhotoLikes>()
                .HasOne(p => p.Photo)
                .WithMany(u => u.PhotoLikes)
                .HasForeignKey(el => el.PhotoId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}

