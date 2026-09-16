using BizStock.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Identity mappings.</summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasIndex(u => u.Username).IsUnique();
        b.Property(u => u.Username).HasMaxLength(60).IsRequired();
        b.Property(u => u.DisplayName).HasMaxLength(120).IsRequired();
        b.Property(u => u.Email).HasMaxLength(160);
        b.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        b.HasMany(u => u.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Role mapping.</summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles");
        b.HasIndex(r => r.Name).IsUnique();
        b.Property(r => r.Name).HasMaxLength(60).IsRequired();
        b.HasMany(r => r.RolePermissions).WithOne().HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(r => r.UserRoles).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Permission mapping.</summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("Permissions");
        b.HasIndex(p => p.Key).IsUnique();
        b.Property(p => p.Key).HasMaxLength(60).IsRequired();
        b.Property(p => p.Module).HasMaxLength(40).IsRequired();
        b.Property(p => p.DisplayName).HasMaxLength(100).IsRequired();
        b.HasMany<RolePermission>().WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>UserRole join mapping (composite key).</summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("UserRoles");
        b.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}

/// <summary>RolePermission join mapping (composite key).</summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("RolePermissions");
        b.HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }
}
