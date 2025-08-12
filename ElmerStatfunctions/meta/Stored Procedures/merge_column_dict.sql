CREATE procedure [meta].[merge_column_dict] @schema_name nvarchar(50), @table_name nvarchar(50)
as

	exec meta.merge_table_dict @schema_name, @table_name

	;with cte as (
		select mt.table_id,
			sc.[name]
		from meta.schemas ms
			join meta.tables mt ON ms.[schema_id] = mt.[schema_id]
			join sys.schemas ss ON ms.[name] = ss.[name]
			join sys.tables st ON ss.schema_id = st.schema_id and mt.[name] = st.[name]
			join sys.columns sc ON st.object_id = sc.object_id
		where ms.[name] = @schema_name
			and st.[name] = @table_name
	),
	meta_table_id_list as (
		select t.table_id
		from meta.tables t
			join meta.schemas s on t.[schema_id] = s.[schema_id]
		where t.[name] = @table_name
			and s.[name] = @schema_name
	)
	merge meta.[columns] as target
	using cte as source
		ON (
			source.table_id = target.table_id 
			and source.[name] = target.[name]
		)
	when not matched then
		insert ([table_id], [name])
		values (source.[table_id], source.[name])
	when not matched by source 
		and target.table_id in (select table_id from meta_table_id_list)-- the table_id and schema are the ones we 
	then delete;
