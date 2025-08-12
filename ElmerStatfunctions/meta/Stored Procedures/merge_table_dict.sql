


CREATE procedure [meta].[merge_table_dict] @schema_name nvarchar(50), @table_name nvarchar(50)
as

exec meta.merge_schema_dict @schema_name


;with cte as (
	select ms.[schema_id],
		ms.[name] as meta_schema_name, 
		t.[name] as table_name
	from meta.schemas ms
		join sys.schemas ss ON ms.[name] = ss.[name]
		join sys.tables t ON ss.schema_id = t.schema_id
	where ms.[name] = @schema_name
		and t.[name] = @table_name
)
merge meta.[tables] as target
using cte as source
	ON (
		source.[table_name] = target.[name]
		and source.[schema_id] = target.[schema_id]
	)
when not matched then
	insert ([schema_id], [name])
	values (source.[schema_id], source.[table_name]);
