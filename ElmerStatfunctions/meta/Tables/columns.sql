CREATE TABLE [meta].[columns] (
    [column_id]        INT           IDENTITY (1, 1) NOT NULL,
    [table_id]         INT           NOT NULL,
    [name]             VARCHAR (50)  NOT NULL,
    [description]      VARCHAR (MAX) NULL,
    [notes]            VARCHAR (MAX) NULL,
    [DWCHECKSUM]       BIGINT        NULL,
    [primary_key_part] VARCHAR (20)  NULL,
    CONSTRAINT [PK_meta_columns__id] PRIMARY KEY CLUSTERED ([column_id] ASC)
);

