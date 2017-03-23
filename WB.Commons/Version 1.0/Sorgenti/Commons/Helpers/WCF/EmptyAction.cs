using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WB.Commons.Helpers.WCF
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EmptyActionBehaviorAttribute : Attribute, IContractBehavior
    {
        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription contractDescription,
            ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void ApplyClientBehavior(ContractDescription contractDescription,
            ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            return;
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription,
            ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            var dispatchDictionary = new Dictionary<XmlQualifiedName, string>();
            foreach (OperationDescription operationDescription in contractDescription.Operations)
            {
                XmlQualifiedName qname =
                    new XmlQualifiedName(operationDescription.Messages[0].Body.WrapperName,
                        operationDescription.Messages[0].Body.WrapperNamespace);

                dispatchDictionary.Add(qname, operationDescription.Name);
            }

            dispatchRuntime.OperationSelector =
                new EmptyActionOperationSelector(dispatchDictionary);
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        #endregion
    }

    class EmptyActionOperationSelector : IDispatchOperationSelector
    {
        Dictionary<XmlQualifiedName, string> dispatchDictionary;

        public EmptyActionOperationSelector(Dictionary<XmlQualifiedName,
            string> dispatchDictionary)
        {
            this.dispatchDictionary = dispatchDictionary;
        }

        public string SelectOperation(ref System.ServiceModel.Channels.Message message)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(message.ToString());

            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");

            XmlNode node =
                xmlDoc.SelectSingleNode("/soapenv:Envelope/soapenv:Body", nsManager).FirstChild;

            XmlQualifiedName lookupQName = new XmlQualifiedName(node.LocalName, node.NamespaceURI);

            if (dispatchDictionary.ContainsKey(lookupQName))
            {
                return dispatchDictionary[lookupQName];
            }
            else
            {
                return node.LocalName;
            }
        }

    }
}