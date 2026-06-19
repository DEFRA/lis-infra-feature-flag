// <copyright file="FeatureGroupsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Configuration;

public class FeatureGroupsConfiguration : IEntityTypeConfiguration<FeatureGroups>
{
    public void Configure(EntityTypeBuilder<FeatureGroups> builder)
    {
        builder.ToTable(nameof(FeatureGroups).ToSnakeCase());

        builder.HasKey(group => group.Id);

        builder.Property(group => group.Id)
            .HasColumnName(nameof(FeatureGroups.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(group => group.Name)
            .HasColumnName(nameof(FeatureGroups.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.Property(group => group.Description)
            .HasColumnName(nameof(FeatureGroups.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        builder.HasIndex(group => group.Name)
            .IsUnique();
    }
}
