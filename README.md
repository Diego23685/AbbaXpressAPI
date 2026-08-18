# Abba Xpress ERP — Backend API

API RESTful multisede desarrollada con **.NET 8/9** para la gestión logística, recepción de carga, facturación multimoneda (**USD / NIO**), auditoría de transacciones y traslados entre sucursales.

**Sucursales:** Managua y León.

---

## 🛠️ Requisitos previos

* [.NET SDK 8.0 o superior](https://dotnet.microsoft.com/download)
* [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/) o MariaDB / SQLite
* [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
* Extensión de **C#** para VS Code, si corresponde
* **Entity Framework Core CLI** para ejecutar migraciones

---

## ⚙️ Configuración inicial

### 1. Clonar o abrir el proyecto

Ubícate en la carpeta del proyecto:

```bash
cd AbbaXpress.API
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Configurar la conexión a la base de datos

Crea o verifica el archivo `appsettings.json` en la raíz del proyecto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=abbaxpress_db;Uid=root;Pwd=tu_contraseña;"
  },
  "Jwt": {
    "Key": "ClaveSecretaSuperLargaYProtegidaParaFirmarJWT2026*",
    "Issuer": "AbbaXpressAPI",
    "Audience": "AbbaXpressApp"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> **Importante:** En un entorno de producción, no se recomienda almacenar contraseñas, claves JWT u otros secretos directamente en `appsettings.json`. Utiliza variables de entorno o un gestor de secretos.

### 4. Aplicar migraciones y datos iniciales

Ejecuta las migraciones de Entity Framework Core para crear las tablas y cargar los datos iniciales de sedes, configuraciones y cuentas base:

```bash
dotnet ef database update
```

Si `dotnet ef` no está instalado:

```bash
dotnet tool install --global dotnet-ef
```

---

## 🚀 Ejecución del servidor

Inicia la API en modo desarrollo:

```bash
dotnet run
```

### URLs disponibles

* **HTTP:** http://localhost:5271
* **HTTPS:** https://localhost:7271
* **Swagger UI:** http://localhost:5271/swagger

Swagger está disponible cuando la aplicación se ejecuta en el entorno de desarrollo.

---

## 👤 Credenciales iniciales

Las siguientes cuentas son creadas mediante el **Seed Data**:

| Rol                 | Usuario     | Contraseña  | Sucursal                    |
| ------------------- | ----------- | ----------- | --------------------------- |
| Super Admin         | `admin`     | `Admin123*` | Bolonia - Central (Managua) |
| Admin Independiente | `adminleon` | `Leon123*`  | León                        |

> **Seguridad:** Estas credenciales son únicamente para desarrollo o configuración inicial. Cámbialas antes de utilizar el sistema en producción.

---

## 📌 Controladores principales

| Endpoint           | Descripción                                                      |
| ------------------ | ---------------------------------------------------------------- |
| `/api/auth`        | Autenticación mediante JWT y validación de tokens.               |
| `/api/proformas`   | Recepción multipaquete, liquidación y despacho intersucursal.    |
| `/api/clientes`    | Gestión del directorio de clientes con aislamiento por sede.     |
| `/api/finanzas`    | Control de gastos operativos y balances de utilidad.             |
| `/api/auditoria`   | Bitácora cronológica de acciones y transacciones.                |
| `/api/exportacion` | Gestión de manifiestos de paquetería de exportación (FedEx USA). |
| `/api/usuarios`    | Gestión de cuentas, roles y asignación de usuarios a sucursales. |

---

## 🏢 Arquitectura multisede

El sistema está diseñado para trabajar con múltiples sucursales, permitiendo:

* Gestión independiente de **Managua y León**.
* Aislamiento de información según la sucursal del usuario.
* Traslados de carga entre sucursales.
* Control de operaciones y movimientos por sede.
* Administración centralizada mediante usuarios con permisos especiales.

---

## 💰 Facturación multimoneda

El sistema permite trabajar con:

* **USD — Dólares estadounidenses**
* **NIO — Córdobas nicaragüenses**

Las operaciones financieras pueden ser registradas y auditadas de acuerdo con la moneda correspondiente.

---

## 🔐 Seguridad

La API utiliza **JWT (JSON Web Tokens)** para la autenticación y autorización de usuarios.

Se recomienda para producción:

1. Utilizar claves JWT seguras y almacenarlas fuera del código fuente.
2. Cambiar las credenciales iniciales.
3. Utilizar HTTPS.
4. Configurar correctamente las variables de entorno.
5. No subir contraseñas ni secretos al repositorio.
6. Mantener actualizados .NET, Entity Framework Core y los paquetes NuGet.

---

## 🧪 Documentación de la API

Una vez iniciado el servidor, puedes consultar y probar los endpoints desde Swagger:

```text
http://localhost:5271/swagger
```

Swagger permite visualizar los endpoints disponibles, sus parámetros, respuestas y esquemas de datos.

---

## 📦 Comandos útiles

Restaurar dependencias:

```bash
dotnet restore
```

Compilar el proyecto:

```bash
dotnet build
```

Ejecutar la API:

```bash
dotnet run
```

Crear una nueva migración:

```bash
dotnet ef migrations add NombreDeLaMigracion
```

Aplicar migraciones:

```bash
dotnet ef database update
```

---

## 📄 Licencia

Este proyecto es propiedad de **Abba Xpress** y su uso, distribución o modificación está sujeto a las condiciones establecidas por sus propietarios.
