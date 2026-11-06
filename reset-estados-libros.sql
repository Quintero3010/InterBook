-- Script para resetear estados de libros y limpiar intercambios pendientes
-- Ejecutar en SQL Server Management Studio o Azure Data Studio

USE InterBookDb;
GO

-- 1. Ver estado actual de todos los libros
SELECT Id, Titulo, EstadoDisponibilidad, UsuarioId 
FROM Libros
ORDER BY Id;

-- 2. Ver todas las solicitudes de intercambio
SELECT Id, Estado, FechaSolicitud, LibroOfrecidoId, LibroSolicitadoId
FROM Intercambios
ORDER BY FechaSolicitud DESC;

-- 3. RESETEAR: Eliminar todos los intercambios (CUIDADO - esto borra todo)
-- DELETE FROM Intercambios;

-- 4. RESETEAR: Poner todos los libros como Disponibles
UPDATE Libros
SET EstadoDisponibilidad = 'Disponible'
WHERE EstadoDisponibilidad IN ('Reservado', 'En Intercambio');

-- 5. Verificar cambios
SELECT Id, Titulo, EstadoDisponibilidad 
FROM Libros
ORDER BY Id;

PRINT 'Estados de libros reseteados correctamente';
