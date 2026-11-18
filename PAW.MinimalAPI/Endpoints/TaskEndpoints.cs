using Microsoft.AspNetCore.Http.HttpResults;
using PAW.MinimalAPI.Data;
using PawTask = PAW.Models.Task;

namespace PAW.MinimalAPI.Endpoints;

public static class TaskEndpoints
{
    public static RouteGroupBuilder MapTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/tasks").WithTags("Tasks");

        group.MapGet("/", GetAllTasks);
        group.MapGet("/completed", GetCompletedTasks);
        group.MapGet("/{id:int}", GetTask);
        group.MapPost("/", CreateTask);
        group.MapPut("/{id:int}", UpdateTask);
        group.MapDelete("/{id:int}", DeleteTask);

        return group;
    }

    // The ITaskRepository parameter is injected by the DI container.
    private static async Task<Ok<IEnumerable<PawTask>>> GetAllTasks(ITaskRepository repository)
        => TypedResults.Ok(await repository.GetAllAsync());

    private static async Task<Ok<IEnumerable<PawTask>>> GetCompletedTasks(ITaskRepository repository)
    {
        var tasks = await repository.GetAllAsync();
        return TypedResults.Ok(tasks.Where(t => t.Status == InMemoryTaskRepository.Completed));
    }

    private static async Task<Results<Ok<PawTask>, NotFound>> GetTask(int id, ITaskRepository repository)
        => await repository.GetByIdAsync(id) is { } task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    private static async Task<Results<Created<PawTask>, ValidationProblem>> CreateTask(PawTask input, ITaskRepository repository)
    {
        if (Validate(input) is { } errors)
            return TypedResults.ValidationProblem(errors);

        var created = await repository.CreateAsync(input);
        return TypedResults.Created($"/tasks/{created.Id}", created);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateTask(int id, PawTask input, ITaskRepository repository)
    {
        if (Validate(input) is { } errors)
            return TypedResults.ValidationProblem(errors);

        return await repository.UpdateAsync(id, input)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTask(int id, ITaskRepository repository)
        => await repository.DeleteAsync(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();

    private static Dictionary<string, string[]>? Validate(PawTask task)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(task.Name))
            errors["name"] = ["Name is required."];
        else if (task.Name.Length > 255)
            errors["name"] = ["Name cannot be longer than 255 characters."];

        if (task.Description is { Length: > 1000 })
            errors["description"] = ["Description cannot be longer than 1000 characters."];

        if (task.Status is { Length: > 50 })
            errors["status"] = ["Status cannot be longer than 50 characters."];

        return errors.Count == 0 ? null : errors;
    }
}
