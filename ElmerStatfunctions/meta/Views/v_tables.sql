create view meta.v_tables
as
select 
	t.table_id,
	s.[name] as schema_name,
	t.[name] as table_name,
	t.[description],
	t.[notes]
from meta.schemas s
	join meta.tables t ON s.[schema_id] = t.[schema_id]