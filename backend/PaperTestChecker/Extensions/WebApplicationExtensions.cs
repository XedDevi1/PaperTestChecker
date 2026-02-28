namespace PaperTestChecker.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseDefaultPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("FrontendDev");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
