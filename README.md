Abba Xpress ERP - Backend API (.NET 8 / 9)

API RESTful multisede para el control logístico, recepción de carga, facturación multimoneda (USD / NIO), auditoría de transacciones y traslados inter-sucursales (Managua y León).

---

## 🛠️ Requisitos Previos

- [.NET SDK 8.0 o superior](https://dotnet.microsoft.com/download)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/) o MariaDB / SQLite
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [VS Code](https://code.visualstudio.com/) con extensión de C#

---

## ⚙️ Configuración Inicial

1. **Clonar o abrir el proyecto en tu editor:**
   ```bash
   cd AbbaXpress.API
Restaurar dependencias NuGet:Bashdotnet restore
Configurar variables de entorno y conexión:Crea o verifica tu archivo appsettings.json en la raíz del proyecto:JSON{
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
Aplicar Migraciones y Seed Data:Ejecuta las migraciones para crear las tablas y sembrar los datos iniciales de sedes, configuraciones y cuentas base:Bashdotnet ef database update
🚀 Ejecución del ServidorInicia el entorno de desarrollo:Bashdotnet run
API URL base: http://localhost:5271 o https://localhost:7271Swagger UI: http://localhost:5271/swagger (disponible en modo desarrollo)👤 Credenciales Iniciales (Seed Data)Rol / AlcanceUsuarioContraseñaSucursal AsignadaSuper AdminadminAdmin123*Sucursal Bolonia - Central (Managua)Admin IndependienteadminleonLeon123*Sucursal León📌 Controladores Principales/api/auth: Autenticación JWT y validación de tokens./api/proformas: Recepción multipaquete, liquidación y despacho inter-sucursal./api/clientes: Directorio de clientes con aislamiento por sede./api/finanzas: Control de gastos operativos y balances de utilidad./api/auditoria: Bitácora cronológica inmutable de acciones./api/exportacion: Manifiestos de paquetería de exportación (FedEx USA)./api/usuarios: Gestión de cuentas y asignación de roles.
