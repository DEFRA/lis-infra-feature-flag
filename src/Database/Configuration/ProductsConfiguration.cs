// <copyright file="ProductsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Configuration;

public class ProductsConfiguration : IEntityTypeConfiguration<Products>
{
    public void Configure(EntityTypeBuilder<Products> builder)
    {
        builder.ToTable(nameof(Products).ToSnakeCase());

        builder.HasKey(prd => prd.Id);

        builder.Property(prd => prd.Id)
            .HasColumnName(nameof(Products.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(prd => prd.Name)
            .HasColumnName(nameof(Products.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.Property(prd => prd.Description)
            .HasColumnName(nameof(Products.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);
    }
}
