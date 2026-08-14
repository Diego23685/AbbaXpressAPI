---

### 2. `README.md` para el Frontend (React + Vite + Tailwind)
Guárdalo en la raíz de la carpeta del frontend (`abba-xpress-ui/README.md`):

```markdown
# Abba Xpress ERP - Frontend Web (React + Vite)

Interfaz web corporativa para operadores de bodega, recepcionistas y administradores. Incluye generador de viñetas térmicas con código de barras Code128, tickets de recepción, modo manifiesto de despacho y bitácora de auditoría.

---

## 🛠️ Requisitos Previos

- [Node.js](https://nodejs.org/) (Versión 18.x o 20.x recomendada)
- `npm`, `yarn` o `pnpm`

---

## ⚙️ Instalación y Configuración

1. **Entrar al directorio del frontend:**
   ```bash
   cd abba-xpress-ui
Instalar dependencias del proyecto:

Bash
npm install
Configurar Endpoint de la API:
Crea un archivo .env en la raíz de la carpeta del frontend si necesitas apuntar a un servidor específico:

Fragmento de código
VITE_API_URL=http://localhost:5271/api
(Por defecto src/services/api.js apunta al puerto local de la API).

🚀 Ejecución en Entorno Local
Inicia el servidor de desarrollo:

Bash
npm run dev
Abre tu navegador en:

http://localhost:5173
📦 Construcción para Producción
Para compilar y empaquetar el proyecto optimizado para hosting o despliegue:

Bash
npm run build
Los archivos estáticos listos para producción se generarán en la carpeta /dist.

🖨️ Módulos y Funcionalidades Destacadas
Recepción Multipaquete: Cálculo dinámico multimoneda (USD / NIO), tarifas por categoría editables en vivo y generación de ticket térmico/digital.

Rótulos y Manifiestos: Impresión directa de viñetas 4x6" con código de barras Code128 y confirmación de despacho inter-sucursal.

Bitácora de Auditoría: Historial cronológico con filtros avanzados por sede y módulo.

Aislamiento Multisede: Interfaces adaptadas automáticamente según los permisos de sede del usuario autenticado (Managua vs. León).
