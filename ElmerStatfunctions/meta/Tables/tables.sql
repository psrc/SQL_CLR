CREATE TABLE [meta].[tables] (
    [table_id]    INT           IDENTITY (1, 1) NOT NULL,
    [schema_id]   INT           NOT NULL,
    [name]        VARCHAR (50)  NOT NULL,
    [description] VARCHAR (MAX) NULL,
    [notes]       VARCHAR (MAX) NULL,
    [DWCHECKSUM]  BIGINT        NULL,
    CONSTRAINT [PK_meta_tables__table_id] PRIMARY KEY CLUSTERED ([table_id] ASC)
);

