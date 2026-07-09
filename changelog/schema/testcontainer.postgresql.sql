-- liquibase formatted sql

-- changeset test-container:999-1 runAlways:true context:testcontainer splitStatements:false
DO $$
DECLARE
  -- ENVIRONMENT
  DEV CONSTANT uuid := '429e63ac-4046-472c-8a47-5ae29a8d1024';
  TEST CONSTANT uuid := '71eefac2-a066-4de4-bde4-369d45e1e5bc';
  PERF_TEST CONSTANT uuid := '404bfb4a-3396-4d3f-9564-8698071eeb7a';
  EXT_TEST CONSTANT uuid := 'aa0f1cbf-f26b-41b5-b3cc-58edde1b1fb4';
  PROD CONSTANT uuid := 'dae37e0c-029b-452f-8853-bd64cf7f87bb';

  -- PRODUCTS
  PRD_LIS uuid := '89a3622a-00c4-405b-be14-dcb31be6e4f5';
          
  -- GROUPS
  GRP_CATTLE_REG CONSTANT uuid := '0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1';

  -- FLAGS
  FLG_CATTLE_REG_BASIC CONSTANT uuid := '83bf35f9-fd59-4c8a-b70a-7d95a1aab2b1';
  FLG_CATTLE_REG_DAM CONSTANT uuid := '83bf35f9-fd59-4c8a-b70a-7d95a1aab2b2';
  FLG_CATTLE_REG_SIRE CONSTANT uuid := '83bf35f9-fd59-4c8a-b70a-7d95a1aab2b3';
  FLG_CATTLE_REG_BREED CONSTANT uuid := '83bf35f9-fd59-4c8a-b70a-7d95a1aab2b4';
BEGIN

  -- clear down
  TRUNCATE TABLE
    feature_flag_statuses,
    feature_flags,
    feature_groups
    RESTART IDENTITY
    CASCADE;

  insert into feature_groups (product_id, id, name, description) values
    (PRD_LIS, GRP_CATTLE_REG, 'Cattle_Register', 'Cattle registration features');
  
  insert into feature_flags (product_id, group_id, id, name, description) values
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_BASIC, 'basic_details', 'show basic details questions'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_DAM, 'dam_details', 'show dam details questions'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_SIRE, 'sire_details', 'show sire details questions'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_BREED, 'breed_details', 'show breed details questions');

  insert into feature_flag_statuses (product_id, group_id, flag_id, environment_id, activation_type, manual_enabled, activate_after, expire_at, updated_by) values
    (PRD_LIS, GRP_CATTLE_REG, null, null, 'manual', true, null, null, 'system'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_BASIC, DEV, 'manual', true, null, null, 'system'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_BASIC, TEST, 'manual', false, null, null, 'system'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_DAM, TEST, 'scheduled', null, '2026-06-15 10:30:00+00', null, 'system'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_SIRE, TEST, 'scheduled', null, '2026-06-15 10:30:00+00', null, 'system'),
    (PRD_LIS, GRP_CATTLE_REG, FLG_CATTLE_REG_BREED, TEST, 'scheduled', null, '2026-06-15 10:30:00+00', null, 'system');

END
$$;