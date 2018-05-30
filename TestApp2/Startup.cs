﻿using MetaQuotes.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MetaQuotes.Models;

namespace TestApp2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        IMemoryCache memoryCache;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddRouting();
            services.AddMvc();

            services.AddScoped<ISearchService, SearchService>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            memoryCache = cache;

            app.UseMvc();
            app.UseStaticFiles();

            LoadGeoBase();
        }

        public void LoadGeoBase(){
            var db = new GeoBase();

            if (!memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out db))
            {
                var loader = new BinaryLoader();
                db = loader.LoadDb("/Documents/geobase.dat");

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                memoryCache.Set(CacheConstants.GeoBaseKey, db, cacheEntryOptions);
            }
        }
    }
}