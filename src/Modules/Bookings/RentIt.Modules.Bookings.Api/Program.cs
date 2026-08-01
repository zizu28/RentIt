// This file is required because the project is a Web SDK.
// The actual entry point is in the Host project.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();
