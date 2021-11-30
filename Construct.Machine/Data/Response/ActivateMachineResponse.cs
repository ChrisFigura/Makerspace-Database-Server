using Construct.Core.Data.Response;

namespace Construct.MachineLockout.Data.Response
{
    public class ActivateMachineResponse : BaseSuccessResponse
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How long the machine can be activated for.
        /// </summary>
        public long MaxSessionTime { get; set; }
    }
}