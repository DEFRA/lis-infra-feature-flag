-- liquibase formatted sql

-- changeset system:initial-seed-3
INSERT INTO environments (id, name, description) VALUES
  ('429e63ac-4046-472c-8a47-5ae29a8d1024', 'dev', 'The Development Environment'),
  ('71eefac2-a066-4de4-bde4-369d45e1e5bc', 'test', 'The Test Environment'),
  ('404bfb4a-3396-4d3f-9564-8698071eeb7a', 'perf-test', 'The Performance Test Environment'),
  ('aa0f1cbf-f26b-41b5-b3cc-58edde1b1fb4', 'ext-test', 'The External Test Environment'),
  ('dae37e0c-029b-452f-8853-bd64cf7f87bb', 'prod', 'The Production Environment');
