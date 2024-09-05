using System.Collections.Generic;

namespace Application.Configurations
{
    public class AppSettings
    {
        public string ContactWebsite { get; set; }
        public List<SwaggerDoc> SwaggerDoc { get; set; }
        public CORS CORS { get; set; }
        public RedisConfiguration RedisConfiguration { get; set; }
    }
}
