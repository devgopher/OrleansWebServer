using System.Threading.Tasks;

namespace Runner.Interfaces
{
    public interface IRunner
    {
        /// <summary>
        /// Creates container on all available servers
        /// </summary>
        /// <param name="containerName">name</param>
        /// <returns></returns>
        public Task CreateSiloContainer(string containerName);

        /// <summary>
        /// Creates container on a concrete server
        /// </summary>
        /// <param name="containerName">name</param>
        /// <param name="server">Server</param>
        /// <returns></returns>
        public Task<bool> CreateSiloContainer(string containerName, Server server);

        /// <summary>
        /// Updates container on all available servers
        /// </summary>
        /// <param name="containerName">name</param>
        /// <returns></returns>
        public Task UpdateSiloContainer(string containerName);

        /// <summary>
        /// Updates container on a concrete server
        /// </summary>
        /// <param name="containerName">name</param>
        /// <param name="server">Server</param>
        /// <returns></returns>
        public Task<bool> UpdateSiloContainer(string containerName, Server server);

        /// <summary>
        /// Removes container from a concrete server
        /// </summary>
        /// <param name="containerName">name</param>
        /// <param name="server">Server</param>
        /// <returns></returns>
        public Task<bool> RemoveSiloContainer(string containerName, Server server);

        /// <summary>
        /// Removes container from all available servers
        /// </summary>
        /// <param name="containerName">name</param>
        /// <returns></returns>
        public Task RemoveSiloContainer(string containerName);
    }
}
