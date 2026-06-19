// <copyright file="FeatureFlagsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Configuration;

public class FeatureFlagsConfiguration : IEntityTypeConfiguration<FeatureFlags>
{
    public void Configure(EntityTypeBuilder<FeatureFlags> builder)
    {
        builder.ToTable(nameof(FeatureFlags).ToSnakeCase());

        builder.HasKey(flag => flag.Id);

        builder.Property(flag => flag.Id)
            .HasColumnName(nameof(FeatureFlags.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(flag => flag.Name)
            .HasColumnName(nameof(FeatureFlags.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.Property(flag => flag.Description)
            .HasColumnName(nameof(FeatureFlags.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        builder.Property(flag => flag.GroupId)
            .HasColumnName(nameof(FeatureFlags.GroupId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.HasIndex(flag => flag.Name)
            .IsUnique();

        builder.HasOne(flag => flag.Group)
            .WithMany(group => group.FeatureFlags)
            .HasForeignKey(flag => flag.GroupId);
    }
}
