using OnlineLibrarySystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OnlineLibrarySystem.Controllers // namespace is similar to packages in java
{
    public class ValuesController : ApiController // (extends ApiController)
    {
        /** 
         * Sample API Method
         * @returns books titles
         */
        [HttpGet]
        [Route("api/Values/GetBooks")] // Call me at "http://localhost:0000/api/Values/GetBooks", replace 0000 with port number
        public List<string> GetBooks()
        {
            var ctx = new OnlineLibrarySystemEntities();
            return ctx.Database.SqlQuery<string>( /* Your SQL query goes here */
                    "SELECT BookTitle FROM Books").ToList();
        }

        [HttpGet] // Used usually to "GET" Values, use [HttpPost] for inserting data (more secure)
        [Route("api/Values/GetAuthorNames")]
        public List<string> GetAuthorNames()
        {
            // Just another example to implement later
            return null;
        }

    }
}
