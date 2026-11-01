-- Script para la creacion de la base de datos para ProductosApi
-- Motor que se uso: SQL Server


IF DB_ID('ProductosDB') IS NULL
    CREATE DATABASE ProductosDB;
GO

USE ProductosDB;
GO

IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL
    DROP TABLE dbo.Productos;
GO

CREATE TABLE dbo.Productos
(
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre        NVARCHAR(100)     NOT NULL,
    Descripcion   NVARCHAR(250)     NULL,
    Categoria     NVARCHAR(50)      NOT NULL,
    Precio        DECIMAL(10,2)     NOT NULL CHECK (Precio > 0),
    Stock         INT               NOT NULL CHECK (Stock >= 0),
    Activo        BIT               NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2         NOT NULL DEFAULT SYSDATETIME()
);
GO

-- Datos de ejemplo
INSERT INTO dbo.Productos (Nombre, Descripcion, Categoria, Precio, Stock, Activo)
VALUES
    (N'Laptop Lenovo IdeaPad 5', N'Ryzen 7, 16 GB RAM, 512 GB SSD', N'Computadoras', 15999.00, 10, 1),
    (N'Mouse Logitech M185',     N'Mouse inalámbrico USB',           N'Accesorios',     249.50, 50, 1),
    (N'Monitor Samsung 24"',     N'Full HD, 75 Hz, panel IPS',        N'Monitores',     2899.00, 15, 1),
    (N'Teclado Redragon Kumara', N'Teclado mecánico switches rojos',  N'Accesorios',     799.00, 25, 1),
    (N'Audífonos Sony WH-CH520', N'Bluetooth, hasta 50 h de batería', N'Audio',         1199.00,  0, 0);
GO

SELECT * FROM dbo.Productos;
GO
