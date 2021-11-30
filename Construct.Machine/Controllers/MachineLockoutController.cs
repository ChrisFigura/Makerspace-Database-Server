using System.Linq;
using System.Threading.Tasks;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.MachineLockout.Data.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.MachineLockout.Controllers
{
    public class MachineLockoutController : Controller
    {
        /// <summary>
        /// Checks if a user is authorized to use a machine.
        /// </summary>
        /// <param name="machine">Machine to be activated.</param>
        /// <param name="hashedId">Hashed id of the user.</param>
        [HttpGet]
        [Path("/machine/getauthorized")]
        public async Task<ActionResult<IResponse>> GetAuthorized(string machine, string hashedId)
        {
            // Return if no hashed id was provided.
            if (hashedId == null)
            {
                Response.StatusCode = 400;
                return new GenericStatusResponse("missing-hashed-id");
            }

            // Get the user
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.Permissions)
                .FirstOrDefaultAsync(user => user.HashedId.ToLower() == hashedId.ToLower());

            // Check if the user is authorized (for now, if they're a lab manager)
            var labmanager = user?.Permissions.FirstOrDefault(p => p.Name.ToLower() == "labmanager");
            if (labmanager is not null && labmanager.IsActive())
            {
                return new ActivateMachineResponse()
                {
                    Name = user.Name,
                    MaxSessionTime = 3600
                };
            }
            else
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
        }
    }
}
