using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using AngleSharp;
using MODELS.Model;
using Microsoft.Extensions.Configuration;


namespace MANAGER
{
    public interface IManagerExtraer
    {
        Task<List<string>> ExtraerEjemplos(string nombre);
        Task<string> ExtraerHTML(string url);
        Task<List<string>> Extraer(string url, string tag);
    }
    public class ManagerExtraer : IManagerExtraer
    {
        private readonly Configuration _configuration;
        private readonly Webs webs_config;
        private readonly string tag = "Webs";
        public ManagerExtraer(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            webs_config = new Webs();
            configuration.GetSection(tag).Bind(webs_config);
        }
        public async Task<string> ExtraerHTML(string url)
        {
            try
            {
                List<string> lst_productos = new List<string>();
                var config = Configuration.Default.WithDefaultLoader();
                var address =url;
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(address);
                var productos = document.QuerySelector("body");
                string response = productos.InnerHtml.Trim();
                return response;
            }
            catch (Exception e)
            {
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<List<string>> ExtraerEjemplos(string nombre)
        {
            try
            {
                Webs webs = webs_config;
                var web_selecionada = webs.lista_webs.Find(x => x.cod == nombre);
                List<string> lst_items = new List<string>();
                var config = Configuration.Default.WithDefaultLoader();
                var address = web_selecionada.url;
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(address);
                var items = document.QuerySelectorAll(web_selecionada.tag);

                if (nombre.Contains("CEIBO"))
                {
                    foreach (var producto in items)
                    {
                        lst_items.Add(producto.TextContent.Trim());
                    }
                }
                else if(nombre.Contains("NFL"))
                {
                    foreach (var producto in items)
                    {
                        lst_items.Add(producto.TextContent.Trim());
                    }
                }
                return lst_items;
            }
            catch (Exception e)
            {
                throw new NotImplementedException($"ERRROR AL INTENAR EXTRAER LOS DATOS: {e.Message}");
            }
        }
        public async Task<List<string>> Extraer(string url, string tag)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new Exception($"ERRROR: NO SE PUEDE ENVIAR LA URL VACIA: {url}");
                if (string.IsNullOrEmpty(tag))
                    throw new Exception($"ERRROR: NO SE PUEDE ENVIAR EL TAG VACIO: {tag}");

                List<string> lst_items = new List<string>();
                var config = Configuration.Default.WithDefaultLoader();
                var address = url;
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(address);
                var items = document.QuerySelectorAll($".{tag}");

                foreach (var producto in items)
                {
                    lst_items.Add(producto.TextContent.Trim());
                }

                return lst_items;
            }
            catch (Exception e)
            {
                throw new Exception($"ERRROR AL INTENAR EXTRAER LOS DATOS: {e.Message}");
            }
        }
    }
}
