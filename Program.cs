using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixMinApi.Data;
using SixMinApi.DTOs;
using SixMinApi.models;
using WatchDog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var sqlConnectionBuilder = new SqlConnectionStringBuilder();
//sqlConnectionBuilder.ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnectionApp")));

//sqlConnectionBuilder.UserID = builder.Configuration["User ID"];
//sqlConnectionBuilder.Password = builder.Configuration["Password"];

//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlConnectionBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddWatchDogServices(settings => {
    settings.IsAutoClear = false; //After a certain amount of time, clear the logs
    //settings.ClearTimeSchedule = WatchDog.src.Enums.WatchDogAutoClearScheduleEnum.Weekly; //Clear the logs weekly
    settings.SetExternalDbConnString = builder.Configuration.GetConnectionString("SQLDbConnectionApp");
    settings.DbDriverOption = WatchDog.src.Enums.WatchDogDbDriverEnum.MSSQL;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWatchDogExceptionLogger();
app.UseWatchDog(opt =>{
    opt.WatchPageUsername = "admin";
    opt.WatchPagePassword = "admin";
});
app.UseHttpsRedirection();

app.MapGet("api/v1/commands", async (ICommandRepo repo, IMapper mapper) => {
    WatchLogger.Log("Ran the get all commands endpoint");
    var commands = await repo.GetAllCommandsAsync();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
});

app.MapGet("api/v1/commands/{id}", async (ICommandRepo repo, int id, IMapper mapper) => {
    var command = await repo.GetCommandByIdAsync(id);
    if (command == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(mapper.Map<CommandReadDto>(command));
});

app.MapPost("api/v1/commands", async (ICommandRepo repo, CommandCreateDto commandCreateDto, IMapper mapper) => {
    var command = mapper.Map<Command>(commandCreateDto);
    await repo.CreateCommandAsync(command);
    await repo.SaveChangesAsync();
    var cmdReadDto = mapper.Map<CommandReadDto>(command);
    //return Results.CreatedAtRoute("api/v1/commands/{id}", new { id = cmdReadDto.Id }, cmdReadDto);
    return Results.Created($"api/v1/commands/{cmdReadDto.Id}", cmdReadDto);
});

app.MapPut("api/v1/commands/{id}", async (ICommandRepo repo, int id, CommandUpdateDto commandUpdateDto, IMapper mapper) => {
    var command = await repo.GetCommandByIdAsync(id);
    if (command == null)
    {
        return Results.NotFound();
    }
    mapper.Map(commandUpdateDto, command);
    await repo.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("api/v1/commands/{id}", async (ICommandRepo repo, int id) => {
    var command = await repo.GetCommandByIdAsync(id);
    if (command == null)
    {
        return Results.NotFound();
    }
    repo.DeleteCommand(command);
    await repo.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

