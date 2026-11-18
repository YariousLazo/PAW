# PAW (PP1 - PP2) 30 Septiembre
### 1. Flujo completo por entidad (Repository → API → DTO → Service → MVC → View)

Se replicó el flujo que ya existía con `Product` para: Category, Component, Inventory,
Notification, Role, Supplier, PawTask (entidad `Task`), User, UserAction y UserRole.
- Todos los repositorios se registraron en `PAW.API/Program.cs` y todos los servicios en `PAW.Web/Program.cs`.
- `Task` se nombró `PawTask` en clases, rutas y vistas.
- Las vistas Index replican el aspecto de `Product/Index`.
- `_Layout.cshtml`: menú desplegable "Entities" con acceso a las 11 entidades.

# PAW (PP3) 9 Octubre
### 2. Edit, Details, Add y Delete + paginación

**Partial Views reutilizables**
- `Views/Shared/_CrudModal.cshtml`: modal único incluido en el `_Layout`.
- `wwwroot/js/site.js`: carga los partials con `fetch` y envía los formularios sin recargar la página.
- `Views/Shared/_DeleteConfirm.cshtml`: confirmación de borrado reutilizable.
- `Views/X/_Details.cshtml`, `_DetailsBody.cshtml` y `_Form.cshtml` en cada entidad.

**Details de Product**
- Muestra el detalle de su Category, Inventory y Supplier asociados, reutilizando los `_DetailsBody` de cada una (`ProductDetailsViewModel`).

**Paginación**
- `Models/PagedList.cs` y `Views/Shared/_Pagination.cshtml`: máximo 25 por página, formato `<< < ... 5 [6] 7 ... > >>`.

**Services y controllers**
- Services: `GetByIdAsync`, `SaveAsync` y `DeleteAsync`.
- API: GET por id, POST (crea o actualiza) y DELETE por id.
- MVC: acciones `Details`, `Create`, `Edit`, `Save` y `Delete`.

**Cambios en código existente**
- `ProductController` (API): `Save` ahora recibe `ProductDTO` y corrige la lógica invertida (crear/actualizar); `Delete` ahora es por id.
- `ProductDTO`: se agregaron `CategoryId`, `SupplierId` e `InventoryId`; `Description` y `Comments` aceptan null.
- `Product/Index.cshtml`: los botones usan `ProductId` en lugar del Guid aleatorio.
- `UserAction` y `UserRole`: `Id` pasó a `decimal` no nulo y `ProductDbContext` usa `HasKey(e => e.Id)` en lugar de `HasNoKey()`.
- `PAW.Web/Program.cs`: cultura invariante para decimales y fechas.
- `ServiceBase`: helper `IsNotFound`.

# PAW (Tarea 2 y 3) 17 Noviembre
### 3. Tipo de cambio por JavaScript (ExchangeRate)

- `wwwroot/js/site.js`: `getExchangeRate()` consulta `https://api.hacienda.go.cr/indicadores/tc` desde el navegador y **cachea** el resultado en `localStorage` (1 hora). Si la API falla usa el último valor guardado.
- `Views/Shared/ExchangeRate.cshtml`: tarjeta flotante arriba a la derecha (sin modelo, se llena desde JS). Se incluye una sola vez en `_Layout.cshtml`, por lo que aparece en todas las pantallas.
- Estilos en `wwwroot/css/site.css` (`.fx-widget`).

### 4. PAW.MinimalAPI y pantalla de Tasks

- Nuevo proyecto `PAW.MinimalAPI` (Minimal API, https://localhost:7200) con el modelo `Task`: `GET /tasks`, `GET /tasks/{id}`, `GET /tasks/completed`, `POST /tasks`, `PUT /tasks/{id}`, `DELETE /tasks/{id}`.
- Dependency Injection: los endpoints reciben `ITaskRepository` (registrado en `Program.cs` como `InMemoryTaskRepository`). No usa el DbContext de Entity Framework.
- `PAW.Web`: `TaskApiService` (consume el Minimal API, URL en `MinimalApi:BaseUrl` de `appsettings.json`), `TasksController` y `Views/Tasks` (mismo flujo de modales, `_DeleteConfirm` y paginación del resto de entidades).
- `_Layout.cshtml`: nuevo menú "Tasks (Minimal API)". El menú "Entities > Task" sigue usando `PAW.API` y la base de datos.
- `PAW.sln` y `PAW.slnLaunch.user`: se agregó `PAW.MinimalAPI` (iniciar Web, API y MinimalAPI a la vez).

### 5. Controllers de Web completados

- Los controllers de `PAW.Web` (Category, Component, Inventory, Notification, PawTask, Product, Role, Supplier, User, UserAction y UserRole) solo tenian `Index` y devolvian una lista; las vistas ya esperaban `PagedList<T>`.
- Ahora cada uno tiene `Index(page)` paginado y las acciones `Details`, `Create`, `Edit`, `Save` y `Delete` (GET de confirmacion + POST), que devuelven los partials que carga el modal (`site.js`).
- `Product`: `Details` agrega Category, Inventory y Supplier (`ProductDetailsViewModel`); `Create` y `Edit` llenan los dropdowns (`ViewBag.Categories`, `Suppliers`, `Inventories`).
- `UserAction` y `UserRole`: el Id lo digita el usuario al agregar (`ViewData["IsNew"]`); se avisa si el Id ya existe.
