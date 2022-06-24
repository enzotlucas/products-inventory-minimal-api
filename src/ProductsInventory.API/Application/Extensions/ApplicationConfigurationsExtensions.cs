namespace ProductsInventory.API.Application.Extensions
{
    public static class ApplicationConfigurationsExtensions
    {
        public static void AddApplicationConfigurations(this WebApplicationBuilder builder, params Type[] scanMarkers)
        {
            var configurations = new List<IDefinition>();

            foreach (var scanMarker in scanMarkers)
                configurations.AddRange(scanMarker.Assembly.ExportedTypes
                                                    .Where(e => typeof(IDefinition).IsAssignableFrom(e) && e.BaseType is not null)
                                                    .Select(Activator.CreateInstance).Cast<IDefinition>());

            foreach (var configuration in configurations)
                configuration.DefineServices(builder);

            builder.Services.AddSingleton(configurations as IReadOnlyCollection<IDefinition>);
        }

        public static void UseApplicationConfigurations(this WebApplication app)
        {
            var configurations = app.Services.GetRequiredService<IReadOnlyCollection<IDefinition>>();

            foreach (var configuration in configurations)
                configuration.DefineActions(app);
        }
    }
}
