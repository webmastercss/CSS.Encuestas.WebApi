CREATE TABLE Encuestas (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
	Uiid            NVARCHAR(MAX),
    Titulo          NVARCHAR(200) NOT NULL,
    Descripcion     NVARCHAR(500) NULL,
    FechaCreacion   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE Preguntas (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    EncuestaId      INT NOT NULL,
    Texto           NVARCHAR(300) NOT NULL,
    Tipo            INT NOT NULL,              -- 1:TextoCorto, 2:TextoLargo, 3:OpcionUnica, 4:OpcionMultiple, 5:Escala
    EsObligatoria   BIT NOT NULL DEFAULT 1,
    EscalaMin       INT NULL,
    EscalaMax       INT NULL,
    Orden           INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Preguntas_Encuestas FOREIGN KEY (EncuestaId) REFERENCES Encuestas(Id) ON DELETE CASCADE
);

CREATE TABLE Opciones (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    PreguntaId      INT NOT NULL,
    Texto           NVARCHAR(200) NOT NULL,
    Orden           INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Opciones_Preguntas FOREIGN KEY (PreguntaId) REFERENCES Preguntas(Id) ON DELETE CASCADE
);

CREATE TABLE Respuestas (
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    EncuestaId        INT NOT NULL,
    UsuarioIdentidad  NVARCHAR(150) NULL,
    FechaEnvio        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Respuestas_Encuestas FOREIGN KEY (EncuestaId) REFERENCES Encuestas(Id)
);

CREATE TABLE RespuestasDetalle (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    RespuestaId   INT NOT NULL,
    PreguntaId    INT NOT NULL,
    ValorTexto    NVARCHAR(MAX) NULL,
    ValorEntero   INT NULL,
    OpcionId      INT NULL,
    CONSTRAINT FK_RespuestasDetalle_Respuestas FOREIGN KEY (RespuestaId) REFERENCES Respuestas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RespuestasDetalle_Preguntas FOREIGN KEY (PreguntaId) REFERENCES Preguntas(Id),
    CONSTRAINT FK_RespuestasDetalle_Opciones  FOREIGN KEY (OpcionId)   REFERENCES Opciones(Id)
);

/* Índices útiles */
CREATE INDEX IX_Preguntas_EncuestaId ON Preguntas(EncuestaId);
CREATE INDEX IX_Opciones_PreguntaId ON Opciones(PreguntaId);
CREATE INDEX IX_Respuestas_EncuestaId ON Respuestas(EncuestaId);
CREATE INDEX IX_Detalle_RespuestaId ON RespuestasDetalle(RespuestaId);
CREATE INDEX IX_Detalle_PreguntaId ON RespuestasDetalle(PreguntaId);
CREATE INDEX IX_Detalle_OpcionId   ON RespuestasDetalle(OpcionId);
