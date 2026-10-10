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
