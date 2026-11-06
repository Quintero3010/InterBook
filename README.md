# InterBook - Sistema de Intercambio de Libros

![SonarCloud](https://img.shields.io/badge/SonarCloud-Passed-brightgreen?logo=sonarcloud)
![Coverage](https://img.shields.io/badge/Coverage-70.7%25-brightgreen)
![Tests](https://img.shields.io/badge/Tests-21%20passed-success)
![Security](https://img.shields.io/badge/Security-0%20issues-success)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)

Sistema de autenticación para plataforma de intercambio de libros entre lectores, desarrollado con metodología TDD y análisis SAST con SonarCloud.

---

## Tabla de Contenidos

- [Descripción](#descripción)
- [Características](#características)
- [Tecnologías](#tecnologías)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Instalación](#instalación)
- [Ejecución de Pruebas](#ejecución-de-pruebas)
- [Análisis de Calidad](#análisis-de-calidad)
- [Métricas del Proyecto](#métricas-del-proyecto)
- [Requisitos Funcionales](#requisitos-funcionales)
- [Metodología TDD](#metodología-tdd)
- [Análisis SAST](#análisis-sast)
- [Code Smells Identificados](#code-smells-identificados)
- [Autor](#autor)

---

## Descripción

**InterBook** es una aplicación web para el intercambio de libros entre lectores. Este repositorio implementa el **módulo de autenticación** con las siguientes funcionalidades:

- Registro de usuarios con validación de contraseñas seguras
- Login con protección contra fuerza bruta (bloqueo tras 5 intentos)
- Validación de formato de email según RFC 5322
- Registro de auditoría de intentos fallidos
- Verificación de cuentas por email/teléfono

---

## Características

### Seguridad
- Validación de contraseñas con 4 reglas (mínimo 8 caracteres, mayúscula, número, carácter especial)
- Protección contra ataques ReDoS (timeout en expresiones regulares)
- Bloqueo automático de cuentas tras 5 intentos fallidos (15 minutos)
- Auditoría completa de intentos de login

### Calidad de Código
- **70.7% de cobertura** de código (superando el 40% requerido)
- **21 pruebas unitarias** con metodología TDD
- **0 vulnerabilidades** de seguridad detectadas
- **0 Security Hotspots** según análisis SAST
- Calificación **A** en seguridad, fiabilidad y mantenibilidad

---

## Tecnologías

### Backend
- **Framework:** ASP.NET Core 8.0
- **Lenguaje:** C# 12

### Testing
- **Framework de pruebas:** xUnit 2.9.3
- **Mocking:** Moq 4.20.72
- **Cobertura:** dotnet-coverage

### Análisis de Calidad
- **SAST:** SonarCloud
- **CI/CD:** Manual (preparado para GitHub Actions)

---

## Estructura del Proyecto
```
InterBook/
├── InterBook.API/              # Código de producción (~600 líneas)
│   ├── Models/                 # Entidades del dominio
│   │   └── Usuario.cs
│   ├── DTOs/                   # Objetos de transferencia de datos
│   │   ├── LoginDto.cs
│   │   └── RegistroDto.cs
│   ├── Services/               # Lógica de negocio
│   │   ├── ValidadorPassword.cs
│   │   ├── ValidadorEmail.cs
│   │   └── ServicioLogin.cs
│   ├── Interfaces/             # Contratos de servicios
│   │   └── Interfaces.cs
│   └── Controllers/            # Controladores MVC
│       └── HomeController.cs
│
├── InterBook.Tests/            # Pruebas unitarias (~400 líneas)
│   ├── ValidadorPasswordTests.cs
│   ├── ValidadorEmailTests.cs
│   └── ServicioLoginTests.cs
│
├── coverage.xml                # Reporte de cobertura para SonarCloud
└── README.md                   # Este archivo
```

---

## Instalación

### Prerrequisitos
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Pasos

**1. Clonar el repositorio**
```bash
git clone https://github.com/quintero3010/InterBook.git
cd InterBook
```

**2. Restaurar dependencias**
```bash
dotnet restore
```

**3. Compilar el proyecto**
```bash
dotnet build
```

---

## Ejecución de Pruebas

### Ejecutar todas las pruebas
```bash
dotnet test
```

**Resultado esperado:**
```
Resumen de pruebas: total: 21; con errores: 0; correcto: 21; omitido: 0
```

### Ejecutar pruebas con cobertura
```bash
dotnet-coverage collect "dotnet test InterBook.Tests/InterBook.Tests.csproj" -f xml -o "coverage.xml"
```

### Generar reporte HTML de cobertura
```bash
reportgenerator -reports:"coverage.xml" -targetdir:"coveragereport" -reporttypes:Html
start coveragereport\index.html
```

---

## Análisis de Calidad

### Ejecutar análisis con SonarCloud
```bash
# 1. Limpiar proyecto
dotnet clean

# 2. Iniciar análisis
dotnet sonarscanner begin /o:"quintero3010" /k:"InterBook" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.token="YOUR_TOKEN" \
  /d:sonar.cs.vscoveragexml.reportsPaths="coverage.xml"

# 3. Compilar
dotnet build --no-incremental

# 4. Ejecutar pruebas con cobertura
dotnet-coverage collect "dotnet test InterBook.Tests/InterBook.Tests.csproj" -f xml -o "coverage.xml"

# 5. Finalizar análisis
dotnet sonarscanner end /d:sonar.token="YOUR_TOKEN"
```

### Ver resultados
[Dashboard de SonarCloud](https://sonarcloud.io/dashboard?id=InterBook)

---

## Métricas del Proyecto

| Métrica | Resultado | Estado |
|---------|-----------|--------|
| **Líneas de código** | ~1,000 | Aprobado |
| **Cobertura de pruebas** | 70.7% | Aprobado (>40%) |
| **Pruebas unitarias** | 21 | Aprobado |
| **Security (Seguridad)** | 0 issues | A |
| **Reliability (Fiabilidad)** | 0 issues | A |
| **Maintainability (Mantenibilidad)** | 21 issues | A |
| **Security Hotspots** | 0 | Aprobado |
| **Duplicación de código** | 0.0% | Aprobado |
| **Deuda técnica** | <5% | Aprobado |

---

## Requisitos Funcionales

### RF-001: Registro de Usuario
**Como** lector  
**Quiero** crear una cuenta con email o teléfono  
**Para** acceder a la plataforma y gestionar mis libros

**Criterios de aceptación:**
- La contraseña debe cumplir: mínimo 8 caracteres, 1 mayúscula, 1 número, 1 carácter especial
- Se envía código de verificación por email/SMS
- Solo se activa la cuenta tras validar el código

### RF-002: Inicio de Sesión
**Como** lector  
**Quiero** iniciar sesión con mi cuenta  
**Para** acceder a mis libros e intercambios

**Criterios de aceptación:**
- Inicio de sesión por email o teléfono
- Bloqueo temporal tras 5 intentos fallidos (15 minutos)
- Los intentos se registran en base de datos para auditoría
- Flujo de recuperación de contraseña disponible

---

## Metodología TDD

Este proyecto fue desarrollado siguiendo **Test-Driven Development**:

### Ciclo TDD Aplicado
1. **RED:** Escribir prueba que falla
2. **GREEN:** Escribir código mínimo para que pase
3. **REFACTOR:** Mejorar código sin romper pruebas

### Ejemplo de Prueba
```csharp
[Fact]
public void ValidarFormatoPassword_DebeRetornarTrue_CuandoPasswordEsValido()
{
    // Arrange
    string password = "Password123!";

    // Act
    bool resultado = _validador.ValidarFormatoPassword(password);

    // Assert
    Assert.True(resultado);
}
```

### Estadísticas TDD
- **Total de pruebas:** 21
- **Tasa de éxito:** 100%
- **Tiempo promedio de ejecución:** <1 segundo
- **Cobertura de líneas:** 70.7%
- **Cobertura de ramas:** 65.2%

---

## Análisis SAST

**Static Application Security Testing** implementado con SonarCloud detectó y corrigió:

### Vulnerabilidades Encontradas y Corregidas

#### 1. ReDoS (Regular Expression Denial of Service)
- **Problema:** Uso de `Regex.IsMatch()` sin timeout
- **Impacto:** Ataque DoS mediante expresiones regulares complejas
- **Solución:** Agregado `TimeSpan.FromSeconds(2)` a todas las expresiones regulares
```csharp
// ANTES (Vulnerable):
Regex.IsMatch(email, patron)

// DESPUÉS (Seguro):
Regex.IsMatch(email, patron, RegexOptions.None, TimeSpan.FromSeconds(2))
```

#### 2. Validaciones de entrada
- Validación de formato de email (RFC 5322)
- Validación de complejidad de contraseñas
- Sanitización de datos de entrada

#### 3. Protección contra Fuerza Bruta
- Bloqueo de cuenta tras 5 intentos fallidos
- Registro de intentos para análisis forense

### Resultado Final
- **0 vulnerabilidades** críticas
- **0 Security Hotspots** sin revisar
- **Calificación A** en seguridad

---

## Code Smells Identificados

SonarCloud identificó **21 Code Smells** (todos de mantenibilidad, ninguno crítico):

### Distribución por Tipo

| Tipo | Cantidad | Severidad | Estado |
|------|----------|-----------|--------|
| Propiedades nullable | 17 | Media | Documentado |
| Código comentado | 3 | Media | Por refactorizar |
| Campo no usado (_logger) | 1 | Alta | Por implementar |
| Optimización string | 1 | Baja | Por optimizar |

### Justificación
Los Code Smells identificados son mejoras de mantenibilidad que no afectan la funcionalidad ni la seguridad del sistema. Se priorizaron primero las vulnerabilidades (0), bugs críticos (0) y cobertura de pruebas (70.7%), cumpliendo todos los requisitos establecidos para el proyecto.

---

## Autor

**Santiago Quintero Cano**  
Estudiante de Ingeniería Informatica  
Universidad De Enviago

- GitHub: [quintero3010](https://github.com/quintero3010)  
- SonarCloud: [quintero3010](https://sonarcloud.io/organizations/quintero3010)

---

## Licencia

Este proyecto es parte de una actividad académica de la materia **Calidad de Software** - Universidad Pontificia Bolivariana.

---

## Agradecimientos

- Profesor de Calidad de Software - UPB
- Comunidad de .NET y xUnit
- SonarSource por SonarCloud
- Microsoft por ASP.NET Core

---

## Referencias

- [Metodología TDD](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [xUnit Documentation](https://xunit.net/)
- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [C# Nullable Reference Types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references)

---

**Última actualización:** Octubre 2025  
**Estado del proyecto:** Completo - Calidad A en SonarCloud