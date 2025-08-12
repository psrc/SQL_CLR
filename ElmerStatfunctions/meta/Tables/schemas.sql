CREATE TABLE [meta].[schemas] (
    [schema_id]   INT           IDENTITY (1, 1) NOT NULL,
    [name]        VARCHAR (50)  NOT NULL,
    [description] VARCHAR (MAX) NULL,
    [notes]       VARCHAR (MAX) NULL,
    [DWCHECKSUM]  BIGINT        NULL,
    CONSTRAINT [PK_meta_schemas__id] PRIMARY KEY CLUSTERED ([schema_id] ASC)
);

