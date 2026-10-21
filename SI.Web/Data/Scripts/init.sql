IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017224554_AgregarProductos'
)
BEGIN
    CREATE TABLE [Productos] (
        [Id] int NOT NULL IDENTITY,
        [Codigo] nvarchar(50) NOT NULL,
        [Nombre] nvarchar(150) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [UnidadMedida] nvarchar(30) NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [ExistenciaActual] decimal(18,4) NOT NULL DEFAULT 0.0,
        [CostoPromedioActual] decimal(18,4) NOT NULL DEFAULT 0.0,
        [ValorInventarioPeps] decimal(18,2) NOT NULL DEFAULT 0.0,
        [ValorInventarioPromedio] decimal(18,2) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_Productos] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017224554_AgregarProductos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Productos_Codigo] ON [Productos] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017224554_AgregarProductos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261017224554_AgregarProductos', N'8.0.4');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018001237_AgregarMovimientos'
)
BEGIN
    CREATE TABLE [Movimientos] (
        [Id] int NOT NULL IDENTITY,
        [ProductoId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Tipo] nvarchar(10) NOT NULL,
        [Cantidad] decimal(18,4) NOT NULL,
        [CostoUnitarioEntrada] decimal(18,4) NOT NULL,
        [ExistenciaResultante] decimal(18,4) NOT NULL,
        [CostoUnitarioPeps] decimal(18,4) NOT NULL,
        [CostoTotalPeps] decimal(18,2) NOT NULL,
        [SaldoValorPeps] decimal(18,2) NOT NULL,
        [CostoUnitarioPromedio] decimal(18,4) NOT NULL,
        [CostoTotalPromedio] decimal(18,2) NOT NULL,
        [SaldoValorPromedio] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Movimientos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Movimientos_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018001237_AgregarMovimientos'
)
BEGIN
    CREATE INDEX [IX_Movimientos_Producto_Fecha_Id] ON [Movimientos] ([ProductoId], [Fecha], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018001237_AgregarMovimientos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261018001237_AgregarMovimientos', N'8.0.4');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018011057_AgregarCapasPeps'
)
BEGIN
    CREATE TABLE [CapasPeps] (
        [Id] int NOT NULL IDENTITY,
        [ProductoId] int NOT NULL,
        [MovimientoId] int NOT NULL,
        [FechaEntrada] datetime2 NOT NULL,
        [CantidadInicial] decimal(18,4) NOT NULL,
        [CantidadDisponible] decimal(18,4) NOT NULL,
        [CostoUnitario] decimal(18,4) NOT NULL,
        CONSTRAINT [PK_CapasPeps] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_CapasPeps_CantidadDisponible] CHECK ([CantidadDisponible] >= 0),
        CONSTRAINT [FK_CapasPeps_Movimientos_MovimientoId] FOREIGN KEY ([MovimientoId]) REFERENCES [Movimientos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CapasPeps_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018011057_AgregarCapasPeps'
)
BEGIN
    CREATE INDEX [IX_CapasPeps_MovimientoId] ON [CapasPeps] ([MovimientoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018011057_AgregarCapasPeps'
)
BEGIN
    CREATE INDEX [IX_CapasPeps_Producto_FechaEntrada_Id] ON [CapasPeps] ([ProductoId], [FechaEntrada], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018011057_AgregarCapasPeps'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261018011057_AgregarCapasPeps', N'8.0.4');
END;
GO

COMMIT;
GO

