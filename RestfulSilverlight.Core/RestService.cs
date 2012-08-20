using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestfulSilverlight.Core
{
    public delegate void CompleteCallback();
    public delegate void CompleteCallback<T>(T result);

    public class RestService
    {
        private string _baseUri;
        protected string BaseUri
        {
            get
            {
                return _baseUri;
            }
        }

        private RestFacilitator _restFacilitator;
        protected RestFacilitator RestFacilitator
        {
            get
            {
                return _restFacilitator;
            }
        }

        public RestService()
            : this(AsyncDelegation.BaseUri)
        {

        }

        public RestService(string baseUri)
        {
            _restFacilitator = new RestFacilitator(SynchronizationContext.Current);
            _baseUri = baseUri;
            if (_baseUri.EndsWith("/") == false)
            {
                _baseUri = _baseUri + "/";
            }
        }

        protected SendOrPostCallback Wrap<T>(CompleteCallback<T> complete) where T : class, new()
        {
            SendOrPostCallback post = (data) => complete(data as T);
            return post;
        }

        private string GenerateGetUri(object resource)
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo controllerProperty = resource.GetType().GetProperty("controller");
            if (controllerProperty == null)
            {
                throw new ArgumentNullException("Controller name is required");
            }
            string controller = resource.GetType().GetProperty("controller").GetValue(resource, null).ToString();

            PropertyInfo actionProperty = resource.GetType().GetProperty("action");
            if (actionProperty == null)
            {
                throw new ArgumentNullException("Action name is required");
            }
            string action = resource.GetType().GetProperty("action").GetValue(resource, null).ToString();

            sb.Append(BaseUri);
            sb.Append(controller);
            sb.Append("/");
            sb.Append(action);
            sb.Append("/");

            bool isFirst = true;
            var propertyInfo = resource.GetType().GetProperties().Where(p => p.Name != "controller" && p.Name != "action").ToList();
            foreach (var property in propertyInfo)
            {
                string value = property.GetValue(resource, null).ToString();
                if (isFirst)
                {
                    sb.Append("?");
                    isFirst = false;
                }
                else
                {
                    sb.Append("&");
                }

                sb.Append(property.Name);
                sb.Append("=");
                sb.Append(value);
            }

            return sb.ToString();
        }

        private string GeneratePostUri(object resource)
        {
            StringBuilder sb = new StringBuilder();
            string controller = resource.GetType().GetProperty("controller").GetValue(resource, null).ToString();
            string action = resource.GetType().GetProperty("action").GetValue(resource, null).ToString();
            sb.Append(BaseUri);
            sb.Append(controller);
            sb.Append("/");
            sb.Append(action);
            sb.Append("/");
            return sb.ToString();
        }

        private Dictionary<string, string> GeneratePostDictonary(object resource)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var propertyInfo = resource.GetType().GetProperties().Where(p => p.Name != "controller" && p.Name != "action").ToList();
            foreach (var property in propertyInfo)
            {
                string value = property.GetValue(resource, null).ToString();
                parameters.Add(property.Name, value);
            }

            return parameters;
        }

        public void Call(string methodName, params object[] args)
        {
            this.GetType().GetMethod(methodName).Invoke(this, args);
        }

        public void Get<T>(object resource, CompleteCallback<T> completeCallback) where T : class, new()
        {
            string fullyQualifiedUri = GenerateGetUri(resource);
            _restFacilitator.Get<T>(fullyQualifiedUri, Wrap<T>(completeCallback));
        }

        public void Post(object resource, CompleteCallback completeCallback)
        {
            string fullyQualifiedUri = GeneratePostUri(resource);
            Dictionary<string, string> parameters = GeneratePostDictonary(resource);
            _restFacilitator.Post(fullyQualifiedUri, parameters, (data) => completeCallback());
        }
    }
}
