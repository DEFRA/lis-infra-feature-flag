-- liquibase formatted sql

-- changeset system:initial-seed-2
create table environments (
  id uuid primary key default uuid_generate_v4(),
  name text not null unique,
  description text
);
alter table environments
  owner to lis_infra_feature_flag_ddl;

create table feature_groups (
  id uuid primary key default uuid_generate_v4(),
  name text not null unique,
  description text
);
alter table feature_groups
  owner to lis_infra_feature_flag_ddl;

create table feature_flags (
  id uuid primary key default uuid_generate_v4(),
  name text not null unique,
  description text,
  group_id uuid references feature_groups(id)
);
alter table feature_flags
  owner to lis_infra_feature_flag_ddl;

create table feature_flag_statuses
(
  id              uuid primary key     default uuid_generate_v4(),
  group_id        uuid        not null references feature_groups (id) on delete cascade,
  flag_id         uuid references feature_flags (id) on delete cascade,
  environment_id  uuid references environments (id),
  activation_type text        not null check (activation_type in ('manual', 'scheduled')),
  manual_enabled  boolean,
  activate_after  timestamptz,
  expire_at       timestamptz,
  updated_at      timestamptz not null default now(),
  updated_by      text        not null,
  unique (group_id, flag_id, environment_id),
  check (
    (activation_type = 'manual' and manual_enabled is not null and activate_after is null)
      or
    (activation_type = 'scheduled' and activate_after is not null)
    ),
  check (
    expire_at is null
      or
    (activation_type = 'manual' and expire_at > now())
      or
    (activation_type = 'scheduled' and activate_after is not null and expire_at > activate_after)
    )
);
alter table feature_flag_statuses
  owner to lis_infra_feature_flag_ddl;
