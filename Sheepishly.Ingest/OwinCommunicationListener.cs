using System;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace Sheepishly.Ingest
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly IOwinAppBuilder _startup;
        private readonly string _appRoot;

        private string _address;

        private IDisposable _handle;

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup)
        {
            _startup = startup;
            _appRoot = appRoot;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
//            var endpoint = _parameters.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");

            var port = 8100;// endpoint.Port;
            var root = String.IsNullOrWhiteSpace(_appRoot) ? String.Empty : _appRoot.TrimEnd('/') + '/';

            var culture = CultureInfo.InvariantCulture;
            _address = String.Format(culture, "http://+:{0}/{1}", port, root);

            _handle = WebApp.Start(_address, a => _startup.Configuration(a));

            var publishAddress = _address.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            ServiceEventSource.Current.Message("Listening on {0}", publishAddress);
            return Task.FromResult(publishAddress);
        }

        public void Abort()
        {
            StopWebServer();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            StopWebServer();
            return Task.FromResult(true);
        }

        private void StopWebServer()
        {
            if (_handle == null)
                return;

            try
            {
                _handle.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}