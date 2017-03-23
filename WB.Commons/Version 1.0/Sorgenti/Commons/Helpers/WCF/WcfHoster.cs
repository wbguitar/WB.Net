using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web;
using WB.IIIParty.Commons.Logger;

namespace WB.Commons.Helpers.WCF
{
    public class WcfHoster : IDisposable
    {
        public event Action OnStart = () => { };
        public event Action OnStop = () => { };
        public event Action Disposing = () => { };
        public event Action<Exception> OnError = (exc) => { };
        public event Action<LogLevels, object, string> OnLog = (ll, obj, str) => { };

        ServiceHost host;
        Uri baseAddress;

        object service;

        public WcfHoster(string _baseAddress, object _service)
        {
            baseAddress = new Uri(_baseAddress);
            service = _service;
        }

        public void Start()
        {
            try
            {

                if (host != null)
                    host.Close();

                host = new ServiceHost(service.GetType(), baseAddress);

                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                OnLog(LogLevels.Info, "The service is ready at {0}", baseAddress.ToString());

                OnStart();
            }
            catch (Exception exc)
            {
                OnLog(LogLevels.Error, this, exc.ToString());
                OnError(exc);
            }
        }

        public void Stop()
        {
            try
            {
                if (host == null)
                    //|| host.State != CommunicationState.Opened) 
                    return;

                // Close the ServiceHost.
                host.Close();
                host = null;

                OnStop();
            }
            catch (Exception exc)
            {
                OnLog(LogLevels.Error, this, exc.ToString());
                OnError(exc);
            }
        }

        public void Dispose()
        {
            Stop();
            Disposing();
        }
    }

    public class WcfHoster<T>: IDisposable
    {
        public event Action OnStart = () => { };
        public event Action OnStop = () => { };
        public event Action Disposing = () => { };
        public event Action<Exception> OnError = (exc) => { };
        public event Action<LogLevels, object, string> OnLog = (ll, obj, str) => { };

        ServiceHost host;
        Uri baseAddress;
        public WcfHoster(string _baseAddress)
        {
            baseAddress = new Uri(_baseAddress);
        }

        public void Start()
        {
            try
            {

                if (host != null)
                    host.Close();

                host = new ServiceHost(typeof(T), baseAddress);

                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                OnLog(LogLevels.Info, "The service is ready at {0}", baseAddress.ToString());

                OnStart();
            }
            catch (Exception exc)
            {
                OnLog(LogLevels.Error, this, exc.ToString());
                OnError(exc);
            }
        }

        public void Stop()
        {
            try
            {
                if (host == null )
                    //|| host.State != CommunicationState.Opened) 
                    return;
                
                // Close the ServiceHost.
                host.Close();
                host = null;

                OnStop();
            }
            catch (Exception exc)
            {
                OnLog(LogLevels.Error, this, exc.ToString());
                OnError(exc);
            }
        }

        public void Dispose()
        {
            Stop();
            Disposing();
        }
    }
}