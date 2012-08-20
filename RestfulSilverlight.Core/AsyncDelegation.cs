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
using System.Collections.Generic;
using System.Linq;

namespace RestfulSilverlight.Core
{
    public class AsyncDelegation
    {
        public static string BaseUri { get; set; }

        private RestFacilitator _restFacilitator;
        private dynamic _restServiceContainer;
        public AsyncDelegation()
            : this(new RestFacilitator())
        {

        }

        public AsyncDelegation(RestFacilitator restFacilitator)
        {
            _restFacilitator = restFacilitator;
        }

        public GetContainer<T> Get<T>(object route) where T : class, new()
        {
            _restServiceContainer = new GetContainer<T>(route);
            return _restServiceContainer;
        }

        public GetContainer<T> Get<T>() where T : class, new()
        {
            _restServiceContainer = new GetContainer<T>(null);
            return _restServiceContainer;
        }

        public PostContainer Post(object route)
        {
            _restServiceContainer = new PostContainer(route);
            return _restServiceContainer;
        }

        public PostContainer Post()
        {
            _restServiceContainer = new PostContainer(null);
            return _restServiceContainer;
        }

        public void Go()
        {
            if (_restServiceContainer != null)
            {
                _restServiceContainer.Go();
            }
        }
    }
}
