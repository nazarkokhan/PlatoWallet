namespace Platipus.Wallet.Infrastructure.EntityConfigurations.Base;

using Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.Abstract.Generic;

public abstract class EntityTypeConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CreatedDate);
        builder.Property(x => x.CreatedDate).HasDefaultValueSqlDateTimeUtcNow().ValueGeneratedOnAdd();

        builder.HasIndex(x => x.LastUpdatedDate);
        builder.Property(x => x.LastUpdatedDate).HasDefaultValueSqlDateTimeUtcNow().ValueGeneratedOnUpdate();
    }
}

public abstract class EntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity, Guid>
    where TEntity : Entity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Id).HasDefaultValueSqlNewGuid().ValueGeneratedOnAdd();
    }
}