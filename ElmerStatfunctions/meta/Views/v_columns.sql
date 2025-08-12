CREATE view meta.v_columns
as
select 
	c.[column_id],
	s.[name] as schema_name,
	t.[name] as table_name,
	c.[name] as column_name,
	c.[description],
	c.[notes]
from meta.schemas s
	join meta.tables t ON s.[schema_id] = t.[schema_id]
	join meta.columns c ON t.[table_id] = c.table_id
