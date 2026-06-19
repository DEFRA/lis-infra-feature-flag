// <copyright file="EnvironmentsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Configuration;

public class EnvironmentsConfiguration : IEntityTypeConfiguration<Environments>
{
    public void Configure(EntityTypeBuilder<Environments> builder)
    {
        builder.ToTable(nameof(Environments).ToSnakeCase());

        builder.HasKey(environment => environment.Id);

        builder.Property(environment => environment.Id)
            .HasColumnName(nameof(Environments.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(environment => environment.Name)
            .HasColumnName(nameof(Environments.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.Property(environment => environment.Description)
            .HasColumnName(nameof(Environments.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        builder.HasIndex(environment => environment.Name)
            .IsUnique();
    }
}
