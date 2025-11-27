using Marten.Schema;

namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();

            if (await session.Query<Product>().AnyAsync())
                return;

            session.Store<Product>(GetPreconfiguredProducts());
            await session.SaveChangesAsync(cancellation);
        }

        public static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>
        {
            // --- Página 1 ---
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop Ultrafino X1",
                Category = new List<string> { "Eletrônicos", "Informática" },
                Description = "Laptop de última geração com processador i9 e 32GB de RAM.",
                ImageFile = "laptop_ultrafino_x1.jpg",
                Price = 7999.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Fones Bluetooth SoundPro",
                Category = new List<string> { "Eletrônicos", "Áudio" },
                Description = "Fones com cancelamento de ruído ativo e som cristalino.",
                ImageFile = "fones_soundpro.png",
                Price = 450.00m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Câmera DSLR EOS 900",
                Category = new List<string> { "Fotografia", "Eletrônicos" },
                Description = "Câmera profissional com sensor de 24MP e gravação 4K.",
                ImageFile = "camera_dslr_eos900.webp",
                Price = 3200.50m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mouse Ergonômico Vertical",
                Category = new List<string> { "Informática", "Acessórios" },
                Description = "Mouse projetado para prevenir lesões por esforço repetitivo (LER).",
                ImageFile = "mouse_ergonomico.jpg",
                Price = 95.90m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Smartwatch Alpha 3",
                Category = new List<string> { "Eletrônicos", "Vestível" },
                Description = "Relógio inteligente com monitoramento cardíaco e GPS integrado.",
                ImageFile = "smartwatch_alpha3.jpg",
                Price = 899.00m
            },

            // --- Página 2 ---
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Teclado Mecânico RGB",
                Category = new List<string> { "Informática", "Acessórios" },
                Description = "Teclado com switches táteis e iluminação RGB customizável.",
                ImageFile = "teclado_mecanico.jpg",
                Price = 350.00m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Tablet Pro P12",
                Category = new List<string> { "Eletrônicos", "Informática" },
                Description = "Tablet com tela OLED de 12 polegadas e caneta digital.",
                ImageFile = "tablet_pro_p12.png",
                Price = 2800.00m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Monitor Gamer 144Hz",
                Category = new List<string> { "Eletrônicos", "Informática" },
                Description = "Monitor de 27 polegadas com taxa de atualização de 144Hz e tempo de resposta de 1ms.",
                ImageFile = "monitor_gamer.jpg",
                Price = 1500.00m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Roteador Mesh Tri-Band",
                Category = new List<string> { "Redes", "Eletrônicos" },
                Description = "Sistema Mesh para cobertura Wi-Fi em toda a casa sem pontos cegos.",
                ImageFile = "roteador_mesh.jpg",
                Price = 650.00m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Caixa de Som Portátil Aqua",
                Category = new List<string> { "Áudio", "Eletrônicos" },
                Description = "Caixa de som à prova d'água com 20 horas de bateria.",
                ImageFile = "caixa_som_portatil.jpg",
                Price = 180.00m
            }
        };
    }
}
