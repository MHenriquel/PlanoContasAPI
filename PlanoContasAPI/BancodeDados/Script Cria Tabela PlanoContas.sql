USE [PlanoContasDB]
GO

/****** Object:  Table [dbo].[PlanoContas]    Script Date: 25/02/2025 13:39:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlanoContas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](max) NOT NULL,
	[NomedaConta] [varchar](150) NOT NULL,
	[Tipo] [varchar](50) NOT NULL,
	[AceitaLancamentos] [bit] NOT NULL,
	[Ativo] [bit] NOT NULL,
 CONSTRAINT [PK_PlanoContas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlanoContas] ADD  CONSTRAINT [DF_PlanoContas_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO

