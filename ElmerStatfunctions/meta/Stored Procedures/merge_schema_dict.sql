
create procedure [meta].[merge_schema_dict] @schema_name nvarchar(50)
as

;with cte as (
	select [name]
	from sys.schemas s
	where s.[name] = @schema_name
)
merge meta.[schemas] as target
using cte as source
	ON (source.[name] = target.[name])
when not matched then
	insert ([name])
	values (source.[name]);
