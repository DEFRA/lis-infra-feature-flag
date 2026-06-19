// <copyright file="FeatureFlagStatusesConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Configuration;

using Lis.Infra.FeatureFlag.Database.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class FeatureFlagStatusesConfiguration : IEntityTypeConfiguration<FeatureFlagStatuses>
{
    public void Configure(EntityTypeBuilder<FeatureFlagStatuses> builder)
    {
        var activationTypeConverter = new ValueConverter<ActivationType, string>(
            value => value.ToString().ToLowerInvariant(),
            value => Enum.Parse<ActivationType>(value, true));

        builder.ToTable(
            nameof(FeatureFlagStatuses).ToSnakeCase(),
            table =>
            {
                table.HasCheckConstraint(
                    "ck_feature_flag_statuses_activation_type",
                    "activation_type in ('manual', 'scheduled')");

                table.HasCheckConstraint(
                    "ck_feature_flag_statuses_activation_values",
                    "((activation_type = 'manual' and manual_enabled is not null and activate_after is null) or (activation_type = 'scheduled' and activate_after is not null))");

                table.HasCheckConstraint(
                    "ck_feature_flag_statuses_expire_after_activate",
                    "(expire_at is null or activate_after is null or expire_at > activate_after)");
            });

        builder.HasKey(status => status.Id);

        builder.Property(status => status.Id)
            .HasColumnName(nameof(FeatureFlagStatuses.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(status => status.GroupId)
            .HasColumnName(nameof(FeatureFlagStatuses.GroupId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(status => status.FlagId)
            .HasColumnName(nameof(FeatureFlagStatuses.FlagId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(status => status.EnvironmentId)
            .HasColumnName(nameof(FeatureFlagStatuses.EnvironmentId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(status => status.ActivationType)
            .HasColumnName(nameof(FeatureFlagStatuses.ActivationType).ToSnakeCase())
            .HasConversion(activationTypeConverter)
            .IsRequired();

        builder.Property(status => status.ManualEnabled)
            .HasColumnName(nameof(FeatureFlagStatuses.ManualEnabled).ToSnakeCase())
            .HasColumnType(ColumnTypes.Boolean);

        builder.Property(status => status.ActivateAfter)
            .HasColumnName(nameof(FeatureFlagStatuses.ActivateAfter).ToSnakeCase())
            .HasColumnType(ColumnTypes.DateTimeOffSet);

        builder.Property(status => status.ExpireAt)
            .HasColumnName(nameof(FeatureFlagStatuses.ExpireAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.DateTimeOffSet);

        builder.Property(status => status.UpdatedAt)
            .HasColumnName(nameof(FeatureFlagStatuses.UpdatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.DateTimeOffSet)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(status => status.UpdatedBy)
            .HasColumnName(nameof(FeatureFlagStatuses.UpdatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.HasIndex(status => new { status.GroupId, status.FlagId, status.EnvironmentId })
            .IsUnique();

        builder.HasOne(status => status.Group)
            .WithMany(group => group.FeatureFlagStatuses)
            .HasForeignKey(status => status.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(status => status.Flag)
            .WithMany(flag => flag.FeatureFlagStatuses)
            .HasForeignKey(status => status.FlagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(status => status.Environment)
            .WithMany(environment => environment.FeatureFlagStatuses)
            .HasForeignKey(status => status.EnvironmentId);
    }
}
