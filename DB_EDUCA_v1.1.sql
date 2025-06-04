IF DB_ID('DB_EDUCA') IS NOT NULL
DROP DATABASE DB_EDUCA;

CREATE DATABASE DB_EDUCA;
GO

USE DB_EDUCA
GO

-- Tabla Usuario
CREATE TABLE Usuario (
    id_usuario INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(100) NOT NULL,
    Rol VARCHAR(50) NOT NULL
);

-- Tabla Docente
CREATE TABLE Docente (
    id_docente INT PRIMARY KEY IDENTITY(1,1),
    nombres VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,
    id_usuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(id_usuario)
);

-- Tabla AdministradorSistema
CREATE TABLE AdministradorSistema (
    id_admin INT PRIMARY KEY IDENTITY(1,1),
    nombres VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,
    id_usuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(id_usuario)
);

-- Tabla Curso
CREATE TABLE Curso (
    id_curso INT PRIMARY KEY IDENTITY(1,1),
    nombre_curso VARCHAR(100) NOT NULL
);

-- Tabla Seccion
CREATE TABLE Seccion (
    id_seccion INT PRIMARY KEY IDENTITY(1,1),
    nombre_seccion VARCHAR(50) NOT NULL
);

-- Tabla Turno
CREATE TABLE Turno (
    id_turno INT PRIMARY KEY IDENTITY(1,1),
    nombre_turno VARCHAR(50) NOT NULL
);

-- Tabla Estudiante
CREATE TABLE Estudiante (
    id_estudiante INT PRIMARY KEY IDENTITY(1,1),
    codigo_estudiante VARCHAR(20) NOT NULL UNIQUE,
    apellidos VARCHAR(50) NOT NULL,
    nombres VARCHAR(50) NOT NULL
);

-- Tabla AsignacionCurso
CREATE TABLE AsignacionCurso (
    id_asignacion INT PRIMARY KEY IDENTITY(1,1),
    id_docente INT NOT NULL FOREIGN KEY REFERENCES Docente(id_docente),
    id_curso INT NOT NULL FOREIGN KEY REFERENCES Curso(id_curso),
    id_seccion INT NOT NULL FOREIGN KEY REFERENCES Seccion(id_seccion),
    id_turno INT NOT NULL FOREIGN KEY REFERENCES Turno(id_turno)
);

-- Tabla AsistenciaDocente
CREATE TABLE AsistenciaDocente (
    id_asistencia_docente INT PRIMARY KEY IDENTITY(1,1),
    id_asignacion INT NOT NULL FOREIGN KEY REFERENCES AsignacionCurso(id_asignacion),
    fecha DATE NOT NULL,
    hora TIME NOT NULL,
    estado_asistencia CHAR(1) CHECK (estado_asistencia IN ('A','F','T')) NOT NULL -- Asistió, Falta, Tardanza
);

-- Tabla Matricula
CREATE TABLE Matricula (
    id_matricula INT PRIMARY KEY IDENTITY(1,1),
    id_estudiante INT NOT NULL FOREIGN KEY REFERENCES Estudiante(id_estudiante),
    id_asignacion INT NOT NULL FOREIGN KEY REFERENCES AsignacionCurso(id_asignacion),
	UNIQUE (id_estudiante, id_asignacion) -- No puede repetir misma asignación
);

-- Tabla AsistenciaEstudiante
CREATE TABLE AsistenciaEstudiante (
    id_asistencia_estudiante INT PRIMARY KEY IDENTITY(1,1),
    id_matricula INT NOT NULL FOREIGN KEY REFERENCES Matricula(id_matricula),
    fecha DATE NOT NULL,
    hora TIME NOT NULL,
    estado_asistencia CHAR(1) CHECK (estado_asistencia IN ('A','F','T')) NOT NULL -- Asistió, Falta, Tardanza
);

-- Tabla Notas
CREATE TABLE Notas (
    id_nota INT PRIMARY KEY IDENTITY(1,1),
    id_matricula INT NOT NULL FOREIGN KEY REFERENCES Matricula(id_matricula),
    nota_T1 DECIMAL(5,2) CHECK (nota_T1 BETWEEN 0 AND 20),
    nota_T2 DECIMAL(5,2) CHECK (nota_T2 BETWEEN 0 AND 20),
    nota_EF DECIMAL(5,2) CHECK (nota_EF BETWEEN 0 AND 20),
	promedio AS ((nota_T1 * 0.25) + (nota_T2 * 0.25) + (nota_EF * 0.50)) PERSISTED,
    estado VARCHAR(20) NOT NULL
);
GO
/*TRIGGER PARA VALIDAR CONTRASEÑA*/
CREATE TRIGGER trg_ValidarPasswordUsuario
ON Usuario
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar todas las filas afectadas
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE LEN(Password) < 8
           OR Password NOT LIKE '%[A-Z]%'     -- No contiene mayúscula
           OR Password NOT LIKE '%[a-z]%'     -- No contiene minúscula
           OR Password NOT LIKE '%[0-9]%'     -- No contiene número
           OR Password NOT LIKE '%[^a-zA-Z0-9]%' -- No contiene carácter especial
    )
    BEGIN
        RAISERROR('La contraseña debe tener al menos 8 caracteres, incluyendo una mayúscula, una minúscula, un número y un carácter especial.', 16, 1);
        RETURN;
    END

    -- Si la validación pasa, realizar INSERT o UPDATE
    MERGE Usuario AS target
    USING inserted AS source
    ON target.id_usuario = source.id_usuario
    WHEN MATCHED THEN
        UPDATE SET
            Username = source.Username,
            Password = source.Password,
            Rol = source.Rol
    WHEN NOT MATCHED THEN
        INSERT (Username, Password, Rol)
        VALUES (source.Username, source.Password, source.Rol);
END;
GO
/*EJEMPLO DE CONTRASEÑA
INSERT INTO Usuario (Username, Password, Rol)
VALUES ('admin', 'Admin123@', 'Administrador');*/
/*TRIGGER PARA EL ESTADO (APROBADO/DESAPROBADO)*/
CREATE TRIGGER trg_ActualizarEstadoNota
ON Notas
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE N
    SET estado = CASE 
                    WHEN i.promedio >= 13 THEN 'Aprobado' 
                    ELSE 'Desaprobado' 
                 END
    FROM Notas N
    INNER JOIN inserted i ON N.id_nota = i.id_nota;
END;
GO

/*INSERTAR DATOS*/
-- Tabla: Turno
INSERT INTO Turno (nombre_turno) VALUES
('Mañana'),
('Tarde'),
('Noche');

-- Tabla: Seccion
INSERT INTO Seccion (nombre_seccion) VALUES
('A1'),
('B1'),
('C1'),
('D1'),
('E1'),
('F1');

-- Tabla: Usuario (Administrador y Docentes)
INSERT INTO Usuario (Username, Password, Rol) VALUES
('admin123', 'Admin123@', 'Administrador'),
('lmendoza', 'LuisM123@', 'Docente'),
('mrodriguez', 'MariaR123@', 'Docente'),
('cadmin', 'Admin456@', 'Administrador'),
('jvaldez', 'JoseVal123@', 'Docente'),
('rsanchez', 'RosaS123@', 'Docente'),
('fchavez', 'FernandoCh@2023', 'Docente'),
('bvasquez', 'BeatrizV2023@', 'Docente');

-- Tabla: AdministradorSistema
INSERT INTO AdministradorSistema (nombres, apellidos, id_usuario) VALUES
('Carlos Eduardo', 'Mendoza Diaz', 1),
('Carmen Allison', 'Díaz Colorado', 4);

-- Tabla: Docente
INSERT INTO Docente (nombres, apellidos, id_usuario) VALUES
('Luis Alonso', 'Mendoza Moreno', 2),
('Maria Ruth', 'Rodríguez Ordoñez', 3),
('Jose Valerio', 'Valdez Montes', 5),
('Rosa Ana', 'Sanchez Savedra', 6),
('Fernando Dean', 'Chávez Parker', 7),
('Beatriz Marta', 'Vasquez Coronel', 8);

-- Tabla: Curso
INSERT INTO Curso (nombre_curso) VALUES
('Programación Web'),
('Bases de Datos'),
('Redes de Computadoras'),
('Desarrollo Web'),
('Inteligencia Artificial'),
('Ciberseguridad'),
('Cloud Computing'),
('Big Data'),
('Machine Learning'),
('Desarrollo Móvil'),
('Fundamentos de Algoritmos');

-- Tabla: AsignacionCurso
INSERT INTO AsignacionCurso (id_docente, id_curso, id_seccion, id_turno) VALUES
(1, 1, 1, 1), -- Luis - Programación Web - A1 - Mañana
(2, 2, 2, 2), -- María - BD - B1 - Tarde
(1, 3, 1, 1), -- Luis - Redes - A1 - Mañana
(3, 4, 3, 1), -- Jose - Desarrollo Web - C1 - Mañana
(4, 5, 4, 2), -- Rosa - IA - D1 - Tarde
(5, 6, 5, 3), -- Fernando - Ciberseguridad - E1 - Noche
(6, 7, 6, 2), -- Beatriz - Cloud Computing - F1 - Tarde
(1, 8, 1, 1), -- Luis - Big Data - A1 - Mañana
(2, 9, 2, 3), -- María - ML - B1 - Noche
(3, 10, 3, 1), -- Jose - Desarrollo Móvil - C1 - Mañana
(4, 11, 4, 2); -- Rosa - Fundamentos - D1 - Tarde

-- Tabla: Estudiante
INSERT INTO Estudiante (codigo_estudiante, apellidos, nombres) VALUES
('EDU001', 'Gonzales Navarro', 'Ana Cristel'),
('EDU002', 'Torres Mendoza', 'Carlos Augusto'),
('EDU003', 'Ruiz Suarez', 'María Lucero'),
('EDU004', 'Castro Sanchez', 'Sergio Valentino'),
('EDU005', 'Ramirez Roque', 'Laura Maria'),
('EDU006', 'Fernandez Bazan', 'Jorge Miguel'),
('EDU007', 'Chavez Montes', 'Brenda Carolina'),
('EDU008', 'Perez Martinez', 'Luis Miguel'),
('EDU009', 'Garcia Perez', 'Diana Angelica'),
('EDU010', 'Suarez Uriarte', 'Valeria Abigail'),
('EDU011', 'Salazar Solorzano', 'Mario Adam'),
('EDU012', 'Lopez Arista', 'Karina Paola'),
('EDU013', 'Medina Cueva', 'Pedro Leonardo');

-- Tabla: Matricula
INSERT INTO Matricula (id_estudiante, id_asignacion) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6),
(7, 7),
(8, 8),
(9, 1),
(10, 2),
(11, 3),
(12, 9),
(13, 10);

