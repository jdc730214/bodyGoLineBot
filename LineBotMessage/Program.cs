using System.Reflection;
using LineBotMessage;
using LineBotMessage.Hub;
using LineBotMessage.Providers;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddSignalR();   // ���U SignalR �A��
builder.Services.AddCors(options =>    // ���U CORS �A�ȡA���\��ӷ��ШD
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// add swagger config
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CORSPolicy");   // �ϥΤW���w�q�� CORS ����
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "UploadFiles")),
    RequestPath = "/UploadFiles"
});

app.MapControllers();
app.UseFileServer();    // �ϥΤ��ت������n�鴣���R�A�ɮ�
app.MapHub<MessageHub>("/messageHub");   // �M�g SignalR Hub �� "/messageHub"(���e�ݪ��s�u�r��)

app.Run();